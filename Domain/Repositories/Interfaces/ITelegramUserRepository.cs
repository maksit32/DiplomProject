using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Repositories.Interfaces
{
	public interface ITelegramUserRepository : IRepository<TelegramUser>
	{
		Task<bool> AddTgUserAsync(TelegramUser user, CancellationToken token);
		Task UpdateTgUserAsync(TelegramUser user, CancellationToken token);
		Task<bool> UpdateSubStatusTgUserAsync(Guid Id, bool subStatus, CancellationToken token);
		Task<bool> UpdateAdminStatusTgUserAsync(Guid Id, bool adminStatus, CancellationToken token);
		Task UpdateLastTimeMessageTgUserAsync(long chatId, CancellationToken token);
		Task<bool> DeleteTgUserByIdAsync(Guid Id, CancellationToken token);
		Task<TelegramUser?> GetTgUserByIdAsync(long chatId, CancellationToken token);
		Task<TelegramUser?> GetTgUserByIdAsync(Guid Id, CancellationToken token);
		Task<List<TelegramUser>> GetSubUsersListAsync(CancellationToken token);
		Task<List<TelegramUser>> GetAdminUsersListAsync(CancellationToken token);
		Task<List<TelegramUser>> GetUsersListAsync(CancellationToken token);
		Task<bool> CheckLastTimeMessageAsync(long chatId, CancellationToken token);
	}
}
