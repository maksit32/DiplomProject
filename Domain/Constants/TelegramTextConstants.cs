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

		public const string UserAddEvent = "Для добавления участия в мероприятии придерживайтесь следующей конструкции:\n" +
					"/adduserevent/Название мероприятия/Место проведения/Дата проведения/Статус призера (указываете true или false)\n" +
					$"{RedCircleEmj}Пример:";

		public const string UserChangeEvent = "Для изменения участия в мероприятии придерживайтесь следующей конструкции:\n" +
					"/updateuserevent/Название мероприятия/Место проведения/Дата проведения/Статус призера (указываете true или false)/Номер мероприятия\n" +
					$"{YellowCircleEmj}Пример:";

		public const string UserDeleteEvent = "Для удаления участия в мероприятии придерживайтесь следующей конструкции:\n" +
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

		public const string ChangeUserName = $"{RedCircleEmj}Для изменения имени введите команду:\n" +
					$"/chname/Ваше имя\nПример:";

		public const string ChangeUserSName = $"{YellowCircleEmj}Для изменения фамилии введите команду:\n" +
					$"/chsname/Ваша фамилия\nПример:";

		public const string ChangeUserPatronymic = $"{GreenCircleEmj}Для изменения отчества введите команду:\n" +
					$"/chpatr/Ваше отчество\nПример:";

		public const string ChangeUserPhone = $"{BlueCircleEmj}Для изменения номера телефона введите команду:\n" +
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

		public const string ChangeSEvent = $"{YellowCircleEmj} Формат изменения мероприятия:\n/chevent/Номер мероприятия/Название события/Дата события/Место проведения/Требования к участнику/Дополнительная информация\nПример:";

		public const string DeleteSEvent = $"{RedCircleEmj} Формат удаления мероприятия:\n/deleteevent/номер события\nПример:";

		public const string NoRules = $"{AlertEmj}У вас нет прав администратора.";

		public const string ChangeRights = "Для изменения прав администратора нужно:\n\n" +
						$"{RedCircleEmj} Иметь права администратора\n" +
						$"{GreenCircleEmj} Получить у пользователя номер чата (вызвать команду просмотра статусов !user)\n" +
						$"{BlueCircleEmj} Вызвать команду /adminchadm/номер чата\nПример:";

		public const string ChangePassword = $"{RedCircleEmj} Для изменения старого пароля проверьте, имеете ли вы права администратора.\n" +
					$"{GreenCircleEmj} Далее воспользуйтесь командой /adminchpass/ваш пароль.\nПример:";

		public const string AdminActionsList = $"{TabletEmj} Список команд администратора:\n\n" +
					"!admin - перейти в панель администратора\n" +
					"!user - перейти в панель пользователя\n" +
					"/adminchpass - изменение пароля\n" +
					"/adminchadm - изменение прав пользователя.\n" +
					"/addevent - добавление нового мероприятия\n" +
					"/chevent - изменение ранее созданного мероприятия\n" +
					"/deleteevent - удаление мероприятия";


	}
}
