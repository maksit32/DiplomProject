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
using static Domain.Constants.EmojiConstants;


namespace DiplomProject.Server.Services
{
	public class NotifyService : INotifyService
	{
		private readonly ITelegramBotClient _botClient;
		private readonly ITelegramUserRepository _tgUserRepo;
		private readonly IScienceEventRepository _scienceEventRepo;


		public NotifyService(ITelegramBotClient botClient, ITelegramUserRepository tgUserRepo, IScienceEventRepository scEvRepo)
		{
			_botClient = botClient ?? throw new ArgumentNullException("ITelegramBotClient is null");
			_tgUserRepo = tgUserRepo ?? throw new ArgumentNullException("TelegramUserDb is null");
			_scienceEventRepo = scEvRepo ?? throw new ArgumentNullException("ScienceEventDb is null");
		}

		//специально для экстренного оповещения ВСЕХ пользователей (собрания, перезагрузка бота итд)
		public async Task NotifyAllUsersAsync(string notifyMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(notifyMessage))
				throw new ArgumentException($"\"{nameof(notifyMessage)}\" не может быть пустым или содержать только пробел.", nameof(notifyMessage));

			if (_tgUserRepo == null) throw new NullReferenceException(nameof(_tgUserRepo));

			var tgUsersList = await _tgUserRepo.GetUsersListAsync(token);

			foreach (var tgUser in tgUsersList)
				await WriteToTgUserAsync(_botClient, tgUser.TgChatId, notifyMessage);
		}
		public async Task NotifySubUsersAsync(string notifyMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(notifyMessage))
				throw new ArgumentException($"\"{nameof(notifyMessage)}\" не может быть пустым или содержать только пробел.", nameof(notifyMessage));

			var subUsersGroup = await _tgUserRepo.GetSubUsersListAsync(token);
			foreach (var subUser in subUsersGroup)
				await WriteToTgUserAsync(_botClient, subUser.TgChatId, notifyMessage);
		}
		//оповестить о добавлении нового мероприятия
		public async Task NotifyLastAddEventUsersAsync(string notifyMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(notifyMessage))
				throw new ArgumentException($"\"{nameof(notifyMessage)}\" не может быть пустым или содержать только пробел.", nameof(notifyMessage));

			if (_tgUserRepo == null) 
				throw new NullReferenceException(nameof(_tgUserRepo));

			var lastCreatedEvent = await _scienceEventRepo.GetLastCreatedEventAsync(token);
			if (lastCreatedEvent == null) return;

			await NotifySubUsersAsync($"{notifyMessage}\n\n" + lastCreatedEvent.ToString(), token);
		}
		//изменено или отменено мероприятие
		public async Task NotifyEventChangingUsersAsync(ScienceEvent sEvent, string notifyMessage, CancellationToken token)
		{
			if (sEvent == null) throw new ArgumentNullException(nameof(sEvent));
			if (string.IsNullOrWhiteSpace(notifyMessage))
				throw new ArgumentException($"\"{nameof(notifyMessage)}\" не может быть пустым или содержать только пробел.", nameof(notifyMessage));

			await NotifySubUsersAsync($"{notifyMessage}\n\n" + sEvent.ToString(), token);
		}

		public async Task NotifyAdminsAsync(string notifyMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(notifyMessage))
				throw new ArgumentException($"\"{nameof(notifyMessage)}\" не может быть пустым или содержать только пробел.", nameof(notifyMessage));
			if (_tgUserRepo == null) throw new NullReferenceException(nameof(_tgUserRepo));

			var adminTgUsers = await _tgUserRepo.GetAdminUsersListAsync(token);

			foreach (var adminTgUser in adminTgUsers)
				await WriteToTgUserAsync(_botClient, adminTgUser.TgChatId, notifyMessage);
		}
	}
}
