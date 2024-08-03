using DiplomProject.Server.DbContexts;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using static Domain.Constants.EmojiConstants;


namespace DiplomProject.Server.Repositories
{
	public class TelegramUserRepository : ITelegramUserRepository
	{
		private readonly DiplomDbContext _dbContext;
		private DbSet<TelegramUser> TelegramUsers => _dbContext.Set<TelegramUser>();

		public TelegramUserRepository(DiplomDbContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}
		public async Task<bool> AddTgUserAsync(TelegramUser user, CancellationToken token)
		{
			if (user is null)
				throw new ArgumentNullException(nameof(user));

			//уже зарегистрирован
			if (TelegramUsers.ToList().Exists(e => e.TgChatId == user.TgChatId))
				return false;

			//добавление пользователя
			await TelegramUsers.AddAsync(user);
			await _dbContext.SaveChangesAsync(token);
			return true;
		}
		public async Task UpdateTgUserAsync(TelegramUser user, CancellationToken token)
		{
			if (user is null)
				throw new ArgumentNullException(nameof(user));

			TelegramUsers.Update(user);
			await _dbContext.SaveChangesAsync(token);
		}
		public async Task<bool> UpdateSubStatusTgUserAsync(Guid Id, bool subStatus, CancellationToken token)
		{
			var _user = await GetTgUserByIdAsync(Id, token);
			if (_user == null) return false;


			_user.IsSubscribed = subStatus;

			await _dbContext.SaveChangesAsync(token);
			return true;
		}
		public async Task<bool> UpdateAdminStatusTgUserAsync(Guid Id, bool adminStatus, CancellationToken token)
		{
			var _user = await GetTgUserByIdAsync(Id, token);
			if (_user == null) return false;

			_user.IsAdmin = adminStatus;

			await _dbContext.SaveChangesAsync(token);
			return true;
		}
		public async Task UpdateLastTimeMessageTgUserAsync(long chatId, CancellationToken token)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			var _user = await GetTgUserByIdAsync(chatId, token);
			//если пользователя нет в БД
			if (_user == null) return;


			_user.LastMessageTime = DateTime.UtcNow;
			await _dbContext.SaveChangesAsync(token);
		}
		public async Task<bool> DeleteTgUserByIdAsync(Guid Id, CancellationToken token)
		{
			var _user = await GetTgUserByIdAsync(Id, token);
			if (_user is not null)
			{
				TelegramUsers.Remove(_user);
				await _dbContext.SaveChangesAsync(token);
				return true;
			}
			return false;
		}
		public async Task<List<TelegramUser>> GetSubUsersListAsync(CancellationToken token)
		{
			var lst = from u in TelegramUsers
					  where u.IsSubscribed == true
					  select u;

			return await lst.ToListAsync();
		}
		public async Task<List<TelegramUser>> GetAdminUsersListAsync(CancellationToken token)
		{
			var lst = from u in TelegramUsers
					  where u.IsAdmin == true
					  select u;

			return await lst.ToListAsync();
		}
		public async Task<TelegramUser?> GetTgUserByIdAsync(long chatId, CancellationToken token)
		{
			return await TelegramUsers.FirstOrDefaultAsync(e => e.TgChatId == chatId);
		}
		public async Task<TelegramUser?> GetTgUserByIdAsync(Guid Id, CancellationToken token)
		{
			return await TelegramUsers.FirstOrDefaultAsync(e => e.Id == Id);
		}

		public async Task<List<TelegramUser>> GetUsersListAsync(CancellationToken token)
		{
			return await TelegramUsers.ToListAsync();
		}
		public async Task<bool> CheckLastTimeMessageAsync(long chatId, CancellationToken token)
		{
			var tgUser = await GetTgUserByIdAsync(chatId, token);
			//если пользователя вообще нет в БД
			if (tgUser is null) return true;

			return DateTime.UtcNow - tgUser.LastMessageTime > TimeSpan.FromSeconds(3);
		}
	}
}
