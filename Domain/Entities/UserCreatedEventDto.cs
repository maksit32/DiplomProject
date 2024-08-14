using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
	public class UserCreatedEventDto
	{
		public Guid Id { get; init; }
		public string NameEvent { get; set; }
        public string PlaceEvent { get; set; }
        public DateTime DateEvent { get; set; }
		public bool IsWinner { get; set; }
		public long ChatId { get; set; }
		public UserCreatedEventDto(Guid id, string nameEvent, string placeEvent, DateTime dateEvent, bool isWinner, long chatId) 
		{ 
			Id = id;
			NameEvent = nameEvent;
			PlaceEvent = placeEvent;
			DateEvent = dateEvent;
			IsWinner = isWinner;
			ChatId = chatId;
		}
	}
}
