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
using MSTUCABOT.ConsoleServer.Services.Classes;
using System.Threading;
using static Domain.Constants.EmojiConstants;


namespace DiplomProject.Server.Services
{
	public class UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger, ITelegramUserRepository telegramUserRepo, 
		IScienceEventRepository scienceEventRepo, INotifyService notifyService, IUserCreatedEventRepository userCreatedEventRepo) : IUpdateHandler
	{
		//private static SemaphoreSlim semaphore = new SemaphoreSlim(1); //для изменения документов
		#region [Paths]
		private static readonly string fileSNOFullPath = Environment.CurrentDirectory + "\\..\\..\\..\\Documents\\SNO.docx";
		private static readonly string fileSMUFullPath = Environment.CurrentDirectory + "\\..\\..\\..\\Documents\\SMU.docx";
		#endregion
		#region [TgButtons]
		//buttons
		private static ReplyKeyboardMarkup replyKeyboardUserSub = new ReplyKeyboardMarkup(new List<KeyboardButton[]>(){
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



		private static ReplyKeyboardMarkup replyKeyboardUserNoSub = new ReplyKeyboardMarkup(new List<KeyboardButton[]>(){
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


		private static ReplyKeyboardMarkup replyKeyboardAdmin = new ReplyKeyboardMarkup(new List<KeyboardButton[]>(){
											new KeyboardButton[]
											{
												new KeyboardButton($"{GreenCircleEmj} Просмотреть номера ваших паролей"),
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
												new KeyboardButton($"{QuestionEmj} Как добавить пароль"),
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{QuestionEmj} Как изменить пароль")
											},
											new KeyboardButton[]
											{
												new KeyboardButton($"{QuestionEmj} Как удалить пароль"),
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



		private static InlineKeyboardMarkup inlineAuthorKeyboard = new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>(){
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

			//старт и приветствие
			#region [DefaultReactions]
			if (lowerCaseMessage == "/start")
			{
				await botClient.SendTextMessageAsync(message.Chat.Id,
					$"Добро пожаловать в бот науки МГТУ ГА! {PlaneEmj}\n" +
					"Пожалуйста, выберите предложенные действия:\n\n" +
					$"Внимание{AlertEmj} В боте установлена защита от спама.\n" +
					$"Время задержки между сообщениями 3 секунды.", replyMarkup: replyKeyboardUserNoSub);
				await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj} В соответствии с требованиями статьи 9 Федерального закона от 27.07.2006 № 152-ФЗ «О персональных данных»," +
					" используя телеграм бот вы даете согласие членам Совета СНО МГТУ ГА на автоматизированную, " +
					"а также без использования средств автоматизации, обработку моих персональных данных, " +
					"включающих фамилию, имя, отчество, дату рождения, должность, сведения о месте работы, месте учебы, " +
					"адрес электронной почты, номер контактного телефона.\r\n" +
					"Так же вы предоставляете свое согласие членам Совета СНО МГТУ ГА на совершение действий (операций) с вашими персональными данными, " +
					"включая сбор, систематизацию, накопление, хранение, обновление, изменение, " +
					"использование, обезличивание, блокирование, уничтожение.\r\n");

				var tgUser = await telegramUserRepo.GetTgUserByIdAsync(message.Chat.Id, token);
				if (tgUser is null)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{GreenCircleEmj}Для продолжения использования телеграмм бота, заполните ваши данные в следующем формате:\n" +
					$"/addinfo/Ваше имя/Ваша фамилия/Ваше отчество/номер телефона (формат +7)\nПример:");
					await botClient.SendTextMessageAsync(message.Chat.Id, "/addinfo/Иван/Яшин/Иванович/+79999999999");
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

					//Валидация
					if (!phone.Contains("+7"))
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Неверно указанный номер!\nНомер должен начинаться с +7");
						return;
					}
					if (phone.Length != 12)
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Неверно указан номер!\nНомер должен состоять из 11 цифр");
						return;
					}

					var res = await telegramUserRepo.AddTgUserAsync(new TelegramUser(message.Chat.Id, name, surname, patronymic, phone, false, false), token);
					if (!res)
					{
						await botClient.SendTextMessageAsync(message.Chat.Id,
							$"{GreenCircleEmj} Телеграм бот уже активен.");
						return;
					}
					else
					{
						await botClient.SendTextMessageAsync(message.Chat.Id,
							$"{GreenCircleEmj} Данные успешно добавлены.", replyMarkup: replyKeyboardUserSub);
					}
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Ошибка. Проверьте правильность ввода.");
				}
			}
			else if (lowerCaseMessage == "привет")
			{
				await botClient.SendTextMessageAsync(message.Chat.Id,
					$"Добро пожаловать! {PlaneEmj}\n" +
					"Пожалуйста, выберите предложенные действия:");
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
				await botClient.SendTextMessageAsync(message.Chat.Id, "Данный бот позволяет выполнить следующие функции:\n" +
					$"{CheckMarkInBlockEmj} Добавить участие в мероприятии\n" +
					$"{ButtonEmj} Изменить участие в мероприятии\n" +
					$"{NegativeRedEmj} Удалить участие в мероприятии");

				await botClient.SendTextMessageAsync(message.Chat.Id, "Для добавления участия в мероприятии придерживайтесь следующей конструкции:\n" +
					"/adduserevent/Название мероприятия/Место проведения/Дата проведения/Статус призера (указываете true или false)\n" +
					$"{RedCircleEmj}Пример:");

				await botClient.SendTextMessageAsync(message.Chat.Id, "/adduserevent/Название 1/Москва/01.01.2024 12:12:12/True");

				await botClient.SendTextMessageAsync(message.Chat.Id, "Для изменения участия в мероприятии придерживайтесь следующей конструкции:\n" +
					"/updateuserevent/Название мероприятия/Место проведения/Дата проведения/Статус призера (указываете true или false)/Номер мероприятия\n" +
					$"{YellowCircleEmj}Пример:");

				await botClient.SendTextMessageAsync(message.Chat.Id, "/updateuserevent/Название 1/Москва/01.01.2024 12:12:12/True/1");

				await botClient.SendTextMessageAsync(message.Chat.Id, "Для удаления участия в мероприятии придерживайтесь следующей конструкции:\n" +
					"/deleteuserevent/Номер мероприятия\n" +
					$"{GreenCircleEmj}Пример:");

				await botClient.SendTextMessageAsync(message.Chat.Id, "/deleteuserevent/1");
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
				await botClient.SendTextMessageAsync(message.Chat.Id, await notifyService.GetInfoAboutTgUserAsync(message.Chat.Id, token));
			}
			else if (lowerCaseMessage.Contains("подача заявления онлайн"))
			{
				await botClient.SendTextMessageAsync(message.Chat.Id,
					$"{PlaneEmj} Для подачи заявления на вступление в СНО или СМУ воспользуйтесь следующими командами\n\n" +
					$"{GreenCircleEmj} Вступление в СНО:\n" +
					$"Формат вступления: /snoapp/Фамилия Имя Отчество(в родительном падеже)/Факультет/Группа/Номер телефона/Почта/День/Месяц/Год\n{RedCircleEmj} Пример:");

				await botClient.SendTextMessageAsync(message.Chat.Id, $"/snoapp/Иванова Ивана Ивановича/ФАСК/РС-5/+79999999999/Mail/12/02/2024");

				await botClient.SendTextMessageAsync(message.Chat.Id, $"{BrownCircleEmj} Вступление в СМУ:\n" +
					$"Формат вступления: /smuapp/Фамилия Имя Отчество(в родительном падеже)/Структурное подразделение(наименование кафедры или иного подразделения)/Наличие ученой степени/Дата рождения/Номер телефона/Почта/День/Месяц/Год\n{YellowCircleEmj} Пример:");

				await botClient.SendTextMessageAsync(message.Chat.Id, $"/smuapp/Иванова Ивана Ивановича/ТЭРЭО ВТ/Ваша научная степень/20.01.1994/+79999999999/Mail/12/02/2024");
			}
			else if (lowerCaseMessage.Contains("наше местоположение"))
			{
				await botClient.SendTextMessageAsync(message.Chat.Id,
					$"{LabelEmj}СМУ и СНО расположен в старом корпусе по адресу:\n" +
					"      г.Москва, ул.Пулковская, д.6.\n" +
					"      кабинет: 3-108");
			}
			else if (lowerCaseMessage.Contains("связь с автором"))
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, "Вы можете связаться с автором по:", replyMarkup: inlineAuthorKeyboard);
			}
			else if (lowerCaseMessage == "!user")
			{
				var user = await telegramUserRepo.GetTgUserByIdAsync(message.Chat.Id, token);
				if (user == null)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id,
						$"{AlertEmj}Возникла ошибка.\n" +
						"Пожалуйста, введите команду: /start");
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
			else if (lowerCaseMessage.Contains("изменение ваших данных"))
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, $"{RedCircleEmj}Для изменения имени введите команду:\n" +
					$"/chname/Ваше имя\nПример:");
				await botClient.SendTextMessageAsync(message.Chat.Id, "/chname/Николай");

				await botClient.SendTextMessageAsync(message.Chat.Id, $"{YellowCircleEmj}Для изменения фамилии введите команду:\n" +
					$"/chsname/Ваша фамилия\nПример:");
				await botClient.SendTextMessageAsync(message.Chat.Id, "/chsname/Яшин");

				await botClient.SendTextMessageAsync(message.Chat.Id, $"{GreenCircleEmj}Для изменения отчества введите команду:\n" +
					$"/chpatr/Ваше отчество\nПример:");
				await botClient.SendTextMessageAsync(message.Chat.Id, "/chpatr/Иванович");

				await botClient.SendTextMessageAsync(message.Chat.Id, $"{BlueCircleEmj}Для изменения номера телефона введите команду:\n" +
					$"/chphone/Ваш номер телефона\nПример:");
				await botClient.SendTextMessageAsync(message.Chat.Id, "/chphone/+79999999999");
			}
			else if (lowerCaseMessage.Contains("помощь"))
			{
				await botClient.SendTextMessageAsync(message.Chat.Id,
					$"{TabletEmj} Список команд пользователя:\n\n" +
					"/start - перезапустить бота\n" +
					"/chname - изменить имя\n" +
					"/chsname - изменить фамилию\n" +
					"/chpatr - изменить отчество\n" +
					"/chphone - изменить номер телефона (формат: +7)\n" +
					"/adduserevent - добавить участие в мероприятии\n" +
					"/updateuserevent - изменить ранее добавленное мероприятие\n" +
					"/deleteuserevent - удаление добавленного мероприятия\n" +
					"/status - просмотреть ваш статус и мероприятия\n" +
					"/snoapp - подать заявление на вступление в СНО\n" +
					"/smuapp - подать заявление на вступление в СМУ");
			}
			else if (lowerCaseMessage.Contains("/status"))
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, await notifyService.GetInfoAboutTgUserAsync(message.Chat.Id, token));
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
			//else if (lowerCaseMessage.Contains("/snoapp"))
			//{
			//	try
			//	{
			//		IFillDataService fillDataService = new FillDocxDataService(fileSNOFullPath, notifyService, semaphore);
			//		var messageToSend = await fillDataService.FillSNODataAsync(lowerCaseMessage, message.Chat.Id);
			//		await botClient.SendTextMessageAsync(message.Chat.Id, messageToSend);
			//	}
			//	catch (Exception)
			//	{
			//		await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при создании документа!\n" +
			//			$"Пожалуйста, проверьте отправленные вами данные.");
			//	}
			//}
			//else if (lowerCaseMessage.Contains("/smuapp"))
			//{
			//	try
			//	{
			//		IFillDataService fillDataService = new FillDocxDataService(fileSMUFullPath, notifyService, semaphore);
			//		var messageToSend = await fillDataService.FillSMUDataAsync(lowerCaseMessage, message.Chat.Id);
			//		await botClient.SendTextMessageAsync(message.Chat.Id, messageToSend);
			//	}
			//	catch (Exception)
			//	{
			//		await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при создании документа!\n" +
			//			$"Пожалуйста, проверьте отправленные вами данные.");
			//	}
			//}
			#endregion

			//реакции на сообщения администратора
			#region [ReactionsOnAdmin]
			else if (lowerCaseMessage == "!admin")
			{
				var user = await telegramUserRepo.GetTgUserByIdAsync(message.Chat.Id, token);
				if (user == null)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id,
						$"{AlertEmj}Возникла ошибка.\n" +
						"Пожалуйста, введите команду: /start");
					return;
				}
				if (user.IsAdmin)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, "Выбрана админ-панель.", replyMarkup: replyKeyboardAdmin);
				}
				else
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}У вас нет прав администратора.");
				}
			}
			//else if (lowerCaseMessage.Contains("просмотреть номера ваших паролей"))
			//{
			//	string passwords = await telegramUserRepo.GetPasswordsByChatIdAsync(message.Chat.Id);
			//	if (string.IsNullOrWhiteSpace(passwords)) passwords = "Список паролей пуст.";
			//	await botClient.SendTextMessageAsync(message.Chat.Id, passwords);
			//}
			else if (lowerCaseMessage.Contains("список действий с мероприятиями"))
			{
				var user = await telegramUserRepo.GetTgUserByIdAsync(message.Chat.Id, token);
				if (user == null)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id,
						$"{AlertEmj}Возникла ошибка.\n" +
						"Пожалуйста, введите команду: /start");
					return;
				}
				if (user.IsAdmin)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id,
						$"{TabletEmj}Форматы действий с мероприятиями:\n\n\n" +
						$"	1) Формат добавления мероприятия:\n/addevent/Название события/Дата события/Место проведения/Требования к участнику/Дополнительная информация\n{YellowCircleEmj}Пример:\n");
					await botClient.SendTextMessageAsync(message.Chat.Id, $"/addevent/Событие 1/01.01.2024 12:12:12/Москва/нет/Хорошее мероприятие");

					await botClient.SendTextMessageAsync(message.Chat.Id, $"	2) Формат изменения мероприятия:\n/chevent/Номер мероприятия/Название события/Дата события/Место проведения/Требования к участнику/Дополнительная информация\n{GreenCircleEmj}Пример:");

					await botClient.SendTextMessageAsync(message.Chat.Id, $"/chevent/1/Событие 1/01.01.2024 12:12:12/Москва/нет/Хорошее мероприятие");

					await botClient.SendTextMessageAsync(message.Chat.Id, $"	3) Формат удаления мероприятия:\n/deleteevent/номер события\n{RedCircleEmj}Пример:");

					await botClient.SendTextMessageAsync(message.Chat.Id, $"/deleteevent 1");
				}
				else
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}У вас нет прав администратора.");
				}
			}
			else if (lowerCaseMessage.Contains("как изменить права администратора"))
			{
				await botClient.SendTextMessageAsync(message.Chat.Id,
					"Для изменения прав администратора нужно:\n\n" +
					$"{RedCircleEmj} Иметь права администратора\n" +
					$"{GreenCircleEmj} Получить у пользователя номер чата (вызвать команду просмотра статусов !user)\n" +
					$"{BlueCircleEmj} Вызвать команду /adminchadm далее ввести номер чата\n{BrownCircleEmj}Пример:");

				await botClient.SendTextMessageAsync(message.Chat.Id, "/adminchadm 1");
			}
			else if (lowerCaseMessage.Contains("как добавить пароль"))
			{
				await botClient.SendTextMessageAsync(message.Chat.Id,
					$"{RedCircleEmj} Для добавления нового пароля проверьте, имеете ли вы права администратора.\n" +
					$"{GreenCircleEmj} Для ввода нового пароля воспользуйтесь командой /adminaddpass и далее сам пароль.\n{BlueCircleEmj}Пример:");

				await botClient.SendTextMessageAsync(message.Chat.Id, "/adminaddpass mypassword123");
			}
			else if (lowerCaseMessage.Contains("как изменить пароль"))
			{
				await botClient.SendTextMessageAsync(message.Chat.Id,
					$"{RedCircleEmj} Для изменения старого пароля проверьте, имеете ли вы права администратора.\n" +
					$"{GreenCircleEmj} Далее воспользуйтесь командой /adminchpass далее номер пароля, который нужно изменить и сам пароль.\n{BrownCircleEmj}Пример:");

				await botClient.SendTextMessageAsync(message.Chat.Id, "/adminchpass 1 newpassword");
			}
			else if (lowerCaseMessage.Contains("как удалить пароль"))
			{
				await botClient.SendTextMessageAsync(message.Chat.Id,
					$"{RedCircleEmj} Для удаления пароля проверьте, имеете ли вы права администратора.\n" +
					$"{GreenCircleEmj} Далее воспользуйтесь командой /admindelpass далее номер пароля.\n{BrownCircleEmj}Пример:");

				await botClient.SendTextMessageAsync(message.Chat.Id, "/admindelpass 1");
			}
			else if (lowerCaseMessage.Contains("полный список команд"))
			{
				var user = await telegramUserRepo.GetTgUserByIdAsync(message.Chat.Id, token);
				if (user == null)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id,
						$"{AlertEmj}Возникла ошибка.\n" +
						"Пожалуйста, введите команду: /start");
					return;
				}
				if (user.IsAdmin)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id,
					$"{TabletEmj}Список команд администратора:\n\n" +
					"!admin - перейти в панель администратора\n" +
					"!user - перейти в панель пользователя\n" +
					"/adminaddpass - добавление пароля\n" +
					"/adminchpass - изменение пароля\n" +
					"/admindelpass - удаление пароля\n" +
					"/adminchadm - изменение прав пользователя.\n" +
					"/addevent - добавление нового мероприятия\n" +
					"/chevent - изменение ранее созданного мероприятия\n" +
					"/deleteevent - удаление мероприятия");
				}
				else
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}У вас нет прав администратора!");
				}
			}
			//else if (lowerCaseMessage.Contains("/adminaddpass"))
			//{
			//	string password = lowerCaseMessage.Replace("/adminaddpass", "");
			//	password.Replace(" ", "");
			//	if (await telegramUserRepo.AddPasswordAsync(new Password(password, message.Chat.Id), message.Chat.Id))
			//	{
			//		await botClient.SendTextMessageAsync(message.Chat.Id, $"{CheckMarkInBlockEmj}Пароль {password} успешно добавлен.");
			//	}
			//	else
			//	{
			//		await botClient.SendTextMessageAsync(message.Chat.Id, $"{NegativeRedEmj}Пароль не добавлен. Возможно у вас нет прав на добавление пароля, либо такой пароль уже существует.");
			//	}
			//}
			//else if (lowerCaseMessage.Contains("/adminchpass"))
			//{
			//	try
			//	{
			//		string messageToSend = await telegramUserRepo.UpdatePasswordByIdAsync(lowerCaseMessage, message.Chat.Id);
			//		await botClient.SendTextMessageAsync(message.Chat.Id, messageToSend);
			//	}
			//	catch (Exception)
			//	{
			//		await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при изменении пароля. Проверьте правильность ввода.");
			//	}
			//}
			//else if (lowerCaseMessage.Contains("/admindelpass"))
			//{
			//	try
			//	{
			//		if (await telegramUserRepo.DeletePasswordByIdAsync(lowerCaseMessage, message.Chat.Id))
			//		{
			//			await botClient.SendTextMessageAsync(message.Chat.Id, $"{CheckMarkInBlockEmj}Пароль успешно удален.");
			//		}
			//		else
			//		{
			//			await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Пароль не удален. Проверьте правильность введного номера пароля.");
			//		}
			//	}
			//	catch (Exception)
			//	{
			//		await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при удалении пароля. Возможно некорректный номер пароля. Помните, что удалять можно только свой пароль.");
			//	}
			//}
			else if (lowerCaseMessage.Contains("/adminchadm"))
			{
				try
				{
					if (await telegramUserRepo.UpdateAdminStatusTgUserAsync(lowerCaseMessage, message.Chat.Id, token))
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{CheckMarkInBlockEmj}Права администратора успешно изменены!");
					}
					else
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Права администратора изменить не удалось.\nВозможно у вас отсутствуют права администратора, либо отсутсвует такой идентификатор, либо вы пытаетесь изменить свои права.");
					}
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при изменении прав. Проверьте правильность ввода.");
				}
			}
			else if (lowerCaseMessage == "/adminhelp")
			{
				var user = await telegramUserRepo.GetTgUserByIdAsync(message.Chat.Id, token);
				if (user == null)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id,
						$"{AlertEmj}Возникла ошибка.\n" +
						"Пожалуйста, введите команду: /start");
					return;
				}
				if (user.IsAdmin)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id,
					$"{TabletEmj}Список команд администратора:\n\n" +
					"!admin - перейти в панель администратора\n" +
					"!user - перейти в панель пользователя\n" +
					"/adminaddpass - добавление пароля\n" +
					"/adminchpass - изменение пароля\n" +
					"/admindelpass - удаление пароля\n" +
					"/adminchadm - изменение прав пользователя.\n" +
					"/addevent - добавление нового мероприятия\n" +
					"/chevent - изменение ранее созданного мероприятия\n" +
					"/deleteevent - удаление мероприятия");
				}
				else
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}У вас нет прав администратора!");
				}
			}
			else if (lowerCaseMessage.Contains("/addevent"))
			{
				try
				{
					var user = await telegramUserRepo.GetTgUserByIdAsync(message.Chat.Id, token);
					if (user == null)
					{
						await botClient.SendTextMessageAsync(message.Chat.Id,
							$"{AlertEmj}Возникла ошибка.\n" +
							"Пожалуйста, введите команду: /start");
						return;
					}
					if (user.IsAdmin)
					{
						string messageToSend = lowerCaseMessage + "/" + message.Chat.Id;
						var result = await scienceEventRepo.AddEventAsync(messageToSend, token);
						if (!result)
						{
							await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Мероприятие с таким названием уже существует!");
							return;
						}
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{CheckMarkEmj}Мероприятие успешно добавлено!");
						await notifyService.NotifyLastAddEventUsersAsync($"{GreenCircleEmj}Новое мероприятие!", token);
					}
					else
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}У вас нет прав администратора.");
					}
				}
				catch (IrrelevatDateTimeException ex)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{ex.Message}");
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при добавлении мероприятия. Пожалуйста, проверьте правильность ввода мероприятия.");
				}
			}
			else if (lowerCaseMessage.Contains("/chevent"))
			{
				try
				{
					var user = await telegramUserRepo.GetTgUserByIdAsync(message.Chat.Id, token);
					if (user.IsAdmin)
					{
						string messageToSend = lowerCaseMessage + "/" + message.Chat.Id;
						var updEvent = await scienceEventRepo.UpdateFullEventAsync(messageToSend, token);
						if (updEvent == null)
						{
							await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Номера такого мероприятия не существует, либо название мероприятия повторяется, либо неверно указана дата мероприятия.");
							return;
						}
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{CheckMarkEmj}Мероприятие успешно изменено!");
						await notifyService.NotifyEventChangingUsersAsync(updEvent, $"{RedCircleEmj}Внимание! Изменения!", token);
					}
					else
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}У вас нет прав администратора.");
					}
				}
				catch (IrrelevatDateTimeException ex)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{ex.Message}");
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при изменении мероприятия. Пожалуйста, проверьте правильность ввода мероприятия.");
				}
			}
			else if (lowerCaseMessage.Contains("/deleteevent"))
			{
				try
				{
					var user = await telegramUserRepo.GetTgUserByIdAsync(message.Chat.Id, token);
					if (user.IsAdmin)
					{
						var res = await scienceEventRepo.DeleteEventByIdAsync(lowerCaseMessage, token);
						if (res == null)
						{
							await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Такого мероприятия не существует!");
						}
						else
						{
							await notifyService.NotifyEventChangingUsersAsync(res, $"{NegativeRedEmj}Мероприятие отменено!", token);
						}
					}
					else
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}У вас нет прав администратора.");
					}
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, $"{AlertEmj}Возникла ошибка при удалении мероприятия. Пожалуйста, проверьте правильность введенного id.");
				}
			}
			#endregion


			//обработка любых сообщений
			//Message sentMessage = await (messageText.Split(' ')[0].ToLower() switch
			//{
			//	"/start" => HelloWorld(msg),
			//	_ => Usage(msg)
			//});
			//logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);
		}

		#region [Methods]
		async Task<Message> HelloWorld(Message msg)
		{
			return await botClient.SendTextMessageAsync(msg.Chat, "Hey, buddy!");
		}
		async Task<Message> Usage(Message msg)
		{
			const string usage = """
                <b><u>Bot menu</u></b>:
                /photo          - send a photo
                /inline_buttons - send inline buttons
                /keyboard       - send keyboard buttons
                /remove         - remove keyboard buttons
                /request        - request location or contact
                /inline_mode    - send inline-mode results list
                /poll           - send a poll
                /poll_anonymous - send an anonymous poll
                /throw          - what happens if handler fails
            """;
			return await botClient.SendTextMessageAsync(msg.Chat, usage, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());
		}

		private Task UnknownUpdateHandlerAsync(Update update)
		{
			logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
			return Task.CompletedTask;
		}
		#endregion
	}
}
