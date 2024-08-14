using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
	public class ScienceEventAddDto
	{
		public string NameEvent { get; set; }
		public DateTime DateEvent { get; set; }
		public string PlaceEvent { get; set; }
		public string RequirementsEvent { get; set; }
		public string InformationEvent { get; set; }
		public string AdminPhoneNumber { get; set; }

		public ScienceEventAddDto(string nameEvent, string dateEvent, string placeEvent, string reqEvent, string infEvent, string phoneNumber)
		{
			NameEvent = nameEvent;
			DateEvent = DateTime.Parse(dateEvent).ToUniversalTime();
			PlaceEvent = placeEvent;
			RequirementsEvent = reqEvent;
			InformationEvent = infEvent;
			AdminPhoneNumber = phoneNumber;
		}
	}
}
