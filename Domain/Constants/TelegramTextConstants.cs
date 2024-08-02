using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Domain.Constants.EmojiConstants;

namespace Domain.Constants
{
	public static class TelegramTextConstants
	{
		public const string WelcomeText1 = $"Добро пожаловать в бот науки МГТУ ГА! {PlaneEmj}\n\n" +
			$"Внимание{AlertEmj}в боте установлена защита от спама.\n" +
			$"Время задержки между сообщениями 3 секунды.";

		public const string WelcomeText2 = $"{AlertEmj} В соответствии с требованиями статьи 9 Федерального закона от 27.07.2006 № 152-ФЗ «О персональных данных»," +
					" используя телеграм бот вы даете согласие членам Совета СНО и СМУ МГТУ ГА на автоматизированную, " +
					"а также без использования средств автоматизации, обработку моих персональных данных, " +
					"включающих фамилию, имя, отчество, дату рождения, должность, сведения о месте работы, месте учебы, " +
					"адрес электронной почты, номер контактного телефона.\r\n" +
					"Так же вы предоставляете свое согласие членам Совета СНО и СМУ МГТУ ГА на совершение действий (операций) с вашими персональными данными, " +
					"включая сбор, систематизацию, накопление, хранение, обновление, изменение, " +
					"использование, обезличивание, блокирование, уничтожение.\r\n";

		public const string WelcomeText3 = $"{GreenCircleEmj}Для продолжения использования телеграмм бота, заполните ваши данные в следующем формате:\n" +
					$"/addinfo/Ваше имя/Ваша фамилия/Ваше отчество/номер телефона (формат +7)\nПример:";

		public const string InvalidNumber = $"{AlertEmj}Неверно указанный номер!\nНомер должен начинаться с +7 и содержать 11 цифр.";

		public const string UserActionsList = "Данный бот позволяет выполнить следующие функции:\n" +
					$"{CheckMarkInBlockEmj} Добавить участие в мероприятии\n" +
					$"{ButtonEmj} Изменить участие в мероприятии\n" +
					$"{NegativeRedEmj} Удалить участие в мероприятии";

		public const string AddUserEventInfo = "Для добавления участия в мероприятии придерживайтесь следующей конструкции:\n" +
					"/adduserevent/Название мероприятия/Место проведения/Дата проведения/Статус призера (указываете true или false)\n" +
					$"{RedCircleEmj}Пример:";

		public const string UserChangeEventInfo = "Для изменения участия в мероприятии придерживайтесь следующей конструкции:\n" +
					"/updateuserevent/Название мероприятия/Место проведения/Дата проведения/Статус призера (указываете true или false)/Номер мероприятия\n" +
					$"{YellowCircleEmj}Пример:";

		public const string DeleteUserEventInfo = "Для удаления участия в мероприятии придерживайтесь следующей конструкции:\n" +
					"/deleteuserevent/Номер мероприятия\n" +
					$"{GreenCircleEmj}Пример:";

		public const string SNOInfo = $"{PlaneEmj} Для подачи заявления на вступление в СНО или СМУ воспользуйтесь следующими командами\n\n" +
					$"{GreenCircleEmj} Вступление в СНО:\n" +
					$"Формат вступления: /snoapp/Фамилия Имя Отчество(в родительном падеже)/Факультет/Группа/Номер телефона/Почта\n{RedCircleEmj} Пример:";

		public const string SMUInfo = $"{BrownCircleEmj} Вступление в СМУ:\n" +
					$"Формат вступления: /smuapp/Должность(в родительном падеже)/Фамилия Имя Отчество(в родительном падеже)/Структурное подразделение(наименование кафедры или иного подразделения)/Наличие ученой степени/Дата рождения/Номер телефона/Почта\n{YellowCircleEmj} Пример:";

		public const string Place = $"{LabelEmj}СМУ и СНО расположен в старом корпусе по адресу:\n" +
					"      г.Москва, ул.Пулковская, д.6.\n" +
					"      кабинет: 3-108";

		public const string Error = $"{AlertEmj}Возникла ошибка.\n" +
						"Пожалуйста, введите команду: /start";

		public const string ChangeUserNameInfo = $"{RedCircleEmj}Для изменения имени введите команду:\n" +
					$"/chname/Ваше имя\nПример:";

		public const string ChangeUserSNameInfo = $"{YellowCircleEmj}Для изменения фамилии введите команду:\n" +
					$"/chsname/Ваша фамилия\nПример:";

		public const string ChangeUserPatronymicInfo = $"{GreenCircleEmj}Для изменения отчества введите команду:\n" +
					$"/chpatr/Ваше отчество\nПример:";

		public const string ChangeUserPhoneInfo = $"{BlueCircleEmj}Для изменения номера телефона введите команду:\n" +
					$"/chphone/Ваш номер телефона\nПример:";

