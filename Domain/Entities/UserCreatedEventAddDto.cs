using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
	public class UserCreatedEventAddDto
	{
		public string NameEvent { get; set; }
		public string PlaceEvent { get; set; }
		public DateTime DateEvent { get; set; }
		public bool IsWinner { get; set; }
		public string PhoneNumber { get; set; }
		public UserCreatedEventAddDto(string nameEvent, string placeEvent, DateTime dateEvent, bool isWinner, string phoneNumber)
		{
			NameEvent = nameEvent;
			PlaceEvent = placeEvent;
			DateEvent = dateEvent;
			IsWinner = isWinner;
			PhoneNumber = phoneNumber;
		}
	}
}
