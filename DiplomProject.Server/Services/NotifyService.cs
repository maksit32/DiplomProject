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
		private readonly ITelegramUserRepository tgUserRepo;
		private readonly IScienceEventRepository scienceEventRepo;
		private ITelegramBotClient _botClient;


		public NotifyService(ITelegramBotClient botClient, ITelegramUserRepository repo1, IScienceEventRepository repo2)
		{
			if (botClient == null) throw new ArgumentNullException("ITelegramBotClient is null");
			if (repo1 == null) throw new ArgumentNullException("TelegramUsersDb is null");


			_botClient = botClient;
			tgUserRepo = repo1;
			scienceEventRepo = repo2;
		}

		//специльно для экстренного оповещения ВСЕХ пользователей (собрания, перезагрузка бота итд)
		public async Task NotifyAllUsersAsync(string notifyMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(notifyMessage))
			{
				throw new ArgumentException($"\"{nameof(notifyMessage)}\" не может быть пустым или содержать только пробел.", nameof(notifyMessage));
			}

			if (tgUserRepo == null) throw new NullReferenceException(nameof(tgUserRepo));

			var tgUsersList = await tgUserRepo.GetUsersListAsync(token);

			foreach (var tgUser in tgUsersList)
			{
				await WriteToTgUserAsync(_botClient, tgUser.TgChatId, notifyMessage);
			}
		}
		public async Task NotifySubUsersAsync(string notifyMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(notifyMessage))
			{
				throw new ArgumentException($"\"{nameof(notifyMessage)}\" не может быть пустым или содержать только пробел.", nameof(notifyMessage));
			}

			//отправляем всем кто подписан
			var subUsersGroup = await tgUserRepo.GetSubUsersListAsync(token);
			foreach (var subUser in subUsersGroup)
			{
				await WriteToTgUserAsync(_botClient, subUser.TgChatId, notifyMessage);
			}
		}
		//оповестить о добавлении нового мероприятия
		public async Task NotifyLastAddEventUsersAsync(string notifyMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(notifyMessage))
			{
				throw new ArgumentException($"\"{nameof(notifyMessage)}\" не может быть пустым или содержать только пробел.", nameof(notifyMessage));
			}

			if (tgUserRepo == null) throw new NullReferenceException(nameof(tgUserRepo));


			//находим событие
			var lastCreatedEvent = await scienceEventRepo.GetLastCreatedEventAsync(token);
			if (lastCreatedEvent == null) return;


			await NotifySubUsersAsync($"{notifyMessage}\n\n" + lastCreatedEvent.ToString(), token);
		}
		//изменено или отменено мероприятие
		public async Task NotifyEventChangingUsersAsync(ScienceEvent sEvent, string notifyMessage, CancellationToken token)
		{
			if (sEvent == null) throw new ArgumentNullException(nameof(sEvent));
			if (string.IsNullOrWhiteSpace(notifyMessage))
			{
				throw new ArgumentException($"\"{nameof(notifyMessage)}\" не может быть пустым или содержать только пробел.", nameof(notifyMessage));
			}

			await NotifySubUsersAsync($"{notifyMessage}\n\n" + sEvent.ToString(), token);
		}

		public async Task NotifyAdminsAsync(string notifyMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(notifyMessage))
			{
				throw new ArgumentException($"\"{nameof(notifyMessage)}\" не может быть пустым или содержать только пробел.", nameof(notifyMessage));
			}
			if (tgUserRepo == null) throw new NullReferenceException(nameof(tgUserRepo));

			var adminTgUsers = await tgUserRepo.GetAdminUsersListAsync(token);

			foreach (var adminTgUser in adminTgUsers)
			{
				await WriteToTgUserAsync(_botClient, adminTgUser.TgChatId, notifyMessage);
			}
		}
	}
}
