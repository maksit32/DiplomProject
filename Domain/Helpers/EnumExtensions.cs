using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Helpers
{
	public static class EnumExtensions
	{
		public static string GetDisplayName(this Enum enumValue)
		{
			return enumValue.GetType()
							.GetMember(enumValue.ToString())
							.First()
							.GetCustomAttribute<DisplayAttribute>()
							?.GetName() ?? enumValue.ToString();
		}
		public static BotEnumCommands GetBotCommandFromText(string text)
		{
			return text switch
			{
				var command when command == BotEnumCommands.Start.GetDisplayName() => BotEnumCommands.Start,
				var command when command.Contains(BotEnumCommands.AddInfo.GetDisplayName()) => BotEnumCommands.AddInfo,
				var command when command.Contains(BotEnumCommands.CalendarEvents.GetDisplayName()) => BotEnumCommands.CalendarEvents,
				var command when command.Contains(BotEnumCommands.UserEventsActions.GetDisplayName()) => BotEnumCommands.UserEventsActions,
				var command when command.Contains(BotEnumCommands.Subscribe.GetDisplayName()) => BotEnumCommands.Subscribe,
				var command when command.Contains(BotEnumCommands.Unsubscribe.GetDisplayName()) => BotEnumCommands.Unsubscribe,
				var command when command.Contains(BotEnumCommands.UserStatus.GetDisplayName()) => BotEnumCommands.UserStatus,
				var command when command.Contains(BotEnumCommands.Statements.GetDisplayName()) => BotEnumCommands.Statements,
				var command when command.Contains(BotEnumCommands.Place.GetDisplayName()) => BotEnumCommands.Place,
				var command when command.Contains(BotEnumCommands.AuthorContacts.GetDisplayName()) => BotEnumCommands.AuthorContacts,
				var command when command == BotEnumCommands.UserPanel.GetDisplayName() => BotEnumCommands.UserPanel,
				var command when command.Contains(BotEnumCommands.ChangeUserDataInfo.GetDisplayName()) => BotEnumCommands.ChangeUserDataInfo,
				var command when command.Contains(BotEnumCommands.HelpUser.GetDisplayName()) => BotEnumCommands.HelpUser,
				var command when command.Contains(BotEnumCommands.AddUserEvent.GetDisplayName()) => BotEnumCommands.AddUserEvent,
				var command when command.Contains(BotEnumCommands.UpdateUserEvent.GetDisplayName()) => BotEnumCommands.UpdateUserEvent,
				var command when command.Contains(BotEnumCommands.DeleteUserEvent.GetDisplayName()) => BotEnumCommands.DeleteUserEvent,
				var command when command.Contains(BotEnumCommands.ChangeUserName.GetDisplayName()) => BotEnumCommands.ChangeUserName,
				var command when command.Contains(BotEnumCommands.ChangeUserSName.GetDisplayName()) => BotEnumCommands.ChangeUserSName,
				var command when command.Contains(BotEnumCommands.ChangeUserPatronymic.GetDisplayName()) => BotEnumCommands.ChangeUserPatronymic,
				var command when command.Contains(BotEnumCommands.ChangeUserPhoneNumber.GetDisplayName()) => BotEnumCommands.ChangeUserPhoneNumber,
				var command when command.Contains(BotEnumCommands.FillSNOData.GetDisplayName()) => BotEnumCommands.FillSNOData,
				var command when command.Contains(BotEnumCommands.FillSMUData.GetDisplayName()) => BotEnumCommands.FillSMUData,
				var command when command == BotEnumCommands.ChangeAdminPanel.GetDisplayName() => BotEnumCommands.ChangeAdminPanel,
				var command when command.Contains(BotEnumCommands.CalendarAdminEvents.GetDisplayName()) => BotEnumCommands.CalendarAdminEvents,
				var command when command.Contains(BotEnumCommands.AdminEventsActions.GetDisplayName()) => BotEnumCommands.AdminEventsActions,
				var command when command.Contains(BotEnumCommands.ChangeAdminRightsInfo.GetDisplayName()) => BotEnumCommands.ChangeAdminRightsInfo,
				var command when command.Contains(BotEnumCommands.ChangeAdminPasswordInfo.GetDisplayName()) => BotEnumCommands.ChangeAdminPasswordInfo,
				var command when command.Contains(BotEnumCommands.HelpAdmin.GetDisplayName()) => BotEnumCommands.HelpAdmin,
				var command when command.Contains(BotEnumCommands.ChangeAdminPassword.GetDisplayName()) => BotEnumCommands.ChangeAdminPassword,
				var command when command.Contains(BotEnumCommands.GiveAdminRights.GetDisplayName()) => BotEnumCommands.GiveAdminRights,
				var command when command.Contains(BotEnumCommands.AddAdminEvent.GetDisplayName()) => BotEnumCommands.AddAdminEvent,
				var command when command.Contains(BotEnumCommands.ChangeAdminEvent.GetDisplayName()) => BotEnumCommands.ChangeAdminEvent,
				var command when command.Contains(BotEnumCommands.DeleteAdminEvent.GetDisplayName()) => BotEnumCommands.DeleteAdminEvent,
				_ => BotEnumCommands.Undefined,
			};
		}
	}
}
