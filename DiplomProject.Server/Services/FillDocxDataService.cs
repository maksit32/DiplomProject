using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Domain.Services.Interfaces;
using static Domain.Constants.EmojiConstants;

//add Microsoft Word 16 dependency
using Word = Microsoft.Office.Interop.Word;




namespace MSTUCABOT.ConsoleServer.Services.Classes
{
	//only .docx type
	public class FillDocxDataService : IFillDataService
	{
		private INotifyService notifyService;
		private FileInfo fileInfo = null!;
		//защита от dataRace
		private SemaphoreSlim semaphoreSlim = null!;
		//maxSize папок с заявлениями СНО и СМУ
		//СМУ + СНО документы весят = 5655944 бит
		private long maxSize = 150000000; //бит (примерно 50 заявлений, далее их нужно переносить, либо удалять)

		public FillDocxDataService(string fileFullPath, INotifyService notifyService, SemaphoreSlim semaphore)
		{
			if (string.IsNullOrWhiteSpace(fileFullPath))
			{
				throw new ArgumentException($"\"{nameof(fileFullPath)}\" не может быть пустым или содержать только пробел.", nameof(fileFullPath));
			}

			//если файл есть
			if (File.Exists(fileFullPath))
			{
				fileInfo = new FileInfo(fileFullPath);
			}
			else
			{
				throw new ArgumentException("File not found!");
			}

			semaphoreSlim = semaphore ?? throw new ArgumentNullException(nameof(semaphore));
			this.notifyService = notifyService ?? throw new ArgumentNullException(nameof(notifyService));
		}

		//format:  "<zone>", "data"
		public async Task<string> FillSNODataAsync(string lowerCaseMessage, long chatId, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
			{
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			}

			if (chatId <= 0) throw new ArgumentOutOfRangeException("chatId out of range");

			lowerCaseMessage = lowerCaseMessage.Replace("/snoapp/", "");
			var dataArr = lowerCaseMessage.Split('/');

			//получение ФИО с большой буквы
			var fioArr = dataArr[0].Split(" ");
			string fioStr = "";
			foreach ( var str in fioArr )
			{
				fioStr += char.ToUpper(str[0]) + str.Substring(1) + " ";
			}

			//From (ex Иванова Ивана Ивановича)
			KeyValuePair<string, string> kv1 = new KeyValuePair<string, string>("<FROM>", fioStr);
			//Facultie
			KeyValuePair<string, string> kv2 = new KeyValuePair<string, string>("<FAT>", dataArr[1].ToUpper());
			//Group
			KeyValuePair<string, string> kv3 = new KeyValuePair<string, string>("<GROUP>", dataArr[2].ToUpper());
			//Number
			KeyValuePair<string, string> kv4 = new KeyValuePair<string, string>("<NUMB>", dataArr[3]);
			if(!kv4.Value.Contains("+7")) return $"{AlertEmj}Неверно указанный номер!\nНомер должен начинаться с +7";
			if(kv4.Value.Length != 12) return $"{AlertEmj}Неверно указан номер!\nНомер должен состоять из 11 цифр";
			//Email
			KeyValuePair<string, string> kv5 = new KeyValuePair<string, string>("<MAIL>", dataArr[4]);
			if (!kv5.Value.Contains("@")) return $"{AlertEmj}Неверно указана почта.";
			//Day
			KeyValuePair<string, string> kv6 = new KeyValuePair<string, string>("<DAY>", dataArr[5]);
			if (long.Parse(kv6.Value) != DateTime.Now.Day) return $"{AlertEmj}Неверно указанный день!";
			//Month
			KeyValuePair<string, string> kv7 = new KeyValuePair<string, string>("<MONTH>", dataArr[6]);
			if (long.Parse(kv7.Value) != DateTime.Now.Month) return $"{AlertEmj}Неверно указанный месяц!";
			//Year
			KeyValuePair<string, string> kv8 = new KeyValuePair<string, string>("<YEAR>", dataArr[7]);
			if (long.Parse(kv8.Value) != DateTime.Now.Year) return $"{AlertEmj}Неверно указанный год!";

			IReadOnlyList<KeyValuePair<string, string>> replaceList = new List<KeyValuePair<string, string>>()
			{
				kv1, kv2, kv3, kv4, kv5, kv6, kv7, kv8
			};

			if (fileInfo != null && !string.IsNullOrWhiteSpace(fileInfo.DirectoryName))
			{
				semaphoreSlim.Wait();
				Word.Application app = null!;

				if (DirectorySize(new DirectoryInfo(fileInfo.DirectoryName + "\\SNOApplications"), maxSize) > maxSize)
				{
					notifyService.NotifyAdminsAsync($"{AlertEmj}Внимание, обработайте заявления на вступление в СНО!\nПрием заявлений временно остановлен. Требуется освободить память.", token);
					semaphoreSlim.Release();
					return $"{BlueCircleEmj} Сервер занят обработкой предыдущих заявлений.\nПожалуйста, повторите попытку позднее.";
				}

				try
				{
					await Task.Run(() =>
					{
						app = new Word.Application();
						Object file = fileInfo.FullName;
						Object missing = Type.Missing;

						app.Documents.Open(fileInfo.FullName);

						//findAndReplace
						foreach (var item in replaceList)
						{
							//объект для поиска
							Word.Find find = app.Selection.Find;
							//что меняем
							find.Text = item.Key;
							//на что меняем
							find.Replacement.Text = item.Value;

							Object wrap = Word.WdFindWrap.wdFindContinue;
							Object replace = Word.WdReplace.wdReplaceAll;

							//поиск
							find.Execute(FindText: Type.Missing,
								MatchCase: false,
								MatchWholeWord: false,
								MatchWildcards: false,
								MatchSoundsLike: missing,
								MatchAllWordForms: false,
								Forward: true,
								Wrap: wrap,
								Format: false,
								ReplaceWith: missing, Replace: replace);
						}

						//новое местоположение (a\b\id)
						Object newFileName = Path.Combine(fileInfo.DirectoryName, "SNOApplications" , chatId.ToString());
						app.ActiveDocument.SaveAs2(newFileName);

						//closing
						app.ActiveDocument.Close();
					});
				}
				finally
				{
					if (app != null) app.Quit();
					semaphoreSlim.Release();
				}
				return $"{CheckMarkEmj}Успешно добавлено!";
			}
			return $"{AlertEmj}Создать файл не удалось!";
		}

