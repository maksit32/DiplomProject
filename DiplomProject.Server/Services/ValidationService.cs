using Domain.Entities;
using Domain.Services.Interfaces;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;

namespace DiplomProject.Server.Services
{
	public class ValidationService : IValidationService
	{
		public bool ValidateScienceEvent(ScienceEvent scEvent, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(scEvent.NameEvent))
				throw new InvalidDataException(nameof(scEvent.NameEvent));
			if (string.IsNullOrWhiteSpace(scEvent.PlaceEvent))
				throw new InvalidDataException(nameof(scEvent.PlaceEvent));
			if (string.IsNullOrWhiteSpace(scEvent.RequirementsEvent))
				throw new InvalidDataException(nameof(scEvent.RequirementsEvent));
			if (string.IsNullOrWhiteSpace(scEvent.InformationEvent))
				throw new InvalidDataException(nameof(scEvent.InformationEvent));
			if (scEvent.DateEvent.ToUniversalTime() <= DateTime.UtcNow)
				throw new InvalidDataException(nameof(scEvent.NameEvent));

			return true;
		}

		public bool ValidateTgUser(TelegramUser tgUser, CancellationToken token)
		{
			if(string.IsNullOrWhiteSpace(tgUser.Name)) 
				throw new InvalidDataException(nameof(tgUser.Name));
			if (string.IsNullOrWhiteSpace(tgUser.Surname))
				throw new InvalidDataException(nameof(tgUser.Surname));
			if (string.IsNullOrWhiteSpace(tgUser.Patronymic))
				throw new InvalidDataException(nameof(tgUser.Patronymic));
			if (!Regex.IsMatch(tgUser.PhoneNumber, @"^\+7\d{10}$"))
				throw new InvalidDataException(nameof(tgUser.PhoneNumber));
			if (tgUser.TgChatId <= 0) throw new ArgumentOutOfRangeException(nameof(tgUser.TgChatId));
			
			return true;
		}

		public bool ValidateUserCreatedEvent(UserCreatedEvent uCreatedEv, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(uCreatedEv.NameEvent))
				throw new InvalidDataException(nameof(uCreatedEv.NameEvent));
			if (string.IsNullOrWhiteSpace(uCreatedEv.PlaceEvent))
				throw new InvalidDataException(nameof(uCreatedEv.PlaceEvent));
			if (uCreatedEv.DateEvent.ToUniversalTime() >= DateTime.UtcNow)
				throw new InvalidDataException(nameof(uCreatedEv.DateEvent));

			return true;
		}
	}
}
