using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using static Domain.Constants.EmojiConstants;
using static Domain.Constants.TelegramTextConstants;



namespace DiplomProject.Server.Services
{
	public class TgUserService : ITgUserService
	{
		private readonly ITelegramUserRepository _tgUserRepo;
		private readonly IUserCreatedEventRepository _userCreatedEvRepo;
		private readonly IPasswordHasherService _passwordHasherService;

		public TgUserService(ITelegramUserRepository tgUserRepo, IUserCreatedEventRepository userCreatedEvRepo, IPasswordHasherService passwordHasherService)
		{
			_tgUserRepo = tgUserRepo ?? throw new ArgumentNullException("tgUserRepo is null!");
			_userCreatedEvRepo = userCreatedEvRepo ?? throw new ArgumentNullException("userCreatedEvRepo is null!");
			_passwordHasherService = passwordHasherService ?? throw new ArgumentNullException("passwordHasherService is null!");
		}
		public async Task<string> GetInfoAboutTgUserAsync(long chatId, CancellationToken token)
		{
			var tgUser = await _tgUserRepo.GetTgUserByIdAsync(chatId, token);
			if (tgUser is null) return "Возникла ошибка! Обратитесь к создателю бота.";

			//получаем все мероприятия, добавленные пользователем
			string evString = "";
			var userEvents = await _userCreatedEvRepo.ReadAllUserEventsAsync(tgUser, token);
			if (userEvents is not null)
			{
				foreach (var ev in userEvents)
					evString += ev.ToString();
			}
			return $"{GreenCircleEmj} Статус подписки:	{tgUser.IsSubscribed}\n{YellowCircleEmj} Фамилия: {tgUser.Surname}\n{BrownCircleEmj} Имя: {tgUser.Name}\n{YellowCircleEmj} Отчество: {tgUser.Patronymic}\n{BrownCircleEmj} Номер телефона: {tgUser.PhoneNumber}\n{RedCircleEmj} Статус админа:	{tgUser.IsAdmin}\n{BlueCircleEmj} Номер чата:	{tgUser.TgChatId}\nУчастие в мероприятиях:\n\n{evString}";
		}
		public TelegramUser HashTelegramUser(TelegramUser user, CancellationToken token)
		{
			if (user is null) throw new ArgumentNullException("User is null!");

			var hashedPass = _passwordHasherService.HashPassword(user.HashedPassword);
			user.HashedPassword = hashedPass;
			return user;
		}
		public TelegramUser? CreateUser(long chatId, string lowerCaseMessage, CancellationToken token)
		{
			string str = lowerCaseMessage.Replace("/addinfo/", "");
			var lst = str.Split("/");

			string name = Char.ToUpper(lst[0][0]) + lst[0].Substring(1);
			string surname = Char.ToUpper(lst[1][0]) + lst[1].Substring(1);
			string patronymic = Char.ToUpper(lst[2][0]) + lst[2].Substring(1);
			string phone = lst[3];

			// Валидация
			if (!Regex.IsMatch(phone, @"^\+7\d{10}$"))
				return null;

			return new TelegramUser(chatId, name, surname, patronymic, phone, true, false);
		}
		public void UpdateSubStatus(TelegramUser user, bool status, CancellationToken token)
		{
			if (user is null) throw new ArgumentNullException("user");
			user.IsSubscribed = status;
		}
		public void ChangeUserNameAction(TelegramUser user, string lowerCaseMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			if (lowerCaseMessage == "/chname")
				throw new ArgumentException(nameof(lowerCaseMessage));

			string name = lowerCaseMessage.Replace("/chname/", "");
			name.Replace(" ", "");
			name = Char.ToUpper(name[0]) + name.Substring(1);

			user.Name = name;
		}

		public void ChangeUserSNameAction(TelegramUser user, string lowerCaseMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			if (lowerCaseMessage == "/chsname")
				throw new ArgumentException(nameof(lowerCaseMessage));

			string sName = lowerCaseMessage.Replace("/chsname/", "");
			sName.Replace(" ", "");
			sName = Char.ToUpper(sName[0]) + sName.Substring(1);

			user.Surname = sName;
		}

		public void ChangeUserPatronymicAction(TelegramUser user, string lowerCaseMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			if (lowerCaseMessage == "/chpatr")
				throw new ArgumentException(nameof(lowerCaseMessage));

			string patr = lowerCaseMessage.Replace("/chpatr/", "");
			patr.Replace(" ", "");
			patr = Char.ToUpper(patr[0]) + patr.Substring(1);

			user.Patronymic = patr;
		}

		public void ChangeUserPhoneAction(TelegramUser user, string lowerCaseMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			if (lowerCaseMessage == "/chphone")
				throw new ArgumentException(nameof(lowerCaseMessage));

			string phone = lowerCaseMessage.Replace("/chphone/", "");
			phone.Replace(" ", "");
			string pattern = @"^\+7\d{10}$";
			if(!Regex.IsMatch(phone, pattern)) throw new InvalidDataException(nameof(phone));

			user.PhoneNumber = phone;
		}
		public void ChangeAdminPasswordAction(TelegramUser user, string hashedPassword, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(hashedPassword))
				throw new ArgumentException($"\"{nameof(hashedPassword)}\" не может быть пустым или содержать только пробел.", nameof(hashedPassword));

			user.HashedPassword = hashedPassword;
		}
		public string GetNoHashedPasswordAction(string lowerCaseMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));

			return lowerCaseMessage.Replace("/adminchpass", "").Replace("/", "").Replace(" ", "");
		}
		public async Task<TelegramUser?> ChangeAdminStatusAction(TelegramUser sender, string lowerCaseMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			if (sender is null)
				throw new ArgumentNullException(nameof(sender));

			lowerCaseMessage = lowerCaseMessage.Replace("/adminchadm/", "");
			lowerCaseMessage.Replace(" ", "");

			long chatId = long.Parse(lowerCaseMessage);

			var user = await _tgUserRepo.GetTgUserByIdAsync(chatId, token);
			if (sender == null || user == null) throw new ArgumentNullException("sender or user are null!");

			if (sender.IsAdmin && sender.TgChatId != chatId)
			{
				user.IsAdmin = !user.IsAdmin;
				return user;
			}
			return null;
		}
	}
}
