using Domain.Entities;
using Newtonsoft.Json;
using static LogLibrary.LogMethods;


namespace JSONLibrary
{
	public static class JSONSerializationMethods
	{
		public static TelegramUser DeserializeTgUser(string strObj)
		{
			if (string.IsNullOrWhiteSpace(strObj))
			{
				throw new ArgumentException($"\"{nameof(strObj)}\" не может быть пустым или содержать только пробел.", nameof(strObj));
			}

			try
			{
				TelegramUser user = JsonConvert.DeserializeObject<TelegramUser>(strObj);
				if (user == null) throw new NullReferenceException(nameof(user));

				return user;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Возникла ошибка! {ex}");
				throw;
			}
		}
		public static string SerializeTgUser(TelegramUser user)
		{
			string strObj = JsonConvert.SerializeObject(user);
			return strObj;
		}
		//для безопасного логгирования
		public static async Task<TelegramUser> DeserializeTgUser(string strObj, object locker)
		{
			if (string.IsNullOrWhiteSpace(strObj))
			{
				throw new ArgumentException($"\"{nameof(strObj)}\" не может быть пустым или содержать только пробел.", nameof(strObj));
			}

			try
			{
				TelegramUser user = JsonConvert.DeserializeObject<TelegramUser>(strObj);
				if (user == null) throw new NullReferenceException(nameof(user));

				return user;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Возникла ошибка! Проверьте лог!	Время: {DateTime.Now}");
				await WriteLogToFileAsync(locker, $"Возникла ошибка: {ex}  Время: {DateTime.Now}", "C:\\Users\\korni\\source\\repos\\ScienceMSTUCABot\\MSTUCABOT.ConsoleServer\\Logs\\errorLog.txt");
				throw;
			}
		}
		public static async Task<string> SerializeTgUser(TelegramUser user, object locker)
		{
			try
			{
				string strObj = JsonConvert.SerializeObject(user);
				return strObj;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Возникла ошибка! Проверьте лог!	Время: {DateTime.Now}");
				await WriteLogToFileAsync(locker, $"Возникла ошибка: {ex}  Время: {DateTime.Now}", "C:\\Users\\korni\\source\\repos\\ScienceMSTUCABot\\MSTUCABOT.ConsoleServer\\Logs\\errorLog.txt");
				throw;
			}
		}
	}
}
