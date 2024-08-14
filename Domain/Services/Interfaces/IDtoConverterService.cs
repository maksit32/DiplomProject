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
	}
}
