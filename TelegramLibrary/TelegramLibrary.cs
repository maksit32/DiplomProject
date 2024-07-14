using Telegram.Bot;


namespace TelegramLibrary
{
	public static class TelegramLibrary
	{
		public static async Task WriteToTgUserAsync(ITelegramBotClient botClient, long chatId, string message)
		{
			if (chatId < 0) throw new ArgumentOutOfRangeException(nameof(chatId));

			if (string.IsNullOrWhiteSpace(message))
			{
				throw new ArgumentException($"\"{nameof(message)}\" не может быть пустым или содержать только пробел.", nameof(message));
			}

			await botClient.SendTextMessageAsync(chatId, message);
		}
	}
}
