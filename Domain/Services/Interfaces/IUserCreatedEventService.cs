using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
	public interface IUserCreatedEventService
	{
		UserCreatedEvent? CreateAddUserEvent(TelegramUser user, string lowerCaseMessage, CancellationToken token);
		Task<UserCreatedEvent?> CreateUpdateUserEventAsync(TelegramUser user, string lowerCaseMessage, CancellationToken token);
		Task<UserCreatedEvent?> CreateDeleteUserEventAsync(TelegramUser user, string lowerCaseMessage, CancellationToken token);
	}
}
