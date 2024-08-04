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
		TelegramUser? CreateUser(long chatId, string lowerCaseMessage, CancellationToken token);
		void UpdateSubStatus(TelegramUser user, bool status, CancellationToken token);
		void ChangeUserNameAction(TelegramUser user, string lowerCaseMessage, CancellationToken token);
		void ChangeUserSNameAction(TelegramUser user, string lowerCaseMessage, CancellationToken token);
		void ChangeUserPatronymicAction(TelegramUser user, string lowerCaseMessage, CancellationToken token);
		void ChangeUserPhoneAction(TelegramUser user, string lowerCaseMessage, CancellationToken token);
		void ChangeAdminPasswordAction(TelegramUser user, string hashedPassword, CancellationToken token);
		Task<TelegramUser?> ChangeAdminStatusAction(TelegramUser user, string lowerCaseMessage, CancellationToken token);
		string GetNoHashedPasswordAction(string lowerCaseMessage, CancellationToken token);
	}
}
