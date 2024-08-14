using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
	public interface IDtoConverterService
	{
		List<TelegramUserDto> ConvertToTelegramUserDtoList(List<TelegramUser> users, CancellationToken token);
		TelegramUserDto ConvertToTelegramUserDto(TelegramUser user, CancellationToken token);
		Task<TelegramUser> ConvertToTelegramUser(TelegramUserDto userDto, CancellationToken token);
		Task<ScienceEvent> ConvertToScienceEvent(ScienceEventAddDto scEventDto, CancellationToken token);
		Task<List<UserCreatedEventDto>> ConvertToUserCreatedEventDtoList(List<UserCreatedEvent> uCreatedEvents, CancellationToken token);
		Task<UserCreatedEventDto> ConvertToUserCreatedEventDto(UserCreatedEvent uCreatedEvent, CancellationToken token);
		Task<UserCreatedEvent> ConvertToUserCreatedEvent(UserCreatedEventDto uCreatedEvDto, CancellationToken token);
		Task<UserCreatedEvent> ConvertToUserCreatedEvent(UserCreatedEventAddDto addDto, CancellationToken token);
	}
}
