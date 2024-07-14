using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Entities
{
	public class Password : IEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; init; }
		public string HashedPassword { get; set; }
		public Guid UserChatId { get; init; }

		public Password(string password, Guid chatId)
		{
			if (string.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentException($"\"{nameof(password)}\" не может быть пустым или содержать только пробел.", nameof(password));
			}

			HashedPassword = HashPasword(password);
			UserChatId = chatId;
		}
		public static string HashPasword(string password)
		{
			if (string.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentException($"\"{nameof(password)}\" не может быть пустым или содержать только пробел.", nameof(password));
			}

			using SHA256 hash = SHA256.Create();
			return Convert.ToHexString(hash.ComputeHash(Encoding.UTF8.GetBytes(password)));
		}
	}
}
