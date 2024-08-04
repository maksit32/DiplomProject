using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
	public interface IValidationService
	{
		bool ValidateScienceEvent(ScienceEvent scEvent, CancellationToken token);
		bool ValidateTgUser(TelegramUser tgUser, CancellationToken token);
		bool ValidateUserCreatedEvent(UserCreatedEvent uCreatedEv, CancellationToken token);
	}
}
