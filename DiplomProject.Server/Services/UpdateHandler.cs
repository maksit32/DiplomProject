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
		IFillDataService fillDataService, IPasswordHasherService passwordHasherService, ITgUserService tgUserService, IConfiguration _configuration) : IUpdateHandler
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
					var res = await scienceEventRepo.DeleteEventByIdAsync(lowerCaseMessage, token);
					if (res == null)
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, ErrorDeleteAdminEvent);
					}
					else
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, SuccessDeleteAdminEvent);
						await notifyService.NotifyEventChangingUsersAsync(res, DeleteEventNotification, token);
					}
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
					string messageToSend = lowerCaseMessage + "/" + message.Chat.Id;
					var updEvent = await scienceEventRepo.UpdateFullEventAsync(messageToSend, token);
					if (updEvent == null)
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, ErrorChangeAdminEvent);
						return;
					}
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
					string messageToSend = lowerCaseMessage + "/" + message.Chat.Id;
					var result = await scienceEventRepo.AddEventAsync(messageToSend, token);
					if (!result)
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, ErrorAddAdminEvent);
						return;
					}
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
					var res = await telegramUserRepo.UpdateAdminStatusTgUserAsync(lowerCaseMessage, message.Chat.Id, token);
					await botClient.SendTextMessageAsync(message.Chat.Id, res ? SuccessChangeRights : ErrorChangeRights);
				}
				catch (Exception)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, ErrorChangeRights2);
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
					string password = lowerCaseMessage.Replace("/adminchpass", "").Replace("/", "").Replace(" ", "");
					if (String.IsNullOrWhiteSpace(password))
					{
						await botClient.SendTextMessageAsync(message.Chat.Id, ErrorChangePassword);
						return;
					}

					string hashedPassword = passwordHasherService.HashPassword(password);
					bool res = await telegramUserRepo.UpdatePasswordTgUserAsync(hashedPassword, message.Chat.Id, token);
					if (res) await botClient.SendTextMessageAsync(message.Chat.Id, SuccessChangePassword);
					else await botClient.SendTextMessageAsync(message.Chat.Id, ErrorChangePassword);
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
				//massive
				await botClient.SendTextMessageAsync(message.Chat.Id, AdminEventsActionsList);
				await botClient.SendTextMessageAsync(message.Chat.Id, AddAdminEventExample);

				await botClient.SendTextMessageAsync(message.Chat.Id, ChangeAdminEventInfo);
				await botClient.SendTextMessageAsync(message.Chat.Id, ChangeAdminEventExample);

				await botClient.SendTextMessageAsync(message.Chat.Id, DeleteAdminEventInfo);
				await botClient.SendTextMessageAsync(message.Chat.Id, DeleteAdminEventExample);
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
				string sendMessage = await scienceEventRepo.ReadAllActualEvAdminToStringAsync(token);
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
			if (user.IsAdmin)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, AdminPanel, replyMarkup: replyKeyboardAdmin);
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
				if (lowerCaseMessage == "/chphone")
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, ErrorChangePhone);
					return;
				}
				string phone = lowerCaseMessage.Replace("/chphone/", "");
				phone.Replace(" ", "");

				string mess = await telegramUserRepo.UpdatePhoneTgUserAsync(message.Chat.Id, phone, token);
				await botClient.SendTextMessageAsync(message.Chat.Id, mess);
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
				if (lowerCaseMessage == "/chpatr")
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, ErrorChangePatronymic);
					return;
				}
				string patr = lowerCaseMessage.Replace("/chpatr/", "");
				patr.Replace(" ", "");

				string mess = await telegramUserRepo.UpdatePatrTgUserAsync(message.Chat.Id, patr, token);
				await botClient.SendTextMessageAsync(message.Chat.Id, mess);
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
				if (lowerCaseMessage == "/chsname")
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, ErrorChangeSName);
					return;
				}
				string sName = lowerCaseMessage.Replace("/chsname/", "");
				sName.Replace(" ", "");

				string mess = await telegramUserRepo.UpdateSNameTgUserAsync(message.Chat.Id, sName, token);
				await botClient.SendTextMessageAsync(message.Chat.Id, mess);
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
				if (lowerCaseMessage == "/chname")
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, ErrorChangeName);
					return;
				}
				string name = lowerCaseMessage.Replace("/chname/", "");
				name.Replace(" ", "");

				string mess = await telegramUserRepo.UpdateNameTgUserAsync(message.Chat.Id, name, token);
				await botClient.SendTextMessageAsync(message.Chat.Id, mess);
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
				var deletedEvent = await userCreatedEventRepo.DeleteUserCreatedEventByIdAsync(lowerCaseMessage, message.Chat.Id, token);
				if (deletedEvent is not null)
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, SuccessDeleteUserEvent + deletedEvent.ToString());
				}
				else
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, ErrorDeleteUserEvent);
				}
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
				if (await userCreatedEventRepo.UpdateUserCreatedEventAsync(lowerCaseMessage, message.Chat.Id, token))
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, SuccessUpdateUserEvent);
				}
				else
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, ErrorUpdateUserEvent);
				}
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
				if (await userCreatedEventRepo.AddUserCreatedEventAsync(lowerCaseMessage, message.Chat.Id, token))
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, SuccessAddUserEvent);
				}
				else
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, ErrorAddUserEvent);
				}
			}
			catch (Exception)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, ErrorAddUserEvent2);
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
			if (user.IsSubscribed)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, UserPanel, replyMarkup: replyKeyboardUserNoSub);
			}
			else
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, UserPanel, replyMarkup: replyKeyboardUserSub);
			}
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
			string Place = _configuration["Place"].Replace("{LabelEmj}", LabelEmj);
			await botClient.SendTextMessageAsync(message.Chat.Id, Place);
		}

		private async Task StatementsInfo(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			await botClient.SendTextMessageAsync(message.Chat.Id, SNOInfo);
			await botClient.SendTextMessageAsync(message.Chat.Id, SNOExample);

			await botClient.SendTextMessageAsync(message.Chat.Id, SMUInfo);
			await botClient.SendTextMessageAsync(message.Chat.Id, SMUExample);
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
			await telegramUserRepo.UpdateSubStatusTgUserAsync(message.Chat.Id, false, token);
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
			await telegramUserRepo.UpdateSubStatusTgUserAsync(message.Chat.Id, true, token);
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
			await botClient.SendTextMessageAsync(message.Chat.Id, UserActionsList);
			await botClient.SendTextMessageAsync(message.Chat.Id, AddUserEventInfo);
			await botClient.SendTextMessageAsync(message.Chat.Id, AddUserEventExample);

			await botClient.SendTextMessageAsync(message.Chat.Id, UserChangeEventInfo);
			await botClient.SendTextMessageAsync(message.Chat.Id, UserChangeEventExample);

			await botClient.SendTextMessageAsync(message.Chat.Id, DeleteUserEventInfo);
			await botClient.SendTextMessageAsync(message.Chat.Id, DeleteUserEventExample);
		}

		private async Task GetCalendarEvents(Message message, CancellationToken token)
		{
			if (user is null)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, WelcomeText3);
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
				return;
			}
			string sendMessage = await scienceEventRepo.ReadAllActualEventsToStringAsync(token);
			if (string.IsNullOrWhiteSpace(sendMessage)) sendMessage = EventsAreEmpty;
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
					await botClient.SendTextMessageAsync(message.Chat.Id, TgBotIsActive);
					return;
				}
				else
				{
					await botClient.SendTextMessageAsync(message.Chat.Id, SuccessRegistration, replyMarkup: replyKeyboardUserNoSub);
				}
			}
			catch (Exception)
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, ErrorRegisterTgUser);
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
				await botClient.SendTextMessageAsync(message.Chat.Id, RegisterTgUser);
			}
			else
			{
				await botClient.SendTextMessageAsync(message.Chat.Id, TgBotIsActive, replyMarkup: replyKeyboardUserNoSub);
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
