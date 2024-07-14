using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Domain.Services.Interfaces;


namespace Domain.Entities
{
	[Serializable]
	public class Password : IEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; init; }
		public string HashedPassword { get; set; }
		//id телеграма
		public long UserChatId { get; init; }

		public Password(string password, long userChatId, IPasswordHasherService passwordHasherService)
		{
			if (string.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentException($"\"{nameof(password)}\" не может быть пустым или содержать только пробел.", nameof(password));
			}

			HashedPassword = passwordHasherService.HashPassword(password);
			UserChatId = userChatId;
		}
	}
}
