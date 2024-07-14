

namespace LogLibrary
{
	public static class LogMethods
	{
		public static void RemoveLogFile(string logFilePath)
		{
			File.Delete(logFilePath);
		}
		public static async Task WriteLogToFileAsync(object lockObject, string message, string logFilePath)
		{
			bool isFirstTime = true;
			try
			{
				await Task.Run(() =>
				{
					// Создаем экземпляр StreamWriter с указанием пути к файлу
					lock (lockObject)
					{
						using (StreamWriter writer = new StreamWriter(logFilePath, true))
						{
							if (isFirstTime)
							{
								writer.WriteLine();
								isFirstTime = false;
							}
							writer.WriteLine(message);
						}
					}
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine("Возникла ошибка при записи лога: " + ex.Message);
			}
		}
	}
}
