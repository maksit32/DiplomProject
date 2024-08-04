using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Telegram.Bot.Types;

namespace DiplomProject.Server.Services
{
	public class UserCreatedEventService : IUserCreatedEventService
	{
		private readonly IUserCreatedEventRepository _userCreatedEventRepository;
		private readonly IValidationService _validationService;


		public UserCreatedEventService(IUserCreatedEventRepository userCreatedEventRepo, IValidationService validationService)
		{
			_userCreatedEventRepository = userCreatedEventRepo ?? throw new ArgumentNullException(nameof(userCreatedEventRepo));
			_validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
		}

		public UserCreatedEvent? CreateAddUserEvent(TelegramUser user, string lowerCaseMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			if (user is null) return null;

			lowerCaseMessage = lowerCaseMessage.Replace("/adduserevent/", "");
			var dataArr = lowerCaseMessage.Split('/');
			//NameEvent
			string nameEvent = char.ToUpper(dataArr[0][0]) + dataArr[0].Substring(1);
			//PlaceEvent
			string placeEvent = char.ToUpper(dataArr[1][0]) + dataArr[1].Substring(1);
			//DateEvent
			var cultureInfo = new CultureInfo("ru-RU");
			DateTime dateEventLocal = DateTime.Parse(dataArr[2], cultureInfo);
			// Преобразование в UTC
			DateTime dateEventUtc = dateEventLocal.ToUniversalTime();
			//IsWinner
			bool isWinner = bool.Parse(dataArr[3]);

			UserCreatedEvent uEvent = new UserCreatedEvent(nameEvent, placeEvent, dateEventUtc, isWinner, user);
			_validationService.ValidateUserCreatedEvent(uEvent, token);

			//проверка на уже добавленное ранее событие этим пользователем
			if (_userCreatedEventRepository.ReadAllUserEventsAsync(user, token).Result.Exists(e => e.NameEvent.ToLower() == uEvent.NameEvent.ToLower()))
				return null;

			return uEvent;
		}
		public async Task<UserCreatedEvent?> CreateUpdateUserEventAsync(TelegramUser user, string lowerCaseMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			if (user is null) return null;

			lowerCaseMessage = lowerCaseMessage.Replace("/updateuserevent/", "");
			var dataArr = lowerCaseMessage.Split('/');
			//NameEvent
			string nameEvent = char.ToUpper(dataArr[0][0]) + dataArr[0].Substring(1);
			//PlaceEvent
			string placeEvent = char.ToUpper(dataArr[1][0]) + dataArr[1].Substring(1);
			//DateEvent
			var cultureInfo = new CultureInfo("ru-RU");
			DateTime dateEventLocal = DateTime.Parse(dataArr[2], cultureInfo);
			// Преобразование в UTC
			DateTime dateEventUtc = dateEventLocal.ToUniversalTime();
			//IsWinner
			bool isWinner = bool.Parse(dataArr[3]);
			//EventId
			Guid eventId = Guid.Parse(dataArr[4]);

			UserCreatedEvent updatedEv = new UserCreatedEvent(nameEvent, placeEvent, dateEventUtc, isWinner, user);
			_validationService.ValidateUserCreatedEvent(updatedEv, token);

			//проверка, что событие именно этого пользователя
			var evToChange = await _userCreatedEventRepository.GetUserCreatedEventByIdAsync(eventId, token);
			if (evToChange is null) return null;

			if (evToChange.TgUser.Id == user.Id)
			{
				//название прочих мероприятий равны
				if (_userCreatedEventRepository.ReadAllUserEventsAsync(user, token).Result.ToList().Exists(e => e.NameEvent == nameEvent && e.Id != evToChange.Id))
					return null;

				evToChange.NameEvent = nameEvent;
				evToChange.PlaceEvent = placeEvent;
				evToChange.DateEvent = dateEventUtc;
				evToChange.IsWinner = isWinner;
				return evToChange;
			}
			return null;
		}
		public async Task<UserCreatedEvent?> CreateDeleteUserEventAsync(TelegramUser user, string lowerCaseMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			if (user is null) return null;

			lowerCaseMessage = lowerCaseMessage.Replace("/deleteuserevent/", "");
			Guid eventToDeleteId = Guid.Parse(lowerCaseMessage);

			//проверка, что событие именно этого пользователя
			var evToChange = await _userCreatedEventRepository.GetUserCreatedEventByIdAsync(eventToDeleteId, token);
			if (evToChange is null) return null;
			//принадлежит пользователю
			if (evToChange.TgUser.Id == user.Id)
				return evToChange;
			return null;
		}
	}
}
