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
		private readonly IValidationService _validationService;

		public TgUserService(ITelegramUserRepository tgUserRepo, IValidationService validationService,
			IUserCreatedEventRepository userCreatedEvRepo, IPasswordHasherService passwordHasherService)
		{
			_tgUserRepo = tgUserRepo ?? throw new ArgumentNullException(nameof(tgUserRepo));
			_userCreatedEvRepo = userCreatedEvRepo ?? throw new ArgumentNullException(nameof(userCreatedEvRepo));
			_passwordHasherService = passwordHasherService ?? throw new ArgumentNullException(nameof(passwordHasherService));
			_validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
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
		public TelegramUser? CreateUser(long chatId, string lowerCaseMessage, CancellationToken token)
		{
			string str = lowerCaseMessage.Replace("/addinfo/", "");
			var lst = str.Split("/");

			string name = Char.ToUpper(lst[0][0]) + lst[0].Substring(1);
			string surname = Char.ToUpper(lst[1][0]) + lst[1].Substring(1);
			string patronymic = Char.ToUpper(lst[2][0]) + lst[2].Substring(1);
			string phone = lst[3];

			TelegramUser newUser = new TelegramUser(chatId, name, surname, patronymic, phone, true, false);
			_validationService.ValidateTgUser(newUser, token);

			return newUser;
		}
		public void UpdateSubStatus(TelegramUser user, bool status, CancellationToken token)
		{
			if (user is null) throw new ArgumentNullException(nameof(user));
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

			string phone = lowerCaseMessage.Replace("/chphone/", "").Replace(" ", "");

			TelegramUser tmpUser = new TelegramUser(user.TgChatId, user.Name, user.Surname, user.Patronymic, phone);
			_validationService.ValidateTgUser(tmpUser, token);

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
		public List<TelegramUserDto> ConvertToTelegramUserDtoList(List<TelegramUser> users, CancellationToken token)
		{
			if (users is null)
				throw new ArgumentNullException(nameof(users));

			List<TelegramUserDto> lst = new List<TelegramUserDto>();
			foreach (var user in users)
			{
				TelegramUserDto newUser = new TelegramUserDto(user.Id, user.Name, user.Surname, user.Patronymic, user.PhoneNumber, user.TgChatId, user.LastMessageTime, user.IsSubscribed, user.IsAdmin);
				lst.Add(newUser);
			}
			return lst;
		}
		public TelegramUserDto ConvertToTelegramUserDto(TelegramUser user, CancellationToken token)
		{
			if (user is null)
				throw new ArgumentNullException(nameof(user));

			return new TelegramUserDto(user.Id, user.Name, user.Surname, user.Patronymic, user.PhoneNumber, user.TgChatId, user.LastMessageTime, user.IsSubscribed, user.IsAdmin);
		}
		public async Task<TelegramUser> ConvertToTelegramUser(TelegramUserDto userDto, CancellationToken token)
		{
			if (userDto is null)
				throw new ArgumentNullException(nameof(userDto));

			var user = await _tgUserRepo.GetTgUserByIdAsync(userDto.Id, token);
			if (user is null) throw new ArgumentNullException(nameof(user));

			user.Name = userDto.Name;
			user.Surname = userDto.Surname;
			user.Patronymic = userDto.Patronymic;
			user.PhoneNumber = userDto.PhoneNumber;
			user.TgChatId = userDto.TgChatId;
			user.IsSubscribed = userDto.IsSubscribed;
			user.IsAdmin = userDto.IsAdmin;
			user.LastMessageTime = userDto.LastMessageTime;
			return user;
		}
	}
}
