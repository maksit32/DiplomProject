using Domain.Services.Interfaces;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;


using static Domain.Constants.EmojiConstants;



namespace DiplomProject.Server.Services
{
	public class FillDocxOpenXMLDataService : IFillDataService
	{
		private readonly IServiceProvider serviceProvider;
		private FileInfo fileInfo;
		private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
		private readonly long maxSize = 150000000; // Примерно 50 заявлений

		public FillDocxOpenXMLDataService(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		public async Task<string> FillSNODataAsync(string lowerCaseMessage, long chatId, string fileFullPath, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
			{
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			}

			if (File.Exists(fileFullPath))
			{
				fileInfo = new FileInfo(fileFullPath);
			}

			if (chatId <= 0) throw new ArgumentOutOfRangeException("chatId out of range");

			if (token.IsCancellationRequested) token.ThrowIfCancellationRequested();

			lowerCaseMessage = lowerCaseMessage.Replace("/snoapp/", "");
			var dataArr = lowerCaseMessage.Split('/');

			// Получение ФИО с большой буквы
			var fioArr = dataArr[0].Split(" ");
			string fioStr = string.Join(" ", fioArr.Select(str => char.ToUpper(str[0]) + str.Substring(1)));

			var replaceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("<FROM>", fioStr),
			new KeyValuePair<string, string>("<FAT>", dataArr[1].ToUpper()),
			new KeyValuePair<string, string>("<GROUP>", dataArr[2].ToUpper()),
			new KeyValuePair<string, string>("<NUMB>", dataArr[3]),
			new KeyValuePair<string, string>("<MAIL>", dataArr[4]),
			new KeyValuePair<string, string>("<DAY>", DateTime.Now.Day.ToString()),
			new KeyValuePair<string, string>("<MONTH>", DateTime.Now.Month.ToString()),
			new KeyValuePair<string, string>("<YEAR>", DateTime.Now.Year.ToString())
		};

			if (!ValidateData(replaceList)) return $"{AlertEmj} Неверно указаны данные.";

			if (fileInfo != null && !string.IsNullOrWhiteSpace(fileInfo.DirectoryName))
			{
				await semaphoreSlim.WaitAsync();
				try
				{
					if (DirectorySize(new DirectoryInfo(Path.Combine(fileInfo.DirectoryName, "SNOApplications")), maxSize) > maxSize)
					{
						using var scope = serviceProvider.CreateScope();

						var notifyService = scope.ServiceProvider.GetRequiredService<INotifyService>();
						await notifyService.NotifyAdminsAsync($"{AlertEmj} Внимание, обработайте заявления на вступление в СНО!\nПрием заявлений временно остановлен. Требуется освободить память.", token);
						return $"{BlueCircleEmj} Сервер занят обработкой предыдущих заявлений.\nПожалуйста, повторите попытку позднее.";
					}

					await Task.Run(() => FillTemplate(fileInfo.FullName, Path.Combine(fileInfo.DirectoryName, "SNOApplications", chatId.ToString()) + ".docx", replaceList));
					return $"{CheckMarkEmj} Успешно добавлено!";
				}
				finally
				{
					semaphoreSlim.Release();
				}
			}
			return $"{AlertEmj} Создать файл не удалось!";
		}

		public async Task<string> FillSMUDataAsync(string lowerCaseMessage, long chatId, string fileFullPath, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
			{
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			}
			if (File.Exists(fileFullPath))
			{
				fileInfo = new FileInfo(fileFullPath);
			}
			if (chatId <= 0) throw new ArgumentOutOfRangeException("chatId out of range");

			if (token.IsCancellationRequested) token.ThrowIfCancellationRequested();

			lowerCaseMessage = lowerCaseMessage.Replace("/smuapp/", "");
			var dataArr = lowerCaseMessage.Split('/');

			// Получение ФИО с большой буквы (родительный падеж)
			var fioRArr = dataArr[1].Split(" ");
			string fioRStr = string.Join(" ", fioRArr.Select(str => char.ToUpper(str[0]) + str.Substring(1)));

			var replaceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("<FROM>", dataArr[0]),
			new KeyValuePair<string, string>("<FIORP>", fioRStr),
			new KeyValuePair<string, string>("<STRUCT>", dataArr[2].ToUpper()),
			new KeyValuePair<string, string>("<ISACADEMIC>", dataArr[3]),
			new KeyValuePair<string, string>("<BIRTHDATE>", dataArr[4]),
			new KeyValuePair<string, string>("<NUMB>", dataArr[5]),
			new KeyValuePair<string, string>("<MAIL>", dataArr[6]),
			new KeyValuePair<string, string>("<DAY>", DateTime.Now.Day.ToString()),
			new KeyValuePair<string, string>("<MONTH>", DateTime.Now.Month.ToString()),
			new KeyValuePair<string, string>("<YEAR>", DateTime.Now.Year.ToString())
		};

			if (!ValidateData(replaceList)) return $"{AlertEmj} Неверно указаны данные.";

