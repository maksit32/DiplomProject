using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Constants.EmojiConstants;

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
		public Guid AddByAdminChatId { get; set; }

		public ScienceEvent(string nameEvent, DateTime dateEvent, string placeEvent, string requirementsEvent, string informationEvent, Guid addByAdminChatId)
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
			DateEventCreated = DateTime.Now;
			AddByAdminChatId = addByAdminChatId;
		}
		public override string ToString()
		{
			return $"{ButtonEmj}Мероприятие:   {NameEvent}\nНомер мероприятия:   {Id}\nДата:   {DateEvent}\nМесто проведения:\n{PlaceEvent}\nТребования:   {RequirementsEvent}\nДополнительная информация:\n{InformationEvent}\n\n";
		}
	}
}
