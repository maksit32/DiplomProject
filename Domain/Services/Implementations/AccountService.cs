using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Services.Implementations
{
	public class AccountService
	{
		private readonly ITelegramUserRepository _telegramUserRepository;
		private readonly IPasswordHasherService _passwordHasherService;

		public AccountService(ITelegramUserRepository repository, IPasswordHasherService passwordHasherService)
		{
			_telegramUserRepository = repository;
			_passwordHasherService = passwordHasherService;
		}

		public async Task<TelegramUser> Register(
			string name,
			string login,
			string email,
			string password,
			CancellationToken token,
			bool isAdmin = false)
		{
			ArgumentNullException.ThrowIfNull(name);
			ArgumentNullException.ThrowIfNull(login);
			ArgumentNullException.ThrowIfNull(email);
			ArgumentNullException.ThrowIfNull(password);
			var existedAccount =
				await _telegramUserRepository.FindByEmail(email, token);
			if (existedAccount != null)
				throw new EmailAlreadyExistsException("Email already used");
			var hashedPassword = _passwordHasherService.HashPassword(password);
			DateTime Date = DateTime.UtcNow;
			var newAccount = new TelegramUser(name, login, hashedPassword, email, isAdmin, Date);
			await _telegramUserRepository.Add(newAccount, token);
			return newAccount;
		}

		public async Task<TelegramUser> Authenticate(
			string email,
			string password,
			CancellationToken token)
		{
			ArgumentNullException.ThrowIfNull(email);
			ArgumentNullException.ThrowIfNull(password);
			var existedAccount =
				await _telegramUserRepository.FindByEmail(email, token);
			if (existedAccount == null)
				throw new AccountNotFoundException("Account not found");
			//var hashedPassword = _passwordHasherService.HashPassword(password);
			var result = _passwordHasherService.VerifyPassword(existedAccount.HashedPassword, password);
			if (result == false)
				throw new IncorrectPasswordException("Password incorrect");
			return existedAccount;
		}
	}
}