		public const string HelpCommands = $"{TabletEmj} Список команд пользователя:\n\n" +
					"/start - перезапустить бота\n" +
					"/chname - изменить имя\n" +
					"/chsname - изменить фамилию\n" +
					"/chpatr - изменить отчество\n" +
					"/chphone - изменить номер телефона (формат: +7)\n" +
					"/adduserevent - добавить участие в мероприятии\n" +
					"/updateuserevent - изменить ранее добавленное мероприятие\n" +
					"/deleteuserevent - удаление добавленного мероприятия\n" +
					"/snoapp - подать заявление на вступление в СНО\n" +
					"/smuapp - подать заявление на вступление в СМУ";

		public const string AdminEventsActionsList = $"{TabletEmj} Форматы действий с мероприятиями:\n\n\n" +
						$"{GreenCircleEmj} Формат добавления мероприятия:\n/addevent/Название события/Дата события/Место проведения/Требования к участнику/Дополнительная информация\nПример:";

		public const string ChangeAdminEventInfo = $"{YellowCircleEmj} Формат изменения мероприятия:\n/chevent/Номер мероприятия/Название события/Дата события/Место проведения/Требования к участнику/Дополнительная информация\nПример:";

		public const string DeleteAdminEventInfo = $"{RedCircleEmj} Формат удаления мероприятия:\n/deleteevent/номер события\nПример:";

		public const string NoRules = $"{AlertEmj}У вас нет прав администратора.";

		public const string ChangeRightsInfo = "Для изменения прав администратора нужно:\n\n" +
						$"{RedCircleEmj} Иметь права администратора\n" +
						$"{GreenCircleEmj} Получить у пользователя номер чата (вызвать команду просмотра статусов !user)\n" +
						$"{BlueCircleEmj} Вызвать команду /adminchadm/номер чата\nПример:";

		public const string ChangePasswordInfo = $"{RedCircleEmj} Для изменения старого пароля проверьте, имеете ли вы права администратора.\n" +
					$"{GreenCircleEmj} Далее воспользуйтесь командой /adminchpass/ваш пароль.\nПример:";