			if (fileInfo != null && !string.IsNullOrWhiteSpace(fileInfo.DirectoryName))
			{
				await semaphoreSlim.WaitAsync();
				try
				{
					if (DirectorySize(new DirectoryInfo(Path.Combine(fileInfo.DirectoryName, "SMUApplications")), maxSize) > maxSize)
					{
						using var scope = serviceProvider.CreateScope();

						var notifyService = scope.ServiceProvider.GetRequiredService<INotifyService>();
						await notifyService.NotifyAdminsAsync($"{AlertEmj} Внимание, обработайте заявления на вступление в СМУ!\nПрием заявлений временно остановлен. Требуется освободить память.", token);
						return $"{BlueCircleEmj} Сервер занят обработкой предыдущих заявлений.\nПожалуйста, повторите попытку позднее.";
					}

					await Task.Run(() => FillTemplate(fileInfo.FullName, Path.Combine(fileInfo.DirectoryName, "SMUApplications", chatId.ToString()) + ".docx", replaceList));
					return $"{CheckMarkEmj} Успешно добавлено!";
				}
				finally
				{
					semaphoreSlim.Release();
				}
			}
			return $"{AlertEmj} Создать файл не удалось!";
		}

		private bool ValidateData(List<KeyValuePair<string, string>> replaceList)
		{
			// Пример проверки данных
			var phone = replaceList.FirstOrDefault(kv => kv.Key == "<NUMB>").Value;
			if (!phone.Contains("+7") || phone.Length != 12) return false;

			var email = replaceList.FirstOrDefault(kv => kv.Key == "<MAIL>").Value;
			if (!Regex.IsMatch(email, @"^[\w-.]+@([\w-]+.)+[\w-]{2,4}$")) return false;

			return true;
		}

		private void FillTemplate(string templatePath, string outputPath, List<KeyValuePair<string, string>> replaceList)
		{
			// Открываем шаблонный документ
			using var document = WordprocessingDocument.Open(templatePath, false); // Открываем в режиме только для чтения
			var clonedDocument = (WordprocessingDocument)document.Clone(outputPath, true); // Клонируем документ для записи
			var body = clonedDocument.MainDocumentPart.Document.Body;

			// Преобразуем список замен в словарь для быстрого доступа
			var replaceDict = replaceList.ToDictionary(kv => kv.Key, kv => kv.Value);

			// Используем регулярное выражение для поиска маркеров в угловых скобках
			var regex = new Regex(@"<\b[A-Z]+\b>");

			// Находим все абзацы, содержащие маркеры
			var paragraphsWithMarks = body.Descendants<Paragraph>().Where(p => regex.IsMatch(p.InnerText)).ToList();

			// Применяем замены
			foreach (var paragraph in paragraphsWithMarks)
			{
				var paragraphText = paragraph.InnerText;
				foreach (Match match in regex.Matches(paragraphText))
				{
					var markValue = match.Value;
					if (replaceDict.TryGetValue(markValue, out var replacementValue))
					{
						paragraphText = paragraphText.Replace(markValue, replacementValue);
					}
				}

				// Создаем новый Run элемент и копируем форматирование
				var newRun = new Run(new Text(paragraphText));

				if (paragraph.GetFirstChild<ParagraphProperties>() == null)
				{
					paragraph.PrependChild(new ParagraphProperties(
						new ParagraphStyleId { Val = "Normal" }, // Устанавливаем стиль "Normal"
						new Justification { Val = JustificationValues.Left } // Выравнивание по левому краю
					));
				}

				// Применяем форматирование Times New Roman и размер 14 к абзацу
				var runProperties = new RunProperties(
					new RunFonts { Ascii = "Times New Roman" }, // Устанавливаем шрифт Times New Roman
					new FontSize { Val = "28" } // Размер шрифта 14 (14 * 2 Half-Points)
				);
				newRun.PrependChild(runProperties);

				// Обновляем текст абзаца
				paragraph.RemoveAllChildren<Run>();
				paragraph.AppendChild(newRun);
			}

			// Сохраняем изменения в клонированном документе
			clonedDocument.MainDocumentPart.Document.Save();
			clonedDocument.Dispose(); // Закрываем клонированный документ
			document.Dispose(); // Закрываем оригинальный документ
		}

		private long DirectorySize(DirectoryInfo dir, long limit)
		{
			if (dir is null)
			{
				throw new ArgumentNullException(nameof(dir));
			}

			if (limit < 0) return long.MaxValue;

			long size = 0;
			// Add file sizes.
			FileInfo[] fis = dir.GetFiles();
			foreach (FileInfo fi in fis)
			{
				size += fi.Length;
				if (size > limit)
					return size;
			}
			// Add subdirectory sizes.
			DirectoryInfo[] dis = dir.GetDirectories();
			foreach (DirectoryInfo di in dis)
			{
				size += DirectorySize(di, limit);
				if (size > limit)
					return size;
			}
			return size;
		}
	}
}
