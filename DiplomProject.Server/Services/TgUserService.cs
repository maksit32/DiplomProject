using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Telegram.Bot.Types;
using static Domain.Constants.EmojiConstants;


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
				{
					evString += ev.ToString();
				}
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
	}
}