		public async Task<string> FillSMUDataAsync(string lowerCaseMessage, long chatId, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
			{
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			}

			if (chatId <= 0) throw new ArgumentOutOfRangeException("chatId out of range");

			lowerCaseMessage = lowerCaseMessage.Replace("/smuapp/", "");
			var dataArr = lowerCaseMessage.Split('/');

			//получение отдельно ФИО с большой буквы (родительный падеж)
			var fioRArr = dataArr[0].Split(" ");
			string fioRStr = "";
			foreach (var str in fioRArr)
			{
				fioRStr += char.ToUpper(str[0]) + str.Substring(1) + " ";
			}

			//From (ex Иванова Ивана Ивановича) родительный падеж
			KeyValuePair<string, string> kv1 = new KeyValuePair<string, string>("<FROM>", fioRStr);
			//Structure
			KeyValuePair<string, string> kv2 = new KeyValuePair<string, string>("<STRUCT>", dataArr[1].ToUpper());
			//FIO родительный падеж (копия)
			KeyValuePair<string, string> kv3 = new KeyValuePair<string, string>("<FIORP>", fioRStr);
			//IsAcedemic
			KeyValuePair<string, string> kv4 = new KeyValuePair<string, string>("<ISACADEMIC>", dataArr[2]);
			//Birthday
			KeyValuePair<string, string> kv5 = new KeyValuePair<string, string>("<BIRTHDATE>", dataArr[3]);
			var match = Regex.IsMatch(kv5.Value, @"^(0[1-9]|[12][0-9]|3[01]).(0[1-9]|1[0-2]).\d{4}$");
			if(!match) return $"{AlertEmj}Неверно указана дата рождения. Пример: dd.mm.yyyy";
			//Number
			KeyValuePair<string, string> kv6 = new KeyValuePair<string, string>("<NUMBER>", dataArr[4]);
			if (!kv6.Value.Contains("+7")) return $"{AlertEmj}Неверно указанный номер!\nНомер должен начинаться с +7";
			if (kv6.Value.Length != 12) return $"{AlertEmj}Неверно указан номер!\nНомер должен состоять из 11 цифр";
			//Email
			KeyValuePair<string, string> kv7 = new KeyValuePair<string, string>("<MAIL>", dataArr[5]);
			var match2 = Regex.IsMatch(kv7.Value, @"^[\w-.]+@([\w-]+.)+[\w-]{2,4}$");
			if (!match2) return $"{AlertEmj}Неверно указана почта.";
			if (!kv7.Value.Contains("@")) return $"{AlertEmj}Неверно указана почта.";
			//Day
			KeyValuePair<string, string> kv8 = new KeyValuePair<string, string>("<DAY>", dataArr[6]);
			if (long.Parse(kv8.Value) != DateTime.Now.Day) return $"{AlertEmj}Неверно указанный день!";
			//Month
			KeyValuePair<string, string> kv9 = new KeyValuePair<string, string>("<MONTH>", dataArr[7]);
			if (long.Parse(kv9.Value) != DateTime.Now.Month) return $"{AlertEmj}Неверно указанный месяц!";
			//Year
			KeyValuePair<string, string> kv10 = new KeyValuePair<string, string>("<YEAR>", dataArr[8]);
			if (long.Parse(kv10.Value) != DateTime.Now.Year) return $"{AlertEmj}Неверно указанный год!";

			IReadOnlyList<KeyValuePair<string, string>> replaceList = new List<KeyValuePair<string, string>>()
			{
				kv1, kv2, kv3, kv4, kv5, kv6, kv7, kv8, kv9, kv10
			};

			if (fileInfo != null && !string.IsNullOrWhiteSpace(fileInfo.DirectoryName))
			{
				semaphoreSlim.Wait();
				Word.Application app = null!;

				if (DirectorySize(new DirectoryInfo(fileInfo.DirectoryName + "\\SMUApplications"), maxSize) > maxSize)
				{
					notifyService.NotifyAdminsAsync($"{AlertEmj}Внимание, обработайте заявления на вступление в СМУ!\nПрием заявлений временно остановлен. Требуется освободить память.", token);
					semaphoreSlim.Release();
					return $"{BlueCircleEmj} Сервер занят обработкой предыдущих заявлений.\nПожалуйста, повторите попытку позднее.";
				}

				try
				{
					await Task.Run(() =>
					{
						app = new Word.Application();
						Object file = fileInfo.FullName;
						Object missing = Type.Missing;

						app.Documents.Open(fileInfo.FullName);

						//findAndReplace
						foreach (var item in replaceList)
						{
							//объект для поиска
							Word.Find find = app.Selection.Find;
							//что меняем
							find.Text = item.Key;
							//на что меняем
							find.Replacement.Text = item.Value;

							Object wrap = Word.WdFindWrap.wdFindContinue;
							Object replace = Word.WdReplace.wdReplaceAll;

							//поиск
							find.Execute(FindText: Type.Missing,
								MatchCase: false,
								MatchWholeWord: false,
								MatchWildcards: false,
								MatchSoundsLike: missing,
								MatchAllWordForms: false,
								Forward: true,
								Wrap: wrap,
								Format: false,
								ReplaceWith: missing, Replace: replace);
						}

						//новое местоположение (a\b\id)
						Object newFileName = Path.Combine(fileInfo.DirectoryName, "SMUApplications", chatId.ToString());
						app.ActiveDocument.SaveAs2(newFileName);

						//closing
						app.ActiveDocument.Close();
					});
				}
				finally
				{
					if (app != null) app.Quit();
					semaphoreSlim.Release();
				}
				return $"{CheckMarkEmj}Успешно добавлено!";
			}
			return $"{AlertEmj}Создать файл не удалось!";
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
			return (size);
		}
	}
}