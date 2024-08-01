using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
	public interface ITgUserService
	{
		Task<string> GetInfoAboutTgUserAsync(long chatId, CancellationToken token);
		TelegramUser HashTelegramUser(TelegramUser user, CancellationToken token);
	}
}
