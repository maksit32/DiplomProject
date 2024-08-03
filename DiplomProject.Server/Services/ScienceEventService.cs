using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static Domain.Constants.EmojiConstants;


namespace DiplomProject.Server.Services
{
	public class ScienceEventService : IScienceEventService
	{
		private readonly IScienceEventRepository _scienceEventRepository;

		public ScienceEventService(IScienceEventRepository scienceEventRepository)
		{
			_scienceEventRepository = scienceEventRepository ?? throw new ArgumentNullException("scienceEventRepository is null!");
		}
		public async Task<string> ReadAllActualEventsToStringAsync(CancellationToken token)
		{
			var events = await _scienceEventRepository.ReadAllActualEventsAsync(token);
			string messageToSend = "";
			foreach (var e in events)
				messageToSend += e.ToString();

			return messageToSend;
		}
		public async Task<string> ReadAllActualEvAdminToStringAsync(CancellationToken token)
		{
			var events = await _scienceEventRepository.ReadAllActualEventsAsync(token);
			string messageToSend = "";
			foreach (var e in events)
				messageToSend += e.ToStringAdmin();

			return messageToSend;
		}

		public ScienceEvent? CreateAddAdminEvent(TelegramUser user, string lowerCaseMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			if (user is null) throw new ArgumentNullException(nameof(user));

			lowerCaseMessage = lowerCaseMessage.Replace("/addevent/", "");
			var dataArray = lowerCaseMessage.Split("/");

			//NameEvent
			string nameEvent = char.ToUpper(dataArray[0][0]) + dataArray[0].Substring(1);
			//DateEvent
			var cultureInfo = new CultureInfo("ru-RU");
			DateTime dateEventLocal = DateTime.Parse(dataArray[1], cultureInfo);
			// Преобразование в UTC
			DateTime dateEventUtc = dateEventLocal.ToUniversalTime();
			if (dateEventUtc < DateTime.UtcNow) { throw new IrrelevatDateTimeException($"{AlertEmj}Неверно указана дата события!"); }
			//PlaceEvent
			string placeEvent = char.ToUpper(dataArray[2][0]) + dataArray[2].Substring(1);
			//Requirement
			string requirement = dataArray[3];
			//Information
			string information = dataArray[4];
			ScienceEvent sEvent = new ScienceEvent(nameEvent, dateEventUtc, placeEvent, requirement, information, user.TgChatId);


			if (_scienceEventRepository.ReadAllActualEventsAsync(token).Result.ToList().Exists(e => e.NameEvent.ToLower() == sEvent.NameEvent.ToLower()))
			{
				return null;
			}
			return sEvent;
		}
		public async Task<ScienceEvent?> CreateUpdateAdminEvent(TelegramUser user, string lowerCaseMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			if (user is null) throw new ArgumentNullException(nameof(user));

			lowerCaseMessage = lowerCaseMessage.Replace("/chevent/", "");
			var dataArray = lowerCaseMessage.Split("/");

			//IdEvent
			Guid idEvent = Guid.Parse(dataArray[0]);
			//NameEvent
			string nameEvent = char.ToUpper(dataArray[1][0]) + dataArray[1].Substring(1);
			//проверка на повтор названия
			if (_scienceEventRepository.ReadAllActualEventsAsync(token).Result.ToList().Exists(e => e.NameEvent.ToLower() == nameEvent.ToLower() && e.Id != idEvent))
			{
				return null;
			}
			//DateEvent
			var cultureInfo = new CultureInfo("ru-RU");
			DateTime dateEventLocal = DateTime.Parse(dataArray[2], cultureInfo);
			// Преобразование в UTC
			DateTime dateEventUtc = dateEventLocal.ToUniversalTime();
			if (dateEventUtc < DateTime.UtcNow) { throw new IrrelevatDateTimeException($"{AlertEmj} Неверно указана дата события!"); }
			//PlaceEvent
			string placeEvent = char.ToUpper(dataArray[3][0]) + dataArray[3].Substring(1);
			//Requirement
			string requirement = dataArray[4];
			//Information
			string information = dataArray[5];

			//получаем старое мероприятие и меняем его значения
			var sEvent = await _scienceEventRepository.GetScienceEventByIdAsync(idEvent, token);
			if (sEvent == null) return null;

			sEvent.NameEvent = nameEvent;
			sEvent.DateEvent = dateEventUtc;
			sEvent.PlaceEvent = placeEvent;
			sEvent.RequirementsEvent = requirement;
			sEvent.InformationEvent = information;
			sEvent.DateEventCreated = DateTime.UtcNow;
			sEvent.AddByAdminChatId = user.TgChatId;

			return sEvent;
		}

		public async Task<ScienceEvent?> CreateDeleteAdminEvent(TelegramUser user, string lowerCaseMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			if (user is null) throw new ArgumentNullException(nameof(user));

			lowerCaseMessage = lowerCaseMessage.Replace("/deleteevent/", "").Replace(" ", "");
			Guid id = Guid.Parse(lowerCaseMessage);

			var sEvent = await _scienceEventRepository.GetScienceEventByIdAsync(id, token);
			return sEvent is not null ? sEvent : null;
		}
	}
}
