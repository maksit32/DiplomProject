using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Repositories.Interfaces
{
	public interface IPasswordRepostitory : IRepository<Password>
	{
		Task<bool> AddPasswordAsync(Password password, long chatId);
		bool ComparePasswords(string password);
		Task<string> UpdatePasswordByIdAsync(string lowerCaseMessage, long chatId);
		Task<bool> DeletePasswordByIdAsync(string lowerCaseMessage, long chatId);
		Task<string> GetPasswordsByChatIdAsync(long chatId, CancellationToken ct);
		Task<string> GetInfoAboutTgUserAsync(long chatId);
		Task<bool> CheckLastTimeMessageAsync(long chatId);
	}
}
