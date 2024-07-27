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
		Task<List<UserCreatedEvent>> ReadAllEventsAsync(CancellationToken token);
		Task<List<UserCreatedEvent>> ReadAllUserEventsAsync(TelegramUser tgUser, CancellationToken token);
		Task<List<UserCreatedEvent>> ReadAllUserEventsAsync(Guid Id, CancellationToken token);
		Task<bool> AddUserCreatedEventAsync(string message, long chatId, CancellationToken token);
		Task<bool> UpdateUserCreatedEventAsync(string message, long chatId, CancellationToken token);
		Task UpdateUserCreatedEventAsync(UserCreatedEvent newEvent, CancellationToken token);
		Task<UserCreatedEvent?> GetUserCreatedEventByIdAsync(Guid uEventId, CancellationToken token);
		Task<UserCreatedEvent?> DeleteUserCreatedEventByIdAsync(string message, long chatId, CancellationToken token);
		Task DeleteUserCreatedEventByIdAsync(Guid Id, CancellationToken token);

	}
}