		public const string AdminActionsList = $"{TabletEmj} Список команд администратора:\n\n" +
					"!admin - перейти в панель администратора\n" +
					"!user - перейти в панель пользователя\n" +
					"/adminchpass - изменение пароля\n" +
					"/adminchadm - изменение прав пользователя.\n" +
					"/addevent - добавление нового мероприятия\n" +
					"/chevent - изменение ранее созданного мероприятия\n" +
					"/deleteevent - удаление мероприятия";
		public const string RegisterTgUser = "/addinfo/Иван/Иванов/Иванович/+79999999999";
		public const string ErrorRegisterTgUser = $"{AlertEmj}Ошибка. Проверьте правильность ввода.";
		public const string TgBotIsActive = $"{GreenCircleEmj} Телеграм бот уже активен.";
		public const string SuccessRegistration = $"{GreenCircleEmj} Данные успешно добавлены.";
		public const string EventsAreEmpty = $"{ButtonEmj}  На ближайшее время мероприятий не запланировано.";
		public const string AddUserEventExample = "/adduserevent/Название 1/Москва/01.01.2024/True";
		public const string UserChangeEventExample = "/updateuserevent/Название 1/Москва/01.01.2024/True/11ce84c9-08ba-487d-89ac-97cd166111fc";
		public const string DeleteUserEventExample = "/deleteuserevent/11ce84c9-08ba-487d-89ac-97cd166111fc";
		public const string Subscribe = $"{CheckMarkEmj}Вы успешно подписались на рассылку!";
		public const string UnSubscribe = $"{SleepZzEmj}Вы успешно отписались от рассылки!";
		public const string SNOExample = $"/snoapp/Иванова Ивана Ивановича/ФАСК/РС-5/+79999999999/example@example.ru";
		public const string SMUExample = $"/smuapp/аспиранта/Иванова Ивана Ивановича/ТЭРЭО ВТ/Ваша научная степень/20.01.1994/+79999999999/example@example.ru";
		public const string AuthorInfo = "Вы можете связаться с автором по:";
		public const string UserPanel = "Выбрана панель обычного пользователя.";
		public const string ChangeUserNameExample = "/chname/Николай";
		public const string ChangeUserSNameExample = "/chsname/Иванов";
		public const string ChangeUserPatronymicExample = "/chpatr/Иванович";
		public const string ChangeUserPhoneExample = "/chphone/+79999999999";
		public const string SuccessAddUserEvent = $"{CheckMarkInBlockEmj}Мероприятие успешно добавлено!";
		public const string SuccessUpdateUserEvent = $"{CheckMarkInBlockEmj}Мероприятие успешно изменено!";
		public const string SuccessDeleteUserEvent = $"{CheckMarkInBlockEmj}Мероприятие успешно удалено!\n";
		public const string ErrorAddUserEvent = $"{NegativeRedEmj}Мероприятие не добавлено! Возможно мероприятие с данным названием сущестует.";
		public const string ErrorUpdateUserEvent = $"{NegativeRedEmj}Мероприятие не изменено! Проверьте на правильность ввода данных и повтор названия.";
		public const string ErrorDeleteUserEvent = $"{NegativeRedEmj}Мероприятие не удалено! Проверьте правильность ввода данных.";
		public const string ErrorAddUserEvent2 = $"{AlertEmj}Возникла ошибка при добавлении!\nПроверьте правильность ввода данных.";
		public const string ErrorUpdateUserEvent2 = $"{AlertEmj}Возникла ошибка при изменении!\nПроверьте правильность ввода данных.";
		public const string ErrorDeleteUserEvent2 = $"{AlertEmj}Возникла ошибка при удалении!\nПроверьте правильность введенного номера мероприятия.";
		public const string ErrorChangeName = $"{RedCircleEmj} Ошибка! Проверьте правильность введенного имени.";
		public const string ErrorChangeName2 = $"{AlertEmj}Возникла ошибка при изменении имени!\nПожалуйста, проверьте отправленные вами данные.";
		public const string ErrorChangeSName = $"{RedCircleEmj} Ошибка! Проверьте правильность введенной фамилии.";
		public const string ErrorChangeSName2 = $"{AlertEmj}Возникла ошибка при изменении фамилии!\nПожалуйста, проверьте отправленные вами данные.";
		public const string ErrorChangePatronymic = $"{RedCircleEmj} Ошибка! Проверьте правильность введенного отчества.";
		public const string ErrorChangePatronymic2 = $"{AlertEmj}Возникла ошибка при изменении отчества!\nПожалуйста, проверьте отправленные вами данные.";
		public const string ErrorChangePhone = $"{RedCircleEmj} Ошибка! Проверьте правильность введенного номера телефона.";
		public const string ErrorChangePhone2 = $"{AlertEmj}Возникла ошибка при изменении номера телефона!\nПожалуйста, проверьте отправленные вами данные.";
		public const string ErrorDocumentCreation = $"{AlertEmj}Возникла ошибка при создании документа!\nПожалуйста, проверьте отправленные вами данные.";
		public const string AdminPanel = "Выбрана админ-панель.";
		public const string EmptyAdminCalendarEvents = $"{ButtonEmj}  На ближайшее время мероприятий не запланировано.";
		public const string AddAdminEventExample = $"/addevent/Событие 1/01.01.2024/Москва/нет/Хорошее мероприятие";
		public const string ChangeAdminEventExample = $"/chevent/1e3eca14-90b2-459c-8471-58c9c9cc4462/Событие 1/01.01.2024/Москва/нет/Хорошее мероприятие";
		public const string DeleteAdminEventExample = $"/deleteevent/1e3eca14-90b2-459c-8471-58c9c9cc4462";
		public const string ChangeRightsExample = "/adminchadm/1";
		public const string ChangePasswordExample = "/adminchpass/newpassword";
		public const string ErrorChangePassword = $"{RedCircleEmj} Ошибка при изменении пароля!";
		public const string SuccessChangePassword = $"{CheckMarkEmj} Пароль успешно изменен!";
		public const string ErrorChangePassword2 = $"{AlertEmj}Возникла ошибка при изменении пароля. Проверьте правильность ввода.";
		public const string SuccessChangeRights = $"{CheckMarkInBlockEmj} Права администратора успешно изменены!";
		public const string ErrorChangeRights = $"{AlertEmj} Права администратора изменить не удалось.\nВозможно у вас отсутствуют права администратора, либо отсутсвует такой идентификатор чата, либо вы пытаетесь изменить свои права.";
		public const string ErrorChangeRights2 = $"{AlertEmj} Возникла ошибка при изменении прав. Проверьте правильность ввода.";
		public const string ErrorAddAdminEvent = $"{AlertEmj} Мероприятие с таким названием уже существует!";
		public const string ErrorAddAdminEvent2 = $"{AlertEmj} Возникла ошибка при добавлении мероприятия. Пожалуйста, проверьте правильность ввода мероприятия.";
		public const string SuccessAddAdminEvent = $"{CheckMarkEmj} Мероприятие успешно добавлено!";
		public const string NewEventNotification = $"{GreenCircleEmj} Новое мероприятие!";
		public const string ErrorChangeAdminEvent = $"{AlertEmj} Номера такого мероприятия не существует, либо название мероприятия повторяется, либо неверно указана дата мероприятия.";
		public const string ErrorChangeAdminEvent2 = $"{AlertEmj} Возникла ошибка при изменении мероприятия. Пожалуйста, проверьте правильность ввода мероприятия.";
		public const string SuccessChangeAdminEvent = $"{CheckMarkEmj} Мероприятие успешно изменено!";
		public const string ChangeEventNotification = $"{RedCircleEmj} Внимание! Изменения!";
		public const string ErrorDeleteAdminEvent = $"{AlertEmj} Такого мероприятия не существует!";
		public const string ErrorDeleteAdminEvent2 = $"{AlertEmj} Возникла ошибка при удалении мероприятия. Пожалуйста, проверьте правильность введенного id.";
		public const string SuccessDeleteAdminEvent = $"{CheckMarkEmj} Мероприятие успешно отменено!";
		public const string DeleteEventNotification = $"{NegativeRedEmj} Мероприятие отменено!";
	}
}
