using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Telegram.Bot;
using static TelegramLibrary.TelegramLibrary;



namespace DiplomProject.Server.Services
{
	public class NotifyService : INotifyService
	{
		private ITelegramBotClient _botClient;
		private readonly ITelegramUserRepository _db;

		public NotifyService(ITelegramBotClient botClient, ITelegramUserRepository db)
		{
			if (botClient == null) throw new ArgumentNullException("ITelegramBotClient is null");
			if (db == null) throw new ArgumentNullException("TelegramUsersDb is null");


			_botClient = botClient;
			_db = db;
		}

		//специльно для экстренного оповещения ВСЕХ пользователей (собрания, перезагрузка бота итд)
		public async Task NotifyAllUsersAsync(string notifyMessage)
		{
			if (string.IsNullOrWhiteSpace(notifyMessage))
			{
				throw new ArgumentException($"\"{nameof(notifyMessage)}\" не может быть пустым или содержать только пробел.", nameof(notifyMessage));
			}

			if (_db == null) throw new NullReferenceException(nameof(_db));

			var tgUsersList = await _db.ReadAllTgUsersAsync();

			foreach (var tgUser in tgUsersList)
			{
				await WriteToTgUserAsync(_botClient, tgUser.TgChatId, notifyMessage);
			}
		}
		public async Task NotifySubUsersAsync(string notifyMessage)
		{
			if (string.IsNullOrWhiteSpace(notifyMessage))
			{
				throw new ArgumentException($"\"{nameof(notifyMessage)}\" не может быть пустым или содержать только пробел.", nameof(notifyMessage));
			}

			//отправляем всем кто подписан
			var subUsersGroup = await _db.GetSubUsersListAsync();
			foreach (var subUser in subUsersGroup)
			{
				await WriteToTgUserAsync(_botClient, subUser.TgChatId, notifyMessage);
			}
		}
		//оповестить о добавлении нового мероприятия
		public async Task NotifyLastAddEventUsersAsync(string notifyMessage)
		{
			if (string.IsNullOrWhiteSpace(notifyMessage))
			{
				throw new ArgumentException($"\"{nameof(notifyMessage)}\" не может быть пустым или содержать только пробел.", nameof(notifyMessage));
			}

			if (_db == null) throw new NullReferenceException(nameof(_db));


			//находим событие
			var lastCreatedEvent = await _db.GetLastCreatedEventAsync();
			if (lastCreatedEvent == null) return;


			await NotifySubUsersAsync($"{notifyMessage}\n\n" + lastCreatedEvent.ToString());
		}
		//изменено или отменено мероприятие
		public async Task NotifyEventChangingUsersAsync(ScienceEvent sEvent, string notifyMessage)
		{
			if (sEvent == null) throw new ArgumentNullException(nameof(sEvent));
			if (string.IsNullOrWhiteSpace(notifyMessage))
			{
				throw new ArgumentException($"\"{nameof(notifyMessage)}\" не может быть пустым или содержать только пробел.", nameof(notifyMessage));
			}

			await NotifySubUsersAsync($"{notifyMessage}\n\n" + sEvent.ToString());
		}

		public async Task NotifyAdminsAsync(string notifyMessage)
		{
			if (string.IsNullOrWhiteSpace(notifyMessage))
			{
				throw new ArgumentException($"\"{nameof(notifyMessage)}\" не может быть пустым или содержать только пробел.", nameof(notifyMessage));
			}
			if (_db == null) throw new NullReferenceException(nameof(_db));

			var adminTgUsers = await _db.GetAdminUsersListAsync();

			foreach (var adminTgUser in adminTgUsers)
			{
				await WriteToTgUserAsync(_botClient, adminTgUser.TgChatId, notifyMessage);
			}
		}
	}
}
