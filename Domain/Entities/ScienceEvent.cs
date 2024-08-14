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
			NameEvent = nameEvent;
			DateEvent = dateEvent;
			PlaceEvent = placeEvent;
			RequirementsEvent = requirementsEvent;
			InformationEvent = informationEvent;
			DateEventCreated = DateTime.UtcNow;
			AddByAdminChatId = addByAdminChatId;
		}
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
