using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;


namespace DiplomProject.Server.Services
{
	public class WordDocumentService : IDocumentService
	{
		public List<string?> GetWordList(string folderPath)
		{
			if (string.IsNullOrWhiteSpace(folderPath)) throw new ArgumentNullException(nameof(folderPath));
			if (!Directory.Exists(folderPath))
				throw new ArgumentNullException(nameof(folderPath));

			var wordFiles = Directory.GetFiles(folderPath, "*.docx");
			var wordFileNames = wordFiles.Select(Path.GetFileName).ToList();
			return wordFileNames;
		}
		public async Task<byte[]> GetWordFile(string folderPath, string fileName)
		{
			if (string.IsNullOrWhiteSpace(folderPath)) throw new ArgumentNullException(nameof(folderPath));
			if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

			var filePath = Path.Combine(folderPath, fileName);

			if (!System.IO.File.Exists(filePath))
				throw new FileNotFoundException("File not found", filePath);

			using var memory = new MemoryStream();
			using var stream = new FileStream(filePath, FileMode.Open);
			await stream.CopyToAsync(memory);

			return memory.ToArray();
		}
		public async Task<bool> UploadWordFile(IFormFile file, string folderPath)
		{
			if(string.IsNullOrWhiteSpace(folderPath)) throw new ArgumentNullException(nameof(folderPath));
			if (file == null || file.Length == 0)
				throw new ArgumentNullException(nameof(file));

			if (!Directory.Exists(folderPath))
				Directory.CreateDirectory(folderPath);

			var filePath = Path.Combine(folderPath, file.FileName);

			using var stream = new FileStream(filePath, FileMode.Create);
			await file.CopyToAsync(stream);
			return true;
		}
	}
}
