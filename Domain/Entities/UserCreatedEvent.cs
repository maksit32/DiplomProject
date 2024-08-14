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
	public class UserCreatedEvent : IEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; init; }
		public string NameEvent { get; set; }
		public string PlaceEvent { get; set; }
		public DateTime DateEvent { get; set; }
        public Guid TgUserId { get; set; }
		public TelegramUser TgUser { get; set; }
        //призер
        public bool IsWinner { get; set; }

		protected UserCreatedEvent() { }
		public UserCreatedEvent(string nameEvent, string placeEvent, DateTime dateEvent, bool isWinner, TelegramUser tgUser)
		{
			if (tgUser is null)
				throw new ArgumentNullException(nameof(tgUser));

			NameEvent = nameEvent;
			PlaceEvent = placeEvent;
			DateEvent = dateEvent;
			IsWinner = isWinner;
			//привязка данных
			TgUser = tgUser;
		}
		public override string ToString()
		{
			return $"{ButtonEmj} Название:   {NameEvent}\nНомер мероприятия:   {Id}\nДата:   {DateEvent.ToLocalTime().ToShortDateString()}\nМесто проведения:   {PlaceEvent}\nПризер:   {IsWinner}\n\n";
		}
	}
}
