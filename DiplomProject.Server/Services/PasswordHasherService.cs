using Domain.Entities;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;


namespace DiplomProject.Server.Services
{
	public class PasswordHasherService : IPasswordHasherService
	{
		public string HashPassword(string password)
		{
			if (string.IsNullOrWhiteSpace(password))
				throw new ArgumentException($"\"{nameof(password)}\" не может быть пустым или содержать только пробел.", nameof(password));

			using (var sha256 = SHA256.Create())
			{
				var bytes = Encoding.UTF8.GetBytes(password);
				var hash = sha256.ComputeHash(bytes);
				return Convert.ToBase64String(hash);
			}
		}

		public bool VerifyPassword(string hashedPassword, string providedPassword)
		{
			var hashedProvidedPassword = HashPassword(providedPassword);
			return hashedPassword == hashedProvidedPassword;
		}
	}
}
