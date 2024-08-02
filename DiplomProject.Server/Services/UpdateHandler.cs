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
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Microsoft.OpenApi.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Domain.Enums;

using static Domain.Helpers.EnumExtensions;
using static Domain.Constants.TelegramTextConstants;
using static Domain.Constants.EmojiConstants;



namespace DiplomProject.Server.Services
{
	public class UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger, ITelegramUserRepository telegramUserRepo,
		IScienceEventRepository scienceEventRepo, INotifyService notifyService, IUserCreatedEventRepository userCreatedEventRepo,
		IFillDataService fillDataService, IPasswordHasherService passwordHasherService, ITgUserService tgUserInfoService, IConfiguration _configuration) : IUpdateHandler
	{
		private TelegramUser? user = null;
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
			user = await telegramUserRepo.GetTgUserByIdAsync(message.Chat.Id, token);

			switch (GetBotCommandFromText(lowerCaseMessage))
			{
				case BotEnumCommands.Start:
					await Welcome(message, lowerCaseMessage, token);
					break;
				case BotEnumCommands.AddInfo:
					await AddInfo(message, lowerCaseMessage, token);
					break;
				case BotEnumCommands.CalendarEvents:
					await GetCalendarEvents(message, token);
					break;
				case BotEnumCommands.UserEventsActions:
					await GetUserEventsActions(message, token);
					break;
				case BotEnumCommands.Subscribe:
					await Subscribe(message, token);
					break;
				case BotEnumCommands.Unsubscribe:
					await Unsubscribe(message, token);
					break;
				case BotEnumCommands.UserStatus:
					await GetUserStatus(message, token);
					break;
				case BotEnumCommands.Statements:
					await StatementsInfo(message, token);
					break;
				case BotEnumCommands.Place:
					await GetPlaceInfo(message, token);
					break;
				case BotEnumCommands.AuthorContacts:
					await GetAuthorContacts(message, token);
					break;
				case BotEnumCommands.UserPanel:
					await SetUserPanel(message, token);
					break;
				case BotEnumCommands.ChangeUserDataInfo:
					await ChangeUserData(message, token);
					break;
				case BotEnumCommands.HelpUser:
					await HelpUserCommands(message, token);
					break;
				case BotEnumCommands.AddUserEvent:
					await AddUserEvent(message, lowerCaseMessage, token);
					break;
				case BotEnumCommands.UpdateUserEvent:
					await UpdateUserEvent(message, lowerCaseMessage, token);
					break;
				case BotEnumCommands.DeleteUserEvent:
					await DeleteUserEvent(message, lowerCaseMessage, token);
					break;
				case BotEnumCommands.ChangeUserName:
					await ChangeUserNameAction(message, lowerCaseMessage, token);
					break;
				case BotEnumCommands.ChangeUserSName:
					await ChangeUserSNameAction(message, lowerCaseMessage, token);
					break;
				case BotEnumCommands.ChangeUserPatronymic:
					await ChangeUserPatronymicAction(message, lowerCaseMessage, token);
					break;
				case BotEnumCommands.ChangeUserPhoneNumber:
					await ChangeUserPhoneNumberAction(message, lowerCaseMessage, token);
					break;
				case BotEnumCommands.FillSNOData:
					await FillSNOData(message, lowerCaseMessage, token);
					break;
				case BotEnumCommands.FillSMUData:
					await FillSMUData(message, lowerCaseMessage, token);
					break;
				case BotEnumCommands.ChangeAdminPanel:
					await ChangeAdminPanel(message, token);
					break;
				case BotEnumCommands.CalendarAdminEvents:
					await GetCalendarAdminEvents(message, token);
					break;
				case BotEnumCommands.AdminEventsActions:
					await GetAdminEventsActions(message, token);
					break;
				case BotEnumCommands.ChangeAdminRightsInfo:
					await ChangeAdminRightsInfo(message, token);
					break;
				case BotEnumCommands.ChangeAdminPasswordInfo:
					await ChangeAdminPasswordInfo(message, token);
					break;
				case BotEnumCommands.HelpAdmin:
					await HelpAdminCommands(message, token);
					break;
				case BotEnumCommands.ChangeAdminPassword:
					await ChangeAdminPassword(message, lowerCaseMessage, token);
					break;
				case BotEnumCommands.GiveAdminRights:
					await GiveAdminRights(message, lowerCaseMessage, token);
					break;
			case BotEnumCommands.AddAdminEvent:
					await AddAdminEvent(message, lowerCaseMessage, token);
					break;
			case BotEnumCommands.ChangeAdminEvent:
					await ChangeAdminEvent(message, lowerCaseMessage, token);
					break;
			case BotEnumCommands.DeleteAdminEvent:
					await DeleteAdminEvent(message, lowerCaseMessage, token);
					break;
			};
		}

		private async Task DeleteAdminEvent(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
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
				await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
			}
		}

		private async Task ChangeAdminEvent(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
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
				await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
			}
		}

		private async Task AddAdminEvent(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
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
				await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
			}
		}

		private async Task GiveAdminRights(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			if (user.IsAdmin)
			{
				try
				{
					var res = await telegramUserRepo.UpdateAdminStatusTgUserAsync(lowerCaseMessage, message.Chat.Id, token);
					await botClient.SendTextMessageAsync(message.Chat.Id, res ? $"{CheckMarkInBlockEmj} Права администратора успешно изменены!" : $"{AlertEmj} Права администратора изменить не удалось.\nВозможно у вас отсутствуют права администратора, либо отсутсвует такой идентификатор чата, либо вы пытаетесь изменить свои права.");
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj} Возникла ошибка при изменении прав. Проверьте правильность ввода.");
				}
			}
			else
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
			}
		}

		private async Task ChangeAdminPassword(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			if (user.IsAdmin)
			{
				try
				{
					string password = lowerCaseMessage.Replace("/adminchpass", "").Replace("/", "").Replace(" ", "");
					if (String.IsNullOrWhiteSpace(password))
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
				await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
			}
		}

		private async Task HelpAdminCommands(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			if (user.IsAdmin)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, AdminActionsList);
			}
			else
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
			}
		}

		private async Task ChangeAdminPasswordInfo(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			if (user.IsAdmin)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, ChangePassword);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/adminchpass/newpassword");
			}
			else
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
			}
		}

		private async Task ChangeAdminRightsInfo(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			if (user.IsAdmin)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, ChangeRights);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/adminchadm/1");
			}
			else
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
			}
		}

		private async Task GetAdminEventsActions(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			if (user.IsAdmin)
			{
				//massive
				await botClient.SendTextMessageAsync(message.Chat.Id, AdminEventsActionsList);
				await botClient.SendTextMessageAsync(message.Chat.Id, $"/addevent/Событие 1/01.01.2024/Москва/нет/Хорошее мероприятие");

				await botClient.SendTextMessageAsync(message.Chat.Id, ChangeSEvent);
				await botClient.SendTextMessageAsync(message.Chat.Id, $"/chevent/1e3eca14-90b2-459c-8471-58c9c9cc4462/Событие 1/01.01.2024/Москва/нет/Хорошее мероприятие");

				await botClient.SendTextMessageAsync(message.Chat.Id, DeleteSEvent);
				await botClient.SendTextMessageAsync(message.Chat.Id, $"/deleteevent/1e3eca14-90b2-459c-8471-58c9c9cc4462");
			}
			else
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
			}
		}

		private async Task GetCalendarAdminEvents(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			if (user.IsAdmin)
			{
				string sendMessage = await scienceEventRepo.ReadAllActualEvAdminToStringAsync(token);
				if (string.IsNullOrWhiteSpace(sendMessage)) sendMessage = $"{ButtonEmj}  На ближайшее время мероприятий не запланировано.";
				await botClient.SendTextMessageAsync(message.Chat.Id, sendMessage);
			}
			else
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
			}
		}

		private async Task ChangeAdminPanel(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			if (user.IsAdmin)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, "Выбрана админ-панель.", replyMarkup: replyKeyboardAdmin);
			}
			else
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, NoRules);
			}
		}

		private async Task FillSMUData(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
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

		private async Task FillSNOData(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
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

		private async Task ChangeUserPhoneNumberAction(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
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

		private async Task ChangeUserPatronymicAction(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
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

		private async Task ChangeUserSNameAction(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
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

		private async Task ChangeUserNameAction(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
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

		private async Task DeleteUserEvent(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
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

		private async Task UpdateUserEvent(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
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

		private async Task AddUserEvent(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
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

		private async Task HelpUserCommands(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			await botClient.SendTextMessageAsync(message.Chat.Id, HelpCommands);
		}

		private async Task ChangeUserData(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			string[] messages = { ChangeUserName, "/chname/Николай", ChangeUserSName, "/chsname/Иванов", ChangeUserPatronymic, "/chpatr/Иванович", ChangeUserPhone, "/chphone/+79999999999" };
			foreach (var mes in messages)
				await botClient.SendTextMessageAsync(message.Chat.Id, mes);
		}

		private async Task SetUserPanel(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			if (user.IsSubscribed)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, "Выбрана панель обычного пользователя.", replyMarkup: replyKeyboardUserNoSub);
			}
			else
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, "Выбрана панель обычного пользователя.", replyMarkup: replyKeyboardUserSub);
			}
		}

		private async Task GetAuthorContacts(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			await botClient.SendTextMessageAsync(message.Chat.Id, "Вы можете связаться с автором по:", replyMarkup: inlineAuthorKeyboard);
		}

		private async Task GetPlaceInfo(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			string Place = _configuration["Place"].Replace("{LabelEmj}", LabelEmj);
			await botClient.SendTextMessageAsync(message.Chat.Id, Place);
		}

		private async Task StatementsInfo(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			await botClient.SendTextMessageAsync(message.Chat.Id, SNOInfo);
			await botClient.SendTextMessageAsync(message.Chat.Id, $"/snoapp/Иванова Ивана Ивановича/ФАСК/РС-5/+79999999999/example@example.ru");

			await botClient.SendTextMessageAsync(message.Chat.Id, SMUInfo);
			await botClient.SendTextMessageAsync(message.Chat.Id, $"/smuapp/аспиранта/Иванова Ивана Ивановича/ТЭРЭО ВТ/Ваша научная степень/20.01.1994/+79999999999/example@example.ru");
		}

		private async Task GetUserStatus(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			await botClient.SendTextMessageAsync(message.Chat.Id, await tgUserInfoService.GetInfoAboutTgUserAsync(message.Chat.Id, token));
		}

		private async Task Unsubscribe(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			await telegramUserRepo.UpdateSubStatusTgUserAsync(message.Chat.Id, false, token);
			await botClient.SendTextMessageAsync(message.Chat.Id, $"{SleepZzEmj}Вы успешно отписались от рассылки!", replyMarkup: replyKeyboardUserSub);
		}

		private async Task Subscribe(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			await telegramUserRepo.UpdateSubStatusTgUserAsync(message.Chat.Id, true, token);
			await botClient.SendTextMessageAsync(message.Chat.Id, $"{CheckMarkEmj}Вы успешно подписались на рассылку!", replyMarkup: replyKeyboardUserNoSub);
		}

		private async Task GetUserEventsActions(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			await botClient.SendTextMessageAsync(message.Chat.Id, UserActionsList);
			await botClient.SendTextMessageAsync(message.Chat.Id, UserAddEvent);
			await botClient.SendTextMessageAsync(message.Chat.Id, "/adduserevent/Название 1/Москва/01.01.2024/True");

			await botClient.SendTextMessageAsync(message.Chat.Id, UserChangeEvent);
			await botClient.SendTextMessageAsync(message.Chat.Id, "/updateuserevent/Название 1/Москва/01.01.2024/True/11ce84c9-08ba-487d-89ac-97cd166111fc");

			await botClient.SendTextMessageAsync(message.Chat.Id, UserDeleteEvent);
			await botClient.SendTextMessageAsync(message.Chat.Id, "/deleteuserevent/11ce84c9-08ba-487d-89ac-97cd166111fc");
		}

		private async Task GetCalendarEvents(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
				return;
			}
			string sendMessage = await scienceEventRepo.ReadAllActualEventsToStringAsync(token);
			if (string.IsNullOrWhiteSpace(sendMessage)) sendMessage = $"{ButtonEmj}  На ближайшее время мероприятий не запланировано.";
			await botClient.SendTextMessageAsync(message.Chat.Id, sendMessage);
		}

		private async Task AddInfo(Message message, string lowerCaseMessage, CancellationToken token)
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

		private async Task Welcome(Message message, string messageText, CancellationToken token)
		{
			string WelcomeText1 = _configuration["WelcomeText1"].Replace("{PlaneEmj}", PlaneEmj).Replace("{AlertEmj}", AlertEmj);
			string WelcomeText2 = _configuration["WelcomeText2"].Replace("{AlertEmj}", AlertEmj);

			await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText1);
			await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText2);

			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3, replyMarkup: replyKeyboardUserSub);
				await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Иванов/Иванович/+79999999999");
			}
			else
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, $"{GreenCircleEmj} Телеграм бот уже активен.", replyMarkup: replyKeyboardUserNoSub);
				await telegramUserRepo.UpdateSubStatusTgUserAsync(message.Chat.Id, true, token);
			}
		}

		private Task UnknownUpdateHandlerAsync(Update update)
		{
			logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
			return Task.CompletedTask;
		}
	}
}
