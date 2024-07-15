using DiplomProject.Server.DbContexts;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Domain.Constants.EmojiConstants;


namespace DiplomProject.Server.Repositories
{
	public class PasswordRepository : IPasswordRepostitory
	{
		private readonly DiplomDbContext _dbContext;
		private DbSet<Password> Passwords => _dbContext.Set<Password>();
		private DbSet<TelegramUser> TelegramUsers => _dbContext.Set<TelegramUser>();
		private DbSet<UserCreatedEvent> UserCreatedEvents => _dbContext.Set<UserCreatedEvent>();
		private readonly IPasswordHasherService _passwordHasherService;

		public PasswordRepository(DiplomDbContext dbContext, IPasswordHasherService passwordHasherService)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_passwordHasherService = passwordHasherService ?? throw new ArgumentNullException(nameof(passwordHasherService));
		}

		public async Task<bool> AddPasswordAsync(Password password, long chatId)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			if (password is null)
			{
				throw new ArgumentNullException(nameof(password));
			}

			var _user = await GetTgUserByIdAsync(chatId);
			if (_user == null) return false;
			if (_user.IsAdmin)
			{
				if (Passwords.ToList().Exists(e => e.HashedPassword == password.HashedPassword))
				{
					return false;
				}

				await Passwords.AddAsync(password);
				await _dbContext.SaveChangesAsync();
				return true;
			}
			return false;
		}
		public bool ComparePasswords(string password)
		{
			if (string.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentException($"\"{nameof(password)}\" не может быть пустым или содержать только пробел.", nameof(password));
			}

			if (Passwords.ToList().Count > 0)
			{
				string hashedPassword = _passwordHasherService.HashPassword(password);

				if (Passwords.ToList().Exists(e => e.HashedPassword == hashedPassword))
				{
					return true;
				}
				return false;
			}
			return true;
		}
		public async Task<string> UpdatePasswordByIdAsync(string lowerCaseMessage, long chatId)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
			{
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			}

			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			lowerCaseMessage = lowerCaseMessage.Replace("/adminchpass ", "");
			var dataArr = lowerCaseMessage.Split(" ");

			Guid passwordId = Guid.Parse(dataArr[0]);

			string newPassword = dataArr[1];

			var _user = await GetTgUserByIdAsync(chatId);
			if (_user == null) return $"{AlertEmj}Возникла ошибка. Свяжитесь с создателем бота.";
			//только админ
			if (_user.IsAdmin)
			{
				string newHashedPassword = _passwordHasherService.HashPassword(newPassword);
				var passwordObj = await GetPasswordByIdAsync(passwordId);
				if (passwordObj is not null && passwordObj.UserChatId == chatId)
				{
					//проверка на повтор пароля
					if (Passwords.ToList().Exists(e => e.HashedPassword == newHashedPassword))
					{
						return $"{AlertEmj}Такой пароль уже существует!";
					}
					passwordObj.HashedPassword = newHashedPassword;

					await _dbContext.SaveChangesAsync();
					return $"{CheckMarkInBlockEmj}Пароль с Id: " + passwordId + " успешно обновлен на: " + newPassword;
				}
				else
				{
					return $"{AlertEmj}Внимание! Можно изменять только свой пароль!";
				}
			}
			return $"{AlertEmj}К сожалению у вас нет прав администратора.";
		}
		public async Task<bool> DeletePasswordByIdAsync(string lowerCaseMessage, long chatId)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
			{
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			}

			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			string delId = lowerCaseMessage.Replace("/admindelpass", "");
			delId = delId.Replace(" ", "");
			Guid passwordId = Guid.Parse(delId);

			var _user = await GetTgUserByIdAsync(chatId);
			if (_user == null) return false;
			if (_user.IsAdmin)
			{
				var passwordObj = await GetPasswordByIdAsync(passwordId);
				if (passwordObj is not null && passwordObj.UserChatId == chatId)
				{
					Passwords.Remove(passwordObj);
					await _dbContext.SaveChangesAsync();
					return true;
				}
			}
			return false;
		}
		public async Task<string> GetPasswordsByChatIdAsync(long chatId, CancellationToken ct)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			string passwords = "";
			var _user = await GetTgUserByIdAsync(chatId);
			if (_user == null) return passwords;
			if (_user.IsAdmin)
			{
				var lstPasswords = from p in Passwords
								   where p.UserChatId == chatId
								   select p;

				foreach (var p in lstPasswords)
				{
					passwords += "Id: " + p.Id + "\n";
				}
			}
			return passwords;
		}
		private async Task<Password?> GetPasswordByIdAsync(Guid Id)
		{
			return await Passwords.FirstOrDefaultAsync(x => x.Id == Id);
		}
		public async Task<string> GetInfoAboutTgUserAsync(long chatId)
		{
			var tgUser = await GetTgUserByIdAsync(chatId);
			if (tgUser is null) return "Возникла ошибка! Обратитесь к создателю бота.";

			//получаем все мероприятия, добавленные пользователем
			string evString = "";
			var userEvents = await ReadAllUserEventsAsync(tgUser);
			if (userEvents is not null)
			{
				foreach (var ev in userEvents)
				{
					evString += ev.ToString();
				}
			}
			return $"{GreenCircleEmj} Статус подписки:	{tgUser.IsSubscribed}\n{YellowCircleEmj} Фамилия: {tgUser.Surname}\n{BrownCircleEmj} Имя: {tgUser.Name}\n{YellowCircleEmj} Отчество: {tgUser.Patronymic}\n{BrownCircleEmj} Номер телефона: {tgUser.PhoneNumber}\n{RedCircleEmj} Статус админа:	{tgUser.IsAdmin}\n{BlueCircleEmj} Номер чата:	{tgUser.TgChatId}\nУчастие в мероприятиях:\n\n{evString}";
		}
		public async Task<IReadOnlyList<UserCreatedEvent>> ReadAllUserEventsAsync(TelegramUser tgUser)
		{
			var lst = from e in UserCreatedEvents
					  where e.TgUser.Id == tgUser.Id
					  select e;
			return await lst.ToListAsync();
		}
		public async Task<bool> CheckLastTimeMessageAsync(long chatId)
		{
			var tgUser = await GetTgUserByIdAsync(chatId);
			//если пользователя вообще нет в БД
			if (tgUser is null) return true;

			return DateTime.UtcNow - tgUser.LastMessageTime > TimeSpan.FromSeconds(3);
		}

		private async Task<TelegramUser?> GetTgUserByIdAsync(long chatId)
		{
			return await TelegramUsers.FirstOrDefaultAsync(e => e.TgChatId == chatId);
		}
		private async Task<TelegramUser?> GetTgUserByIdAsync(Guid Id)
		{
			return await TelegramUsers.FirstOrDefaultAsync(e => e.Id == Id);
		}
	}
}
