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
		Task<bool> AddTgUserAsync(TelegramUser user);
		Task<IReadOnlyCollection<TelegramUser>> ReadAllTgUsersAsync();
		Task<bool> UpdateSubStatusTgUserAsync(long chatId, bool subStatus);
		Task<bool> UpdateSubStatusTgUserAsync(Guid Id, bool subStatus);
		Task<bool> UpdateAdminStatusTgUserAsync(string lowerCaseMessage, long senderChatId);
		Task<string> UpdateNameTgUserAsync(long chatId, string newName);
		Task<string> UpdateSNameTgUserAsync(long chatId, string sName);
		Task<string> UpdatePatrTgUserAsync(long chatId, string patronymic);
		Task<string> UpdatePhoneTgUserAsync(long chatId, string phoneNumb);
		Task<bool> UpdateAdminStatusTgUserAsync(Guid Id, bool adminStatus);
		Task UpdateLastTimeMessageTgUserAsync(long chatId);
		Task<TelegramUser?> ReadTgUserByIdAsync(Guid Id);
		Task<bool> DeleteTgUserByIdAsync(Guid Id);
		Task<bool> DeleteTgUserByIdAsync(long chatId);
		Task<List<TelegramUser>> GetSubUsersListAsync();
		Task<List<TelegramUser>> GetAdminUsersListAsync();

	}
}
