using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using Domain.Repositories.Interfaces;
using Npgsql.Replication.PgOutput.Messages;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Services.Interfaces;
using System.Threading;
using API;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

using static Domain.Constants.TelegramTextConstants;
using static Domain.Constants.EmojiConstants;



namespace DiplomProject.Server.Services
{
	public class UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger, ITelegramUserRepository telegramUserRepo,
		IScienceEventRepository scienceEventRepo, INotifyService notifyService, IUserCreatedEventRepository userCreatedEventRepo,
		IFillDataService fillDataService, IPasswordHasherService passwordHasherService, ITgUserService tgUserInfoService, IConfiguration _configuration) : IUpdateHandler
	{
		#region [Paths]
		private readonly string fileSNOFullPath = _configuration["fileSNOFullPath"] ?? throw new ArgumentNullException("FileSNOFullPath is null!");
		private readonly string fileSMUFullPath = _configuration["fileSMUFullPath"] ?? throw new ArgumentNullException("FileSMUFullPath is null!");
		#endregion
		#region [TgButtons]
		//buttons
		private readonly ReplyKeyboardMarkup replyKeyboardUserSub = new ReplyKeyboardMarkup(new List<KeyboardButton[]>(){
											new KeyboardButton[]
											{
												new KeyboardButton($"{CalendarEmj} Календарь мероприятий"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{TabletEmj} Возможности работы с вашими участиями в мероприятиях"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{CheckMarkInBlockEmj} Подписаться на рассылку")
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{StatusEmj} Проверить ваши статусы и участия в мероприятиях"),
											},
			new KeyboardButton[]
			{
												new KeyboardButton($"{MailEmj} Подача заявления онлайн"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{LabelEmj} Наше местоположение"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{YellowCircleEmj} Изменение ваших данных"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{HeartEmj} Помощь"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{MessageEmj} Связь с автором"),
											}})
		{ ResizeKeyboard = true };



		private readonly ReplyKeyboardMarkup replyKeyboardUserNoSub = new ReplyKeyboardMarkup(new List<KeyboardButton[]>(){
											new KeyboardButton[]
											{
												new KeyboardButton($"{CalendarEmj} Календарь мероприятий"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{TabletEmj} Возможности работы с вашими участиями в мероприятиях"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{NegativeGreenEmj} Отписаться от рассылки")
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{StatusEmj} Проверить ваши статусы и участия в мероприятиях"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{MailEmj} Подача заявления онлайн"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{LabelEmj} Наше местоположение"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{YellowCircleEmj} Изменение ваших данных"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{HeartEmj} Помощь"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{MessageEmj} Связь с автором"),
											}})
		{ ResizeKeyboard = true };


		private readonly ReplyKeyboardMarkup replyKeyboardAdmin = new ReplyKeyboardMarkup(new List<KeyboardButton[]>(){
											new KeyboardButton[]
											{
												new KeyboardButton($"{CalendarEmj} Календарь добавленных мероприятий"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{TabletEmj} Список действий с мероприятиями"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{QuestionEmj} Как изменить права администратора"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{QuestionEmj} Как изменить пароль")
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{TabletEmj} Полный список команд"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{MessageEmj} Связь с автором"),
											}})
		{ ResizeKeyboard = true };



		private readonly InlineKeyboardMarkup inlineAuthorKeyboard = new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>(){
								new InlineKeyboardButton[]
								{
									InlineKeyboardButton.WithUrl($"{GreenCircleEmj} Вконтакте", "https://vk.com/id538682062"),
									InlineKeyboardButton.WithUrl($"{YellowCircleEmj} Telegram группе", "https://t.me/scienceMstucaBotRecall"),
								}});
		#endregion

		public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
		{
			logger.LogInformation("HandleError: {Exception}", exception);
			// Cooldown in case of network connection error
			if (exception is RequestException)
				await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
		}

		public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			//защита от спама
			if (await telegramUserRepo.CheckLastTimeMessageAsync(update.Message.Chat.Id, cancellationToken))
			{
				//обновляем время последнего сообщения
				await telegramUserRepo.UpdateLastTimeMessageTgUserAsync(update.Message.Chat.Id, cancellationToken);
				await (update switch
				{
					{ Message: { } message } => OnMessage(message, cancellationToken),
					_ => UnknownUpdateHandlerAsync(update)
				});
			}
		}

		private async Task OnMessage(Message message, CancellationToken token)
		{
			logger.LogInformation("Receive message type: {MessageType}", message.Type);
			if (message.Text is not { } messageText)
				return;

			var lowerCaseMessage = message.Text.ToLower();

			//получаем данные о пользователе
			var user = await telegramUserRepo.GetTgUserByIdAsync(message.Chat.Id, token);

			//string sentMessage = await (lowerCaseMessage switch
			//{
			//	var msg when msg == "/photo" => SendPhoto(msg),
			//	var msg when msg.Contains("/inline_buttons") => SendInlineKeyboard(msg),
			//	var msg when msg.Contains("/keyboard") => SendReplyKeyboard(msg),
			//	var msg when msg.Contains("/remove") => RemoveKeyboard(msg),
			//	var msg when msg.Contains("/request") => RequestContactAndLocation(msg),
			//	var msg when msg.Contains("/inline_mode") => StartInlineQuery(msg),
			//	var msg when msg.Contains("/poll") => SendPoll(msg),
			//	var msg when msg.Contains("/poll_anonymous") => SendAnonymousPoll(msg),
			//	var msg when msg.Contains("/throw") => FailingHandler(msg),
			//	_ => Usage(msg)
			//});

			//старт и приветствие
			#region [DefaultReactions]
			if (lowerCaseMessage == "/start")
			{
				string WelcomeText1 = _configuration["WelcomeText1"].Replace("{PlaneEmj}", PlaneEmj).Replace("{AlertEmj}", AlertEmj);
				string WelcomeText2 = _configuration["WelcomeText2"].Replace("{AlertEmj}", AlertEmj);

				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText1);
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText2);

				if (user is null)
				{
					string WelcomeText3 = _configuration["WelcomeText3"].Replace("{GreenCircleEmj}", GreenCircleEmj);

					await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3, replyMarkup: replyKeyboardUserSub);
					await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				}
				else
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{GreenCircleEmj} Телеграм бот уже активен.", replyMarkup: replyKeyboardUserNoSub);
					await telegramUserRepo.UpdateSubStatusTgUserAsync(message.Chat.Id, true, token);
				}
			}
			else if (lowerCaseMessage.Contains("/addinfo/"))
			{
				try
				{
					string str = lowerCaseMessage.Replace("/addinfo/", "");
					var lst = str.Split("/");

					string name = Char.ToUpper(lst[0][0]) + lst[0].Substring(1);
					string surname = Char.ToUpper(lst[1][0]) + lst[1].Substring(1);
					string patronymic = Char.ToUpper(lst[2][0]) + lst[2].Substring(1);
					string phone = lst[3];

					// Валидация
					if (!Regex.IsMatch(phone, @"^\+7\d{10}$"))
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, InvalidNumber);
						return;
					}

					var res = await telegramUserRepo.AddTgUserAsync(new TelegramUser(message.Chat.Id, name, surname, patronymic, phone, true, false), token);
					if (!res)
					{
						await botClient.SendTextMessageAsync(message.Chat.Id,
							$"{GreenCircleEmj} Телеграм бот уже активен.");
						return;
					}
					else
					{
						await botClient.SendTextMessageAsync(message.Chat.Id,
							$"{GreenCircleEmj} Данные успешно добавлены.", replyMarkup: replyKeyboardUserNoSub);
					}
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Ошибка. Проверьте правильность ввода.");
				}
			}
			//проверка на логин
			else if (user is null)
			{
				string WelcomeText3 = _configuration["WelcomeText3"].Replace("{GreenCircleEmj}", GreenCircleEmj);

				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
			}
			#endregion

			//реакции на сообщения обычного пользователя
			#region [ReactionsOnUser]
			else if (lowerCaseMessage.Contains("календарь мероприятий"))
			{
				string sendMessage = await scienceEventRepo.ReadAllActualEventsToStringAsync(token);
				if (string.IsNullOrWhiteSpace(sendMessage)) sendMessage = $"{ButtonEmj}  На ближайшее время мероприятий не запланировано.";
				await botClient.SendTextMessageAsync(message.Chat.Id, sendMessage);
			}
			else if (lowerCaseMessage.Contains("возможности работы с вашими участиями в мероприятиях"))
			{
				string UserActionsList = _configuration["UserActionsList"].Replace("{CheckMarkInBlockEmj}", CheckMarkInBlockEmj).Replace("{ButtonEmj}", ButtonEmj).Replace("{NegativeRedEmj}", NegativeRedEmj);
				string UserAddEvent = _configuration["UserAddEvent"].Replace("{RedCircleEmj}", RedCircleEmj);
				string UserChangeEvent = _configuration["UserChangeEvent"].Replace("{YellowCircleEmj}", YellowCircleEmj);
				string UserDeleteEvent = _configuration["UserDeleteEvent"].Replace("{GreenCircleEmj}", GreenCircleEmj);

				await botClient.SendTextMessageAsync(message.Chat.Id, UserActionsList);
				await botClient.SendTextMessageAsync(message.Chat.Id, UserAddEvent);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/adduserevent/Название 1/Москва/01.01.2024/True");

				await botClient.SendTextMessageAsync(message.Chat.Id, UserChangeEvent);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/updateuserevent/Название 1/Москва/01.01.2024/True/11ce84c9-08ba-487d-89ac-97cd166111fc");

				await botClient.SendTextMessageAsync(message.Chat.Id, UserDeleteEvent);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/deleteuserevent/11ce84c9-08ba-487d-89ac-97cd166111fc");
			}
			else if (lowerCaseMessage.Contains("подписаться на рассылку"))
			{
				await telegramUserRepo.UpdateSubStatusTgUserAsync(message.Chat.Id, true, token);
				await botClient.SendTextMessageAsync(message.Chat.Id, $"{CheckMarkEmj}Вы успешно подписались на рассылку!", replyMarkup: replyKeyboardUserNoSub);
			}
			else if (lowerCaseMessage.Contains("отписаться от рассылки"))
			{
				await telegramUserRepo.UpdateSubStatusTgUserAsync(message.Chat.Id, false, token);
				await botClient.SendTextMessageAsync(message.Chat.Id, $"{SleepZzEmj}Вы успешно отписались от рассылки!", replyMarkup: replyKeyboardUserSub);
			}
			else if (lowerCaseMessage.Contains("проверить ваши статусы и участия в мероприятиях"))
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, await tgUserInfoService.GetInfoAboutTgUserAsync(message.Chat.Id, token));
			}
			else if (lowerCaseMessage.Contains("подача заявления онлайн"))
			{
				string SNOInfo = _configuration["SNOInfo"].Replace("{PlaneEmj}", PlaneEmj).Replace("{GreenCircleEmj}", GreenCircleEmj).Replace("{RedCircleEmj}", RedCircleEmj);
				string SMUInfo = _configuration["SMUInfo"].Replace("{BrownCircleEmj}", BrownCircleEmj).Replace("{YellowCircleEmj}", YellowCircleEmj);

				await botClient.SendTextMessageAsync(message.Chat.Id, SNOInfo);
				await botClient.SendTextMessageAsync(message.Chat.Id, $"/snoapp/Иванова Ивана Ивановича/ФАСК/РС-5/+79999999999/example@example.ru");

				await botClient.SendTextMessageAsync(message.Chat.Id, SMUInfo);
				await botClient.SendTextMessageAsync(message.Chat.Id, $"/smuapp/аспиранта/Иванова Ивана Ивановича/ТЭРЭО ВТ/Ваша научная степень/20.01.1994/+79999999999/example@example.ru");
			}
			else if (lowerCaseMessage.Contains("наше местоположение"))
			{
				string Place = _configuration["Place"].Replace("{LabelEmj}", LabelEmj);
				await botClient.SendTextMessageAsync(message.Chat.Id, Place);
			}
			else if (lowerCaseMessage.Contains("связь с автором"))
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, "Вы можете связаться с автором по:", replyMarkup: inlineAuthorKeyboard);
			}
			else if (lowerCaseMessage == "!user")
			{
				if (user.IsSubscribed)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, "Выбрана панель обычного пользователя.", replyMarkup: replyKeyboardUserNoSub);
				}
				else
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, "Выбрана панель обычного пользователя.", replyMarkup: replyKeyboardUserSub);
				}
			}
			else if (lowerCaseMessage.Contains("изменение ваших данных"))
			{
				string ChangeUserName = _configuration["ChangeUserName"].Replace("{RedCircleEmj}", RedCircleEmj);
				string ChangeUserSName = _configuration["ChangeUserSName"].Replace("{YellowCircleEmj}", YellowCircleEmj);
				string ChangeUserPatronymic = _configuration["ChangeUserPatronymic"].Replace("{GreenCircleEmj}", GreenCircleEmj);
				string ChangeUserPhone = _configuration["ChangeUserPhone"].Replace("{BlueCircleEmj}", BlueCircleEmj);

				await botClient.SendTextMessageAsync(message.Chat.Id, ChangeUserName);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/chname/Николай");

				await botClient.SendTextMessageAsync(message.Chat.Id, ChangeUserSName);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/chsname/Иванов");

				await botClient.SendTextMessageAsync(message.Chat.Id, ChangeUserPatronymic);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/chpatr/Иванович");

				await botClient.SendTextMessageAsync(message.Chat.Id, ChangeUserPhone);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/chphone/+79999999999");
			}
			else if (lowerCaseMessage.Contains("помощь"))
			{
				string HelpCommands = _configuration["HelpCommands"].Replace("{TabletEmj}", TabletEmj);
				await botClient.SendTextMessageAsync(message.Chat.Id, HelpCommands);
			}
			else if (lowerCaseMessage.Contains("/adduserevent"))
			{
				try
				{
					if (await userCreatedEventRepo.AddUserCreatedEventAsync(lowerCaseMessage, message.Chat.Id, token))
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{CheckMarkInBlockEmj}Мероприятие успешно добавлено!");
					}
					else
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{NegativeRedEmj}Мероприятие не добавлено! " +
							$"Возможно мероприятие с данным названием сущестует.");
					}
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при добавлении!\n" +
						$"Проверьте правильность ввода данных.");
				}
			}
			else if (lowerCaseMessage.Contains("/updateuserevent"))
			{
				try
				{
					if (await userCreatedEventRepo.UpdateUserCreatedEventAsync(lowerCaseMessage, message.Chat.Id, token))
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{CheckMarkInBlockEmj}Мероприятие успешно изменено!");
					}
					else
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{NegativeRedEmj}Мероприятие не изменено! " +
							$"Проверьте на правильность ввода данных и повтор названия.");
					}
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при изменении!\n" +
						$"Проверьте правильность ввода данных.");
				}
			}
			else if (lowerCaseMessage.Contains("/deleteuserevent"))
			{
				try
				{
					var deletedEvent = await userCreatedEventRepo.DeleteUserCreatedEventByIdAsync(lowerCaseMessage, message.Chat.Id, token);
					if (deletedEvent is not null)
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{CheckMarkInBlockEmj}Мероприятие успешно удалено!\n{deletedEvent.ToString()}");
					}
					else
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{NegativeRedEmj}Мероприятие не удалено! Проверьте правильность ввода данных.");
					}
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при удалении!\n" +
						$"Проверьте правильность введенного номера мероприятия.");
				}
			}
			else if (lowerCaseMessage.Contains("/chname"))
			{
				try
				{
					if (lowerCaseMessage == "/chname")
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{RedCircleEmj} Ошибка! Проверьте правильность введенного имени.");
						return;
					}
					string name = lowerCaseMessage.Replace("/chname/", "");
					name.Replace(" ", "");

					string mess = await telegramUserRepo.UpdateNameTgUserAsync(message.Chat.Id, name, token);
					await botClient.SendTextMessageAsync(message.Chat.Id, mess);
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при изменении имени!\n" +
						$"Пожалуйста, проверьте отправленные вами данные.");
				}
			}
			else if (lowerCaseMessage.Contains("/chsname"))
			{
				try
				{
					if (lowerCaseMessage == "/chsname")
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{RedCircleEmj} Ошибка! Проверьте правильность введенной фамилии.");
						return;
					}
					string sName = lowerCaseMessage.Replace("/chsname/", "");
					sName.Replace(" ", "");

					string mess = await telegramUserRepo.UpdateSNameTgUserAsync(message.Chat.Id, sName, token);
					await botClient.SendTextMessageAsync(message.Chat.Id, mess);
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при изменении фамилии!\n" +
						$"Пожалуйста, проверьте отправленные вами данные.");
				}
			}
			else if (lowerCaseMessage.Contains("/chpatr"))
			{
				try
				{
					if (lowerCaseMessage == "/chpatr")
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{RedCircleEmj} Ошибка! Проверьте правильность введенного отчества.");
						return;
					}
					string patr = lowerCaseMessage.Replace("/chpatr/", "");
					patr.Replace(" ", "");

					string mess = await telegramUserRepo.UpdatePatrTgUserAsync(message.Chat.Id, patr, token);
					await botClient.SendTextMessageAsync(message.Chat.Id, mess);
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при изменении отчества!\n" +
						$"Пожалуйста, проверьте отправленные вами данные.");
				}
			}
			else if (lowerCaseMessage.Contains("/chphone"))
			{
				try
				{
					if (lowerCaseMessage == "/chphone")
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{RedCircleEmj} Ошибка! Проверьте правильность введенного номера телефона.");
						return;
					}
					string phone = lowerCaseMessage.Replace("/chphone/", "");
					phone.Replace(" ", "");

					string mess = await telegramUserRepo.UpdatePhoneTgUserAsync(message.Chat.Id, phone, token);
					await botClient.SendTextMessageAsync(message.Chat.Id, mess);
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при изменении номера телефона!\n" +
						$"Пожалуйста, проверьте отправленные вами данные.");
				}
			}
			else if (lowerCaseMessage.Contains("/snoapp"))
			{
				try
				{
					var messageToSend = await fillDataService.FillSNODataAsync(lowerCaseMessage, message.Chat.Id, fileSNOFullPath, token);
					await botClient.SendTextMessageAsync(message.Chat.Id, messageToSend);
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при создании документа!\n" +
						$"Пожалуйста, проверьте отправленные вами данные.");
				}
			}
			else if (lowerCaseMessage.Contains("/smuapp"))
			{
				try
				{
					var messageToSend = await fillDataService.FillSMUDataAsync(lowerCaseMessage, message.Chat.Id, fileSMUFullPath, token);
					await botClient.SendTextMessageAsync(message.Chat.Id, messageToSend);
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при создании документа!\n" +
						$"Пожалуйста, проверьте отправленные вами данные.");
				}
			}
			#endregion

			//реакции на сообщения администратора
			#region [ReactionsOnAdmin]
			else if (lowerCaseMessage == "!admin")
			{
				if (user.IsAdmin)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, "Выбрана админ-панель.", replyMarkup: replyKeyboardAdmin);
				}
				else
				{
					string NoRules = _configuration["NoRules"].Replace("{AlertEmj}", AlertEmj);
					await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
				}
			}
			else if (lowerCaseMessage.Contains("календарь добавленных мероприятий"))
			{
				if (user.IsAdmin)
				{
					string sendMessage = await scienceEventRepo.ReadAllActualEvAdminToStringAsync(token);
					if (string.IsNullOrWhiteSpace(sendMessage)) sendMessage = $"{ButtonEmj}  На ближайшее время мероприятий не запланировано.";
					await botClient.SendTextMessageAsync(message.Chat.Id, sendMessage);
				}
				else
				{
					string NoRules = _configuration["NoRules"].Replace("{AlertEmj}", AlertEmj);
					await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
				}
			}
			else if (lowerCaseMessage.Contains("список действий с мероприятиями"))
			{
				if (user.IsAdmin)
				{
					string AdminEventsActionsList = _configuration["AdminEventsActionsList"].Replace("{TabletEmj}", TabletEmj).Replace("{GreenCircleEmj}", GreenCircleEmj);
					string ChangeSEvent = _configuration["ChangeSEvent"].Replace("{YellowCircleEmj}", YellowCircleEmj);
					string DeleteSEvent = _configuration["DeleteSEvent"].Replace("{RedCircleEmj}", RedCircleEmj);

					await botClient.SendTextMessageAsync(message.Chat.Id, AdminEventsActionsList);
					await botClient.SendTextMessageAsync(message.Chat.Id, $"/addevent/Событие 1/01.01.2024/Москва/нет/Хорошее мероприятие");

					await botClient.SendTextMessageAsync(message.Chat.Id, ChangeSEvent);
					await botClient.SendTextMessageAsync(message.Chat.Id, $"/chevent/1e3eca14-90b2-459c-8471-58c9c9cc4462/Событие 1/01.01.2024/Москва/нет/Хорошее мероприятие");

					await botClient.SendTextMessageAsync(message.Chat.Id, DeleteSEvent);
					await botClient.SendTextMessageAsync(message.Chat.Id, $"/deleteevent/1e3eca14-90b2-459c-8471-58c9c9cc4462");
				}
				else
				{
					string NoRules = _configuration["NoRules"].Replace("{AlertEmj}", AlertEmj);
					await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
				}
			}
			else if (lowerCaseMessage.Contains("как изменить права администратора"))
			{
				if (user.IsAdmin)
				{
					string ChangeRights = _configuration["ChangeRights"].Replace("{RedCircleEmj}", RedCircleEmj).Replace("{GreenCircleEmj}", GreenCircleEmj).Replace("{BlueCircleEmj}", BlueCircleEmj);

					await botClient.SendTextMessageAsync(message.Chat.Id, ChangeRights);
					await botClient.SendTextMessageAsync(message.Chat.Id, "/adminchadm/1");
				}
				else
				{
					string NoRules = _configuration["NoRules"].Replace("{AlertEmj}", AlertEmj);
					await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
				}
			}
			else if (lowerCaseMessage.Contains("как изменить пароль"))
			{
				if (user.IsAdmin)
				{
					string ChangePassword = _configuration["ChangePassword"].Replace("{RedCircleEmj}", RedCircleEmj).Replace("{GreenCircleEmj}", GreenCircleEmj);

					await botClient.SendTextMessageAsync(message.Chat.Id, ChangePassword);
					await botClient.SendTextMessageAsync(message.Chat.Id, "/adminchpass/newpassword");
				}
				else
				{
					string NoRules = _configuration["NoRules"].Replace("{AlertEmj}", AlertEmj);
					await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
				}
			}
			else if (lowerCaseMessage.Contains("полный список команд"))
			{
				if (user.IsAdmin)
				{
					string AdminActionsList = _configuration["AdminActionsList"].Replace("{TabletEmj}", TabletEmj);

					await botClient.SendTextMessageAsync(message.Chat.Id, AdminActionsList);
				}
				else
				{
					string NoRules = _configuration["NoRules"].Replace("{AlertEmj}", AlertEmj);
					await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
				}
			}
			else if (lowerCaseMessage.Contains("/adminchpass"))
			{
				if (user.IsAdmin)
				{
					try
					{
						string password = lowerCaseMessage.Replace("/adminchpass", "").Replace("/", "").Replace(" ", "");
						if(String.IsNullOrWhiteSpace(password))
						{
							await botClient.SendTextMessageAsync(message.Chat.Id, $"{RedCircleEmj} Ошибка при изменении пароля!");
							return;
						}
						
						string hashedPassword = passwordHasherService.HashPassword(password);
						bool res = await telegramUserRepo.UpdatePasswordTgUserAsync(hashedPassword, message.Chat.Id, token);
						if (res) await botClient.SendTextMessageAsync(message.Chat.Id, $"{CheckMarkEmj} Пароль успешно изменен!");
						else await botClient.SendTextMessageAsync(message.Chat.Id, $"{RedCircleEmj} Ошибка при изменении пароля!");
					}
					catch (Exception)
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при изменении пароля. Проверьте правильность ввода.");
					}
				}
				else
				{
					string NoRules = _configuration["NoRules"].Replace("{AlertEmj}", AlertEmj);
					await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
				}
			}
			else if (lowerCaseMessage.Contains("/adminchadm"))
			{
				if (user.IsAdmin)
				{
					try
					{
						if (await telegramUserRepo.UpdateAdminStatusTgUserAsync(lowerCaseMessage, message.Chat.Id, token))
						{
							await botClient.SendTextMessageAsync(message.Chat.Id, $"{CheckMarkInBlockEmj} Права администратора успешно изменены!");
						}
						else
						{
							await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj} Права администратора изменить не удалось.\nВозможно у вас отсутствуют права администратора, либо отсутсвует такой идентификатор чата, либо вы пытаетесь изменить свои права.");
						}
					}
					catch (Exception)
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj} Возникла ошибка при изменении прав. Проверьте правильность ввода.");
					}
				}
				else
				{
					string NoRules = _configuration["NoRules"].Replace("{AlertEmj}", AlertEmj);
					await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
				}
			}
			else if (lowerCaseMessage.Contains("/addevent"))
			{
				if (user.IsAdmin)
				{
					try
					{
						string messageToSend = lowerCaseMessage + "/" + message.Chat.Id;
						var result = await scienceEventRepo.AddEventAsync(messageToSend, token);
						if (!result)
						{
							await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj} Мероприятие с таким названием уже существует!");
							return;
						}
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{CheckMarkEmj} Мероприятие успешно добавлено!");
						await notifyService.NotifyLastAddEventUsersAsync($"{GreenCircleEmj} Новое мероприятие!", token);
					}
					catch (Exception)
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj} Возникла ошибка при добавлении мероприятия. Пожалуйста, проверьте правильность ввода мероприятия.");
					}
				}
				else
				{
					string NoRules = _configuration["NoRules"].Replace("{AlertEmj}", AlertEmj);
					await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
				}
			}
			else if (lowerCaseMessage.Contains("/chevent"))
			{
				if (user.IsAdmin)
				{
					try
					{
						string messageToSend = lowerCaseMessage + "/" + message.Chat.Id;
						var updEvent = await scienceEventRepo.UpdateFullEventAsync(messageToSend, token);
						if (updEvent == null)
						{
							await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj} Номера такого мероприятия не существует, либо название мероприятия повторяется, либо неверно указана дата мероприятия.");
							return;
						}
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{CheckMarkEmj} Мероприятие успешно изменено!");
						await notifyService.NotifyEventChangingUsersAsync(updEvent, $"{RedCircleEmj} Внимание! Изменения!", token);
					}
					catch (Exception)
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj} Возникла ошибка при изменении мероприятия. Пожалуйста, проверьте правильность ввода мероприятия.");
					}
				}
				else
				{
					string NoRules = _configuration["NoRules"].Replace("{AlertEmj}", AlertEmj);
					await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
				}
			}
			else if (lowerCaseMessage.Contains("/deleteevent"))
			{
				if (user.IsAdmin)
				{
					try
					{
						var res = await scienceEventRepo.DeleteEventByIdAsync(lowerCaseMessage, token);
						if (res == null)
						{
							await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj} Такого мероприятия не существует!");
						}
						else
						{
							await botClient.SendTextMessageAsync(message.Chat.Id, $"{CheckMarkEmj} Мероприятие успешно отменено!");
							await notifyService.NotifyEventChangingUsersAsync(res, $"{NegativeRedEmj} Мероприятие отменено!", token);
						}
					}
					catch (Exception)
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj} Возникла ошибка при удалении мероприятия. Пожалуйста, проверьте правильность введенного id.");
					}
				}
				else
				{
					string NoRules = _configuration["NoRules"].Replace("{AlertEmj}", AlertEmj);
					await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
				}
			}
			#endregion
		}

		private Task UnknownUpdateHandlerAsync(Update update)
		{
			logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
			return Task.CompletedTask;
		}
	}
}
