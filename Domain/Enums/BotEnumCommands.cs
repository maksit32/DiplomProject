using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
	public enum BotEnumCommands
	{
		[Display(Name = "/start")]
		Start = 0,
		[Display(Name = "/addinfo/")]
		AddInfo = 1,
		[Display(Name = "календарь мероприятий")]
		CalendarEvents = 2,
		[Display(Name = "возможности работы с вашими участиями в мероприятиях")]
		UserEventsActions = 3,
		[Display(Name = "подписаться на рассылку")]
		Subscribe = 4,
		[Display(Name = "отписаться от рассылки")]
		Unsubscribe = 5,
		[Display(Name = "проверить ваши статусы и участия в мероприятиях")]
		UserStatus = 6,
		[Display(Name = "подача заявления онлайн")]
		Statements = 7,
		[Display(Name = "наше местоположение")]
		Place = 8,
		[Display(Name = "связь с автором")]
		AuthorContacts = 9,
		[Display(Name = "!user")]
		UserPanel = 10,
		[Display(Name = "изменение ваших данных")]
		ChangeUserDataInfo = 11,
		[Display(Name = "помощь")]
		HelpUser = 12,
		[Display(Name = "/adduserevent")]
		AddUserEvent = 13,
		[Display(Name = "/updateuserevent")]
		UpdateUserEvent = 14,
		[Display(Name = "/deleteuserevent")]
		DeleteUserEvent = 15,
		[Display(Name = "/chname")]
		ChangeUserName = 16,
		[Display(Name = "/chsname")]
		ChangeUserSName = 17,
		[Display(Name = "/chpatr")]
		ChangeUserPatronymic = 18,
		[Display(Name = "/chphone")]
		ChangeUserPhoneNumber = 19,
		[Display(Name = "/snoapp")]
		FillSNOData = 20,
		[Display(Name = "/smuapp")]
		FillSMUData = 21,
		[Display(Name = "!admin")]
		ChangeAdminPanel = 22,
		[Display(Name = "календарь добавленных мероприятий")]
		CalendarAdminEvents = 23,
		[Display(Name = "список действий с мероприятиями")]
		AdminEventsActions = 24,
		[Display(Name = "как изменить права администратора")]
		ChangeAdminRightsInfo = 25,
		[Display(Name = "как изменить пароль")]
		ChangeAdminPasswordInfo = 26,
		[Display(Name = "полный список команд")]
		HelpAdmin = 27,
		[Display(Name = "/adminchpass")]
		ChangeAdminPassword = 28,
		[Display(Name = "/adminchadm")]
		GiveAdminRights = 29,
		[Display(Name = "/addevent")]
		AddAdminEvent = 30,
		[Display(Name = "/chevent")]
		ChangeAdminEvent = 31,
		[Display(Name = "/deleteevent")]
		DeleteAdminEvent = 32,
		Undefined = -1
	}
}
