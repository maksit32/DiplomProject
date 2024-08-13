using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.Entities
{
	public class TelegramUserDto
	{
		public Guid Id { get; init; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Patronymic { get; set; }
		public string PhoneNumber { get; set; }
		public long TgChatId { get; set; }
		public bool IsSubscribed { get; set; } = false;
		public bool IsAdmin { get; set; } = false;
		public DateTime LastMessageTime { get; set; }
		public TelegramUserDto(Guid id, string name, string surname, string patronymic, string phoneNumber, long tgChatId, DateTime _lastMessageTime, bool isSubscribed = false, bool isAdmin = false)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException($"\"{nameof(name)}\" не может быть пустым или содержать только пробел.", nameof(name));

			if (string.IsNullOrWhiteSpace(surname))
				throw new ArgumentException($"\"{nameof(surname)}\" не может быть пустым или содержать только пробел.", nameof(surname));

			if (string.IsNullOrWhiteSpace(patronymic))
				throw new ArgumentException($"\"{nameof(patronymic)}\" не может быть пустым или содержать только пробел.", nameof(patronymic));

			if (string.IsNullOrWhiteSpace(phoneNumber))
				throw new ArgumentException($"\"{nameof(phoneNumber)}\" не может быть пустым или содержать только пробел.", nameof(phoneNumber));
			string pattern = @"^\+7\d{10}$";
			if (!Regex.IsMatch(phoneNumber, pattern)) throw new InvalidDataException("Invalid phone number!");
			if (tgChatId <= 0) throw new ArgumentOutOfRangeException("Поле tgChatId не должно быть меньше 0!");

			Id = id;
			TgChatId = tgChatId;
			IsSubscribed = isSubscribed;
			IsAdmin = isAdmin;
			LastMessageTime = _lastMessageTime;
			Name = name;
			Surname = surname;
			Patronymic = patronymic;
			PhoneNumber = phoneNumber;
		}
	}
}
