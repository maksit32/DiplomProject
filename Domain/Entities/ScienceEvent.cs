using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Constants.EmojiConstants;
using Domain.Exceptions;
using System.Globalization;

namespace Domain.Entities
{
	[Serializable]
	public class ScienceEvent : IEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; init; }
		public string NameEvent { get; set; }
		public DateTime DateEvent { get; set; }
		public string PlaceEvent { get; set; }
		public string RequirementsEvent { get; set; }
		public string InformationEvent { get; set; }
		public DateTime DateEventCreated { get; set; }
		public long AddByAdminChatId { get; set; }

		protected ScienceEvent() { }
		public ScienceEvent(string nameEvent, DateTime dateEvent, string placeEvent, string requirementsEvent, string informationEvent, long addByAdminChatId)
		{
			if (string.IsNullOrWhiteSpace(nameEvent))
			{
				throw new ArgumentException($"\"{nameof(nameEvent)}\" не может быть пустым или содержать только пробел.", nameof(nameEvent));
			}

			if (string.IsNullOrWhiteSpace(placeEvent))
			{
				throw new ArgumentException($"\"{nameof(placeEvent)}\" не может быть пустым или содержать только пробел.", nameof(placeEvent));
			}

			if (string.IsNullOrWhiteSpace(requirementsEvent))
			{
				throw new ArgumentException($"\"{nameof(requirementsEvent)}\" не может быть пустым или содержать только пробел.", nameof(requirementsEvent));
			}

			if (string.IsNullOrWhiteSpace(informationEvent))
			{
				throw new ArgumentException($"\"{nameof(informationEvent)}\" не может быть пустым или содержать только пробел.", nameof(informationEvent));
			}

			NameEvent = nameEvent;
			DateEvent = dateEvent;
			PlaceEvent = placeEvent;
			RequirementsEvent = requirementsEvent;
			InformationEvent = informationEvent;
			DateEventCreated = DateTime.UtcNow;
			AddByAdminChatId = addByAdminChatId;
		}
		//      public ScienceEvent(string lowerCaseMessage)
		//      {
		//	lowerCaseMessage = lowerCaseMessage.Replace("/chevent/", "");
		//	var dataArray = lowerCaseMessage.Split("/");
		//	Guid idEvent = Guid.Parse(dataArray[0]);
		//	this.NameEvent = char.ToUpper(dataArray[1][0]) + dataArray[1].Substring(1);
		//	this.DateEvent = DateTime.Parse(dataArray[2], new CultureInfo("ru-RU")).ToUniversalTime();
		//	this.PlaceEvent = char.ToUpper(dataArray[3][0]) + dataArray[3].Substring(1);
		//	this.RequirementsEvent = dataArray[4];
		//	this.InformationEvent = dataArray[5];
		//	this.AddByAdminChatId = long.Parse(dataArray[6]);
		//}
		public override string ToString()
		{
			return $"{ButtonEmj} Мероприятие:   {NameEvent}\nДата:   {DateEvent.ToLocalTime().ToShortDateString()}\nМесто проведения:\n{PlaceEvent}\nТребования:   {RequirementsEvent}\nДополнительная информация:\n{InformationEvent}\n\n";
		}
		public string ToStringAdmin()
		{
			return $"{ButtonEmj} Мероприятие:   {NameEvent}\nНомер мероприятия:   {Id}\nДата:   {DateEvent.ToLocalTime().ToShortDateString()}\nМесто проведения:\n{PlaceEvent}\nТребования:   {RequirementsEvent}\nДополнительная информация:\n{InformationEvent}\n\n";
		}
	}
}
