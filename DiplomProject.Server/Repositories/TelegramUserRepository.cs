using DiplomProject.Server.DbContexts;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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
		public async Task<bool> AddTgUserAsync(TelegramUser user)
		{
			if (user is null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			//проверка на существоание? (по chatId)
			if (TelegramUsers.ToList().Exists(e => e.TgChatId == user.TgChatId))
			{
				return false;
			}

			//добавление пользователя
			await TelegramUsers.AddAsync(user);
			await _dbContext.SaveChangesAsync();
			return true;
		}
		public async Task<IReadOnlyCollection<TelegramUser>> ReadAllTgUsersAsync()
		{
			return await TelegramUsers.ToListAsync();
		}

		public async Task<bool> UpdateSubStatusTgUserAsync(long chatId, bool subStatus)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			var _user = await GetTgUserByIdAsync(chatId);
			if (_user == null) return false;

			_user.IsSubscribed = subStatus;

			await _dbContext.SaveChangesAsync();
			return true;
		}
		public async Task<bool> UpdateSubStatusTgUserAsync(Guid Id, bool subStatus)
		{
			var _user = await GetTgUserByIdAsync(Id);
			if (_user == null) return false;


			_user.IsSubscribed = subStatus;

			await _dbContext.SaveChangesAsync();
			return true;
		}
		public async Task<bool> UpdateAdminStatusTgUserAsync(string lowerCaseMessage, long senderChatId)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
			{
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			}

			if (senderChatId < 0) throw new ArgumentOutOfRangeException(nameof(senderChatId));

			lowerCaseMessage = lowerCaseMessage.Replace("/adminchadm", "");
			lowerCaseMessage.Replace(" ", "");

			long chatId = long.Parse(lowerCaseMessage);

			var _sender = await GetTgUserByIdAsync(senderChatId);
			var _user = await GetTgUserByIdAsync(chatId);
			if (_sender == null || _user == null) return false;

			if (_sender.IsAdmin && senderChatId != chatId)
			{
				_user.IsAdmin = !_user.IsAdmin;

				await _dbContext.SaveChangesAsync();
				return true;
			}
			return false;
		}
		public async Task<string> UpdateNameTgUserAsync(long chatId, string newName)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			if (string.IsNullOrWhiteSpace(newName))
			{
				throw new ArgumentException($"\"{nameof(newName)}\" не может быть пустым или содержать только пробел.", nameof(newName));
			}

			var tgUser = await GetTgUserByIdAsync(chatId);
			if (tgUser == null) return "Ошибка. Пользователь не найден";

			newName = Char.ToUpper(newName[0]) + newName.Substring(1);
			tgUser.Name = newName;
			await _dbContext.SaveChangesAsync();
			return $"{GreenCircleEmj}Имя успешно изменено на: {newName}";
		}
		public async Task<string> UpdateSNameTgUserAsync(long chatId, string sName)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			if (string.IsNullOrWhiteSpace(sName))
			{
				throw new ArgumentException($"\"{nameof(sName)}\" не может быть пустым или содержать только пробел.", nameof(sName));
			}

			var tgUser = await GetTgUserByIdAsync(chatId);
			if (tgUser == null) return "Ошибка. Пользователь не найден";

			sName = Char.ToUpper(sName[0]) + sName.Substring(1);
			tgUser.Surname = sName;
			await _dbContext.SaveChangesAsync();
			return $"{GreenCircleEmj}Фамилия успешно изменена на: {sName}";
		}
		public async Task<string> UpdatePatrTgUserAsync(long chatId, string patronymic)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			if (string.IsNullOrWhiteSpace(patronymic))
			{
				throw new ArgumentException($"\"{nameof(patronymic)}\" не может быть пустым или содержать только пробел.", nameof(patronymic));
			}

			var tgUser = await GetTgUserByIdAsync(chatId);
			if (tgUser == null) return "Ошибка. Пользователь не найден";

			patronymic = Char.ToUpper(patronymic[0]) + patronymic.Substring(1);
			tgUser.Patronymic = patronymic;
			await _dbContext.SaveChangesAsync();
			return $"{GreenCircleEmj}Отчество успешно изменено на: {patronymic}";
		}
		public async Task<string> UpdatePhoneTgUserAsync(long chatId, string phoneNumb)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			if (string.IsNullOrWhiteSpace(phoneNumb))
			{
				throw new ArgumentException($"\"{nameof(phoneNumb)}\" не может быть пустым или содержать только пробел.", nameof(phoneNumb));
			}

			var tgUser = await GetTgUserByIdAsync(chatId);
			if (tgUser == null) return "Ошибка. Пользователь не найден";

			if (!phoneNumb.Contains("+7")) return $"{AlertEmj}Номер должен начинаться с +7!";
			if (phoneNumb.Length != 12) return $"{AlertEmj}Неверный номер телефона. Пожалуйста, проверьте правильность ввода.";

			tgUser.PhoneNumber = phoneNumb;
			await _dbContext.SaveChangesAsync();
			return $"{GreenCircleEmj}Номер успешно изменен на: {phoneNumb}";
		}
		public async Task<bool> UpdateAdminStatusTgUserAsync(Guid Id, bool adminStatus)
		{
			var _user = await GetTgUserByIdAsync(Id);
			if (_user == null) return false;


			_user.IsAdmin = adminStatus;

			await _dbContext.SaveChangesAsync();
			return true;
		}
		public async Task UpdateLastTimeMessageTgUserAsync(long chatId)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			var _user = await GetTgUserByIdAsync(chatId);
			//если пользователя нет в БД
			if (_user == null) return;


			_user.LastMessageTime = DateTime.UtcNow;
			await _dbContext.SaveChangesAsync();
		}
		public async Task<TelegramUser?> ReadTgUserByIdAsync(Guid Id)
		{
			var _user = await GetTgUserByIdAsync(Id);
			return _user;
		}
		public async Task<TelegramUser?> ReadTgUserByIdAsync(long chatId)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			var _user = await GetTgUserByIdAsync(chatId);
			return _user;
		}
		public async Task<bool> DeleteTgUserByIdAsync(Guid Id)
		{
			var _user = await GetTgUserByIdAsync(Id);
			if (_user is not null)
			{
				TelegramUsers.Remove(_user);
				await _dbContext.SaveChangesAsync();
				return true;
			}
			return false;
		}
		public async Task<bool> DeleteTgUserByIdAsync(long chatId)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			var _user = await GetTgUserByIdAsync(chatId);
			if (_user is not null)
			{
				TelegramUsers.Remove(_user);
				await _dbContext.SaveChangesAsync();
				return true;
			}
			return false;
		}
		public async Task<List<TelegramUser>> GetSubUsersListAsync()
		{
			var lst = from u in TelegramUsers
					  where u.IsSubscribed == true
					  select u;

			return await lst.ToListAsync();
		}
		public async Task<List<TelegramUser>> GetAdminUsersListAsync()
		{
			var lst = from u in TelegramUsers
					  where u.IsAdmin == true
					  select u;

			return await lst.ToListAsync();
		}
		private async Task<TelegramUser?> GetTgUserByIdAsync(long chatId)
		{
			return await TelegramUsers.FirstOrDefaultAsync(e => e.TgChatId == chatId);
		}
		private async Task<TelegramUser?> GetTgUserByIdAsync(Guid Id)
		{
			return await TelegramUsers.FirstOrDefaultAsync(e => e.Id == Id);
		}

		public async Task<List<TelegramUser>> GetUsersListAsync(CancellationToken ct)
		{
			return await TelegramUsers.ToListAsync();
		}
	}
}
