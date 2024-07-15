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
		Task<IReadOnlyList<UserCreatedEvent>> ReadAllUserEventsAsync(TelegramUser tgUser);
		Task<bool> AddUserCreatedEventAsync(string message, long chatId);
		Task<bool> UpdateUserCreatedEventAsync(string message, long chatId);
		Task<UserCreatedEvent?> GetUserCreatedEventByIdAsync(Guid uEventId);
		Task<UserCreatedEvent?> DeleteUserCreatedEventByIdAsync(string message, long chatId);

	}
}
