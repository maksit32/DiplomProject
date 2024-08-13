using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Domain.Entities
{
	[Serializable]
	public class TelegramUser : IEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; init; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public string Patronymic { get; set; }
		public string PhoneNumber { get; set; }
		public long TgChatId { get; set; }
		public bool IsSubscribed { get; set; } = false;
		public bool IsAdmin { get; set; } = false;
		public string? HashedPassword { get; set; } = null;
        public DateTime LastMessageTime { get; set; }
		//много эвентов (Fluent API)
		public List<UserCreatedEvent> UserCreatedEvents { get; set; } = new();

		protected TelegramUser() { }
		//спец. добавление для клиентского приложения
		public TelegramUser(Guid id, string name, string surname, string patronymic, string phoneNumber, long tgChatId, bool isSubscribed = false, bool isAdmin = false, string? hashedPassword = null)
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
			LastMessageTime = DateTime.UtcNow;
			Name = name;
			Surname = surname;
			Patronymic = patronymic;
			PhoneNumber = phoneNumber;
			HashedPassword = hashedPassword;
		}
		//добавление из телеграм бота
		public TelegramUser(long tgChatId, string name, string surname, string patronymic, string phoneNumber, bool isSubscribed = false, bool isAdmin = false, string? hashedPassword = null)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException($"\"{nameof(name)}\" не может быть пустым или содержать только пробел.", nameof(name));

			if (string.IsNullOrWhiteSpace(surname))
				throw new ArgumentException($"\"{nameof(surname)}\" не может быть пустым или содержать только пробел.", nameof(surname));

			if (string.IsNullOrWhiteSpace(patronymic))
				throw new ArgumentException($"\"{nameof(patronymic)}\" не может быть пустым или содержать только пробел.", nameof(patronymic));

			if (string.IsNullOrWhiteSpace(phoneNumber))
				throw new ArgumentException($"\"{nameof(phoneNumber)}\" не может быть пустым или содержать только пробел.", nameof(phoneNumber));
			if (tgChatId <= 0) throw new ArgumentOutOfRangeException("tgChatId");

			TgChatId = tgChatId;
			IsSubscribed = isSubscribed;
			IsAdmin = isAdmin;
			Name = name;
			Surname = surname;
			Patronymic = patronymic;
			PhoneNumber = phoneNumber;
			LastMessageTime = DateTime.UtcNow;
			HashedPassword = hashedPassword;
		}
		public override string ToString()
		{
			return $"Id: {Id}	Имя: {Name}	  Фамилия: {Surname}   Отчество: {Patronymic}   Номер: {PhoneNumber}   TgChatId: {TgChatId}	IsSubscribed: {IsSubscribed}	IsAdmin: {IsAdmin}";
		}
	}
}
