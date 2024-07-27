using DiplomProject.Server.DbContexts;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
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
			await _dbContext.SaveChangesAsync(token);
			return true;
		}

		public async Task<bool> UpdateSubStatusTgUserAsync(long chatId, bool subStatus, CancellationToken token)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			var _user = await GetTgUserByIdAsync(chatId, token);
			if (_user == null) return false;

			_user.IsSubscribed = subStatus;

			await _dbContext.SaveChangesAsync(token);
			return true;
		}
		public async Task<bool> UpdateSubStatusTgUserAsync(Guid Id, bool subStatus, CancellationToken token)
		{
			var _user = await GetTgUserByIdAsync(Id, token);
			if (_user == null) return false;


			_user.IsSubscribed = subStatus;

			await _dbContext.SaveChangesAsync(token);
			return true;
		}
		public async Task<bool> UpdateAdminStatusTgUserAsync(string lowerCaseMessage, long senderChatId, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
			{
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			}

			if (senderChatId < 0) throw new ArgumentOutOfRangeException(nameof(senderChatId));

			lowerCaseMessage = lowerCaseMessage.Replace("/adminchadm", "");
			lowerCaseMessage.Replace(" ", "");

			long chatId = long.Parse(lowerCaseMessage);

			var _sender = await GetTgUserByIdAsync(senderChatId, token);
			var _user = await GetTgUserByIdAsync(chatId, token);
			if (_sender == null || _user == null) return false;

			if (_sender.IsAdmin && senderChatId != chatId)
			{
				_user.IsAdmin = !_user.IsAdmin;

				await _dbContext.SaveChangesAsync(token);
				return true;
			}
			return false;
		}
		public async Task<string> UpdateNameTgUserAsync(long chatId, string newName, CancellationToken token)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			if (string.IsNullOrWhiteSpace(newName))
			{
				throw new ArgumentException($"\"{nameof(newName)}\" не может быть пустым или содержать только пробел.", nameof(newName));
			}

			var tgUser = await GetTgUserByIdAsync(chatId, token);
			if (tgUser == null) return "Ошибка. Пользователь не найден";

			newName = Char.ToUpper(newName[0]) + newName.Substring(1);
			tgUser.Name = newName;
			await _dbContext.SaveChangesAsync(token);
			return $"{GreenCircleEmj}Имя успешно изменено на: {newName}";
		}
		public async Task<string> UpdateSNameTgUserAsync(long chatId, string sName, CancellationToken token)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			if (string.IsNullOrWhiteSpace(sName))
			{
				throw new ArgumentException($"\"{nameof(sName)}\" не может быть пустым или содержать только пробел.", nameof(sName));
			}

			var tgUser = await GetTgUserByIdAsync(chatId, token);
			if (tgUser == null) return "Ошибка. Пользователь не найден";

			sName = Char.ToUpper(sName[0]) + sName.Substring(1);
			tgUser.Surname = sName;
			await _dbContext.SaveChangesAsync(token);
			return $"{GreenCircleEmj}Фамилия успешно изменена на: {sName}";
		}
		public async Task<string> UpdatePatrTgUserAsync(long chatId, string patronymic, CancellationToken token)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			if (string.IsNullOrWhiteSpace(patronymic))
			{
				throw new ArgumentException($"\"{nameof(patronymic)}\" не может быть пустым или содержать только пробел.", nameof(patronymic));
			}

			var tgUser = await GetTgUserByIdAsync(chatId, token);
			if (tgUser == null) return "Ошибка. Пользователь не найден";

			patronymic = Char.ToUpper(patronymic[0]) + patronymic.Substring(1);
			tgUser.Patronymic = patronymic;
			await _dbContext.SaveChangesAsync(token);
			return $"{GreenCircleEmj}Отчество успешно изменено на: {patronymic}";
		}
		public async Task<string> UpdatePhoneTgUserAsync(long chatId, string phoneNumb, CancellationToken token)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			if (string.IsNullOrWhiteSpace(phoneNumb))
			{
				throw new ArgumentException($"\"{nameof(phoneNumb)}\" не может быть пустым или содержать только пробел.", nameof(phoneNumb));
			}

			var tgUser = await GetTgUserByIdAsync(chatId, token);
			if (tgUser == null) return "Ошибка. Пользователь не найден";

			if (!phoneNumb.Contains("+7")) return $"{AlertEmj}Номер должен начинаться с +7!";
			if (phoneNumb.Length != 12) return $"{AlertEmj}Неверный номер телефона. Пожалуйста, проверьте правильность ввода.";

			tgUser.PhoneNumber = phoneNumb;
			await _dbContext.SaveChangesAsync(token);
			return $"{GreenCircleEmj}Номер успешно изменен на: {phoneNumb}";
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
		public async Task<TelegramUser?> ReadTgUserByIdAsync(Guid Id, CancellationToken token)
		{
			var _user = await GetTgUserByIdAsync(Id, token);
			return _user;
		}
		public async Task<TelegramUser?> ReadTgUserByIdAsync(long chatId, CancellationToken token)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			var _user = await GetTgUserByIdAsync(chatId, token);
			return _user;
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
		public async Task<bool> DeleteTgUserByIdAsync(long chatId, CancellationToken token)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			var _user = await GetTgUserByIdAsync(chatId, token);
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
		public async Task UpdateTgUserAsync(TelegramUser newUser, CancellationToken token)
		{
			var _user = await GetTgUserByIdAsync(newUser.Id, token);
			if (_user == null) return;

			_user.Name = newUser.Name;
			_user.Surname = newUser.Surname;
			_user.Patronymic = newUser.Patronymic;
			_user.PhoneNumber = newUser.PhoneNumber;
			_user.IsSubscribed = newUser.IsSubscribed;
			_user.IsAdmin = newUser.IsAdmin;

			await _dbContext.SaveChangesAsync(token);
		}
	}
}
