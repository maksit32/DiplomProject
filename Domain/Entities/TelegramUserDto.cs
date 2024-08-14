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
