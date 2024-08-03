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
		IFillDataService fillDataService, IPasswordHasherService passwordHasherService, ITgUserService tgUserService,
		IScienceEventService scienceEventService, IUserCreatedEventService userCreatedEventService, IConfiguration _configuration) : IUpdateHandler
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
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			if (user.IsAdmin)
			{
				try
				{
					var delSEvent = await scienceEventService.CreateDeleteAdminEvent(user, lowerCaseMessage, token);
					await scienceEventRepo.DeleteEventAsync(delSEvent, token);
					await botClient.SendTextMessageAsync(message.Chat.Id, SuccessDeleteAdminEvent);
					await notifyService.NotifyEventChangingUsersAsync(delSEvent, DeleteEventNotification, token);
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, ErrorDeleteAdminEvent2);
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
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			if (user.IsAdmin)
			{
				try
				{
					var updEvent = await scienceEventService.CreateUpdateAdminEvent(user, lowerCaseMessage, token);
					await scienceEventRepo.UpdateFullEventAsync(updEvent, token);
					await botClient.SendTextMessageAsync(message.Chat.Id, SuccessChangeAdminEvent);
					await notifyService.NotifyEventChangingUsersAsync(updEvent, ChangeEventNotification, token);
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, ErrorChangeAdminEvent2);
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
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			if (user.IsAdmin)
			{
				try
				{
					var newEvent = scienceEventService.CreateAddAdminEvent(user, lowerCaseMessage, token);
					await scienceEventRepo.AddEventAsync(newEvent, token);
					await botClient.SendTextMessageAsync(message.Chat.Id, SuccessAddAdminEvent);
					await notifyService.NotifyLastAddEventUsersAsync(NewEventNotification, token);
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, ErrorAddAdminEvent2);
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
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			if (user.IsAdmin)
			{
				try
				{
					var userToUpdate = await tgUserService.ChangeAdminStatusAction(user, lowerCaseMessage, token);
					await telegramUserRepo.UpdateTgUserAsync(userToUpdate, token);
					await botClient.SendTextMessageAsync(message.Chat.Id, SuccessChangeRights);
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, ErrorChangeRights);
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
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			if (user.IsAdmin)
			{
				try
				{
					string noHashedPassword = tgUserService.GetNoHashedPasswordAction(lowerCaseMessage, token);
					string hashedPassword = passwordHasherService.HashPassword(noHashedPassword);
					tgUserService.ChangeAdminPasswordAction(user, hashedPassword, token);
					await telegramUserRepo.UpdateTgUserAsync(user, token);
					await botClient.SendTextMessageAsync(message.Chat.Id, SuccessChangePassword);
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, ErrorChangePassword2);
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
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			await (user.IsAdmin ? botClient.SendTextMessageAsync(message.Chat.Id, AdminActionsList) : botClient.SendTextMessageAsync(message.Chat.Id, NoRules));
		}

		private async Task ChangeAdminPasswordInfo(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			if (user.IsAdmin)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, ChangePasswordInfo);
				await botClient.SendTextMessageAsync(message.Chat.Id, ChangePasswordExample);
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
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			if (user.IsAdmin)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, ChangeRightsInfo);
				await botClient.SendTextMessageAsync(message.Chat.Id, ChangeRightsExample);
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
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			if (user.IsAdmin)
			{
				string[] messages = { AdminEventsActionsList, AddAdminEventExample, ChangeAdminEventInfo, ChangeAdminEventExample, DeleteAdminEventInfo, DeleteAdminEventExample };
				foreach (var mess in messages)
					await botClient.SendTextMessageAsync(message.Chat.Id, mess);
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
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			if (user.IsAdmin)
			{
				string sendMessage = await scienceEventService.ReadAllActualEvAdminToStringAsync(token);
				if (string.IsNullOrWhiteSpace(sendMessage)) sendMessage = EmptyAdminCalendarEvents;
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
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			await (user.IsAdmin ? botClient.SendTextMessageAsync(message.Chat.Id, AdminPanel, replyMarkup: replyKeyboardAdmin) : botClient.SendTextMessageAsync(message.Chat.Id, NoRules));
		}

		private async Task FillSMUData(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			try
			{
				var messageToSend = await fillDataService.FillSMUDataAsync(lowerCaseMessage, message.Chat.Id, fileSMUFullPath, token);
				await botClient.SendTextMessageAsync(message.Chat.Id, messageToSend);
			}
			catch (Exception)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, ErrorDocumentCreation);
			}
		}

		private async Task FillSNOData(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			try
			{
				var messageToSend = await fillDataService.FillSNODataAsync(lowerCaseMessage, message.Chat.Id, fileSNOFullPath, token);
				await botClient.SendTextMessageAsync(message.Chat.Id, messageToSend);
			}
			catch (Exception)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, ErrorDocumentCreation);
			}
		}

		private async Task ChangeUserPhoneNumberAction(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			try
			{
				tgUserService.ChangeUserPhoneAction(user, lowerCaseMessage, token);
				await telegramUserRepo.UpdateTgUserAsync(user, token);
				await botClient.SendTextMessageAsync(message.Chat.Id, SuccessPhoneUpdate);
			}
			catch (Exception)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, ErrorChangePhone2);
			}
		}

		private async Task ChangeUserPatronymicAction(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			try
			{
				tgUserService.ChangeUserPatronymicAction(user, lowerCaseMessage, token);
				await telegramUserRepo.UpdateTgUserAsync(user, token);
				await botClient.SendTextMessageAsync(message.Chat.Id, SuccessPatronymicUpdate);
			}
			catch (Exception)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, ErrorChangePatronymic2);
			}
		}

		private async Task ChangeUserSNameAction(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			try
			{
				tgUserService.ChangeUserSNameAction(user, lowerCaseMessage, token);
				await telegramUserRepo.UpdateTgUserAsync(user, token);
				await botClient.SendTextMessageAsync(message.Chat.Id, SuccessSNameUpdate);
			}
			catch (Exception)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, ErrorChangeSName2);
			}
		}

		private async Task ChangeUserNameAction(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			try
			{
				tgUserService.ChangeUserNameAction(user, lowerCaseMessage, token);
				await telegramUserRepo.UpdateTgUserAsync(user, token);
				await botClient.SendTextMessageAsync(message.Chat.Id, SuccessNameUpdate);
			}
			catch (Exception)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, ErrorChangeName2);
			}
		}

		private async Task DeleteUserEvent(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			try
			{
				var deletedEvent = await userCreatedEventService.CreateDeleteUserEventAsync(user, lowerCaseMessage, token);
				await userCreatedEventRepo.DeleteUserCreatedEvent(deletedEvent, token);
				await botClient.SendTextMessageAsync(message.Chat.Id, SuccessDeleteUserEvent + deletedEvent.ToString());
			}
			catch (Exception)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, ErrorDeleteUserEvent2);
			}
		}

		private async Task UpdateUserEvent(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			try
			{
				var updatedEv = await userCreatedEventService.CreateUpdateUserEventAsync(user, lowerCaseMessage, token);
				await userCreatedEventRepo.UpdateUserCreatedEventAsync(updatedEv, token);
				await botClient.SendTextMessageAsync(message.Chat.Id, SuccessUpdateUserEvent);
			}
			catch (Exception)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, ErrorUpdateUserEvent2);
			}
		}

		private async Task AddUserEvent(Message message, string lowerCaseMessage, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			try
			{
				var ev = userCreatedEventService.CreateAddUserEvent(user, lowerCaseMessage, token);
				await userCreatedEventRepo.AddUserCreatedEventAsync(ev, token);
				await botClient.SendTextMessageAsync(message.Chat.Id, SuccessAddUserEvent);
			}
			catch (Exception)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, ErrorAddUserEvent);
			}
		}

		private async Task HelpUserCommands(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			await botClient.SendTextMessageAsync(message.Chat.Id, HelpCommands);
		}

		private async Task ChangeUserData(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			string[] messages = { ChangeUserNameInfo, ChangeUserNameExample, ChangeUserSNameInfo, ChangeUserSNameExample, ChangeUserPatronymicInfo, ChangeUserPatronymicExample, ChangeUserPhoneInfo, ChangeUserPhoneExample };
			foreach (var mes in messages)
				await botClient.SendTextMessageAsync(message.Chat.Id, mes);
		}

		private async Task SetUserPanel(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			await (user.IsSubscribed ? botClient.SendTextMessageAsync(message.Chat.Id, UserPanel, replyMarkup: replyKeyboardUserNoSub) : botClient.SendTextMessageAsync(message.Chat.Id, UserPanel, replyMarkup: replyKeyboardUserSub));
		}

		private async Task GetAuthorContacts(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			await botClient.SendTextMessageAsync(message.Chat.Id, AuthorInfo, replyMarkup: inlineAuthorKeyboard);
		}

		private async Task GetPlaceInfo(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			await botClient.SendTextMessageAsync(message.Chat.Id, _configuration["Place"].Replace("{LabelEmj}", LabelEmj));
		}

		private async Task StatementsInfo(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			string[] messages = { SNOInfo, SNOExample, SMUInfo, SMUExample };
			foreach (var mess in messages)
				await botClient.SendTextMessageAsync(message.Chat.Id, mess);
		}

		private async Task GetUserStatus(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			await botClient.SendTextMessageAsync(message.Chat.Id, await tgUserService.GetInfoAboutTgUserAsync(message.Chat.Id, token));
		}

		private async Task Unsubscribe(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			tgUserService.UpdateSubStatus(user, false, token);
			await telegramUserRepo.UpdateTgUserAsync(user, token);
			await botClient.SendTextMessageAsync(message.Chat.Id, UnSubscribe, replyMarkup: replyKeyboardUserSub);
		}

		private async Task Subscribe(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			tgUserService.UpdateSubStatus(user, true, token);
			await telegramUserRepo.UpdateTgUserAsync(user, token);
			await botClient.SendTextMessageAsync(message.Chat.Id, Domain.Constants.TelegramTextConstants.Subscribe, replyMarkup: replyKeyboardUserNoSub);
		}

		private async Task GetUserEventsActions(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			string[] messages = { UserActionsList, AddUserEventInfo, AddUserEventExample, UserChangeEventInfo, UserChangeEventExample, DeleteUserEventInfo, DeleteUserEventExample };
			foreach (var mess in messages)
				await botClient.SendTextMessageAsync(message.Chat.Id, mess);
		}

		private async Task GetCalendarEvents(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			string sendMessage = await scienceEventService.ReadAllActualEventsToStringAsync(token);
			if (string.IsNullOrWhiteSpace(sendMessage)) sendMessage = EventsAreEmpty;
			await botClient.SendTextMessageAsync(message.Chat.Id, sendMessage);
		}

		private async Task AddInfo(Message message, string lowerCaseMessage, CancellationToken token)
		{
			try
			{
				var newUser = tgUserService.CreateUser(message.Chat.Id, lowerCaseMessage, token);
				var res = await telegramUserRepo.AddTgUserAsync(newUser, token);
				await (res ? botClient.SendTextMessageAsync(message.Chat.Id, SuccessRegistration, replyMarkup: replyKeyboardUserNoSub) : botClient.SendTextMessageAsync(message.Chat.Id, TgBotIsActive));
			}
			catch (Exception)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, ErrorRegisterTgUser);
			}
		}

		private async Task Welcome(Message message, string messageText, CancellationToken token)
		{
			string[] textArray = { _configuration["WelcomeText1"].Replace("{PlaneEmj}", PlaneEmj).Replace("{AlertEmj}", AlertEmj), _configuration["WelcomeText2"].Replace("{AlertEmj}", AlertEmj) };
			foreach (var text in textArray)
				await botClient.SendTextMessageAsync(message.Chat.Id, text);

			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3, replyMarkup: replyKeyboardUserSub);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
			}
			else
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, TgBotIsActive, replyMarkup: replyKeyboardUserNoSub);
				tgUserService.UpdateSubStatus(user, true, token);
				await telegramUserRepo.UpdateTgUserAsync(user, token);
			}
		}

		private Task UnknownUpdateHandlerAsync(Update update)
		{
			logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
			return Task.CompletedTask;
		}
	}
}
