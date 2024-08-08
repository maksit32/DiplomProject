using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Services.Interfaces
{
	public interface IDocumentService
	{
		List<string?> GetWordList(string wordFolderPath);
		Task<byte[]> GetWordFile(string folderPath, string fileName);
		Task<bool> UploadWordFile(IFormFile file, string folderPath);
	}
}
