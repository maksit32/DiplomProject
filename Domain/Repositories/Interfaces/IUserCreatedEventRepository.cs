using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Repositories.Interfaces
{
	public interface IUserCreatedEventRepository : IRepository<UserCreatedEvent>
	{
		Task AddUserCreatedEventAsync(UserCreatedEvent userCreatedEvent, CancellationToken token);
		Task<List<UserCreatedEvent>> ReadAllEventsAsync(CancellationToken token);
		Task<List<UserCreatedEvent>> ReadAllUserEventsAsync(TelegramUser tgUser, CancellationToken token);
		Task<List<UserCreatedEvent>> ReadAllUserEventsAsync(Guid UserId, CancellationToken token);
		Task UpdateUserCreatedEventAsync(UserCreatedEvent newEvent, CancellationToken token);
		Task<UserCreatedEvent?> GetUserCreatedEventByIdAsync(Guid uEventId, CancellationToken token);
		Task DeleteUserCreatedEvent(UserCreatedEvent ev,  CancellationToken token);
		Task DeleteUserCreatedEventByIdAsync(Guid Id, CancellationToken token);
	}
}
