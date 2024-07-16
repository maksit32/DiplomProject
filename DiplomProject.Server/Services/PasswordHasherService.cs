using Domain.Entities;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;


namespace DiplomProject.Server.Services
{
	public class PasswordHasherService : IPasswordHasherService
	{
		private readonly IPasswordHasher<TelegramUser> _passwordHasher;

		public PasswordHasherService(IOptions<PasswordHasherOptions> options)
		{
			_passwordHasher = new PasswordHasher<TelegramUser>(options);
		}

		public string HashPassword(string password)
		{
			return _passwordHasher.HashPassword(null!, password);
		}

		//hashedPassword из бд
		public bool VerifyPassword(string hashedPassword, string providedPassword)
		{
			var result = _passwordHasher.VerifyHashedPassword(null!, hashedPassword, providedPassword);
			return result != PasswordVerificationResult.Failed;
		}
	}
}
