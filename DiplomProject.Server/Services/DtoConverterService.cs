using DiplomProject.Server.Repositories;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;


namespace DiplomProject.Server.Services
{
	public class DtoConverterService : IDtoConverterService
	{
		private readonly ITelegramUserRepository _tgUserRepo;
		public DtoConverterService(ITelegramUserRepository tgUserRepo)
		{
			_tgUserRepo = tgUserRepo ?? throw new ArgumentNullException(nameof(tgUserRepo));
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
