using DiplomProject.Server.Repositories;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;


namespace DiplomProject.Server.Services
{
	public class DtoConverterService : IDtoConverterService
	{
		private readonly ITelegramUserRepository _tgUserRepo;
		private readonly IUserCreatedEventRepository _userCreatedEventRepo;

		public DtoConverterService(ITelegramUserRepository tgUserRepo, IUserCreatedEventRepository userCreatedEventRepo)
		{
			_tgUserRepo = tgUserRepo ?? throw new ArgumentNullException(nameof(tgUserRepo));
			_userCreatedEventRepo = userCreatedEventRepo ?? throw new ArgumentNullException(nameof(userCreatedEventRepo));
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
		//изменение TelegramUser
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
		//добавление ScienceEvent
		public async Task<ScienceEvent> ConvertToScienceEvent(ScienceEventAddDto scEventDto, CancellationToken token)
		{
			var tgUser = await _tgUserRepo.GetTgUserByPhoneAsync(scEventDto.AdminPhoneNumber, token);
			return new ScienceEvent(scEventDto.NameEvent, scEventDto.DateEvent, scEventDto.PlaceEvent, scEventDto.RequirementsEvent, scEventDto.InformationEvent, tgUser.TgChatId);
		}

		public async Task<List<UserCreatedEventDto>> ConvertToUserCreatedEventDtoList(List<UserCreatedEvent> uCreatedEvents, CancellationToken token)
		{
			List<UserCreatedEventDto> dtoList = new List<UserCreatedEventDto>();
			foreach (var uEvent in uCreatedEvents)
			{
				var tgUser = await _tgUserRepo.GetTgUserByIdAsync(uEvent.TgUserId, token);
				UserCreatedEventDto newEventDto = new UserCreatedEventDto(uEvent.Id, uEvent.NameEvent, uEvent.PlaceEvent, uEvent.DateEvent, uEvent.IsWinner, tgUser.TgChatId);
				dtoList.Add(newEventDto);
			}
			return dtoList;
		}

		public async Task<UserCreatedEventDto> ConvertToUserCreatedEventDto(UserCreatedEvent uEvent, CancellationToken token)
		{
			var tgUser = await _tgUserRepo.GetTgUserByIdAsync(uEvent.TgUserId, token);
			return new UserCreatedEventDto(uEvent.Id, uEvent.NameEvent, uEvent.PlaceEvent, uEvent.DateEvent, uEvent.IsWinner, tgUser.TgChatId);
		}
		//для изменения
		public async Task<UserCreatedEvent> ConvertToUserCreatedEvent(UserCreatedEventDto uCreatedEvDto, CancellationToken token)
		{
			if (uCreatedEvDto is null) throw new ArgumentNullException(nameof(uCreatedEvDto));

			var updatedEvent = await _userCreatedEventRepo.GetUserCreatedEventByIdAsync(uCreatedEvDto.Id, token);
			if(updatedEvent is null) throw new ArgumentNullException(nameof(updatedEvent));

			updatedEvent.NameEvent = uCreatedEvDto.NameEvent;
			updatedEvent.PlaceEvent = uCreatedEvDto.PlaceEvent;
			updatedEvent.DateEvent = uCreatedEvDto.DateEvent;
			updatedEvent.IsWinner = uCreatedEvDto.IsWinner;

			return updatedEvent;
		}
		//для добавления (нет использования)
		public async Task<UserCreatedEvent> ConvertToUserCreatedEvent(UserCreatedEventAddDto addDto, CancellationToken token)
		{
			var tgUser = await _tgUserRepo.GetTgUserByPhoneAsync(addDto.PhoneNumber, token);
			return new UserCreatedEvent(addDto.NameEvent, addDto.PlaceEvent, addDto.DateEvent, addDto.IsWinner, tgUser);
		}
	}
}
