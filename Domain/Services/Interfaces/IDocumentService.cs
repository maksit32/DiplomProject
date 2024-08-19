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
		List<string?> GetFilesList(string filesFolderPath);
		Task<byte[]> GetFile(string folderPath, string fileName);
		Task<bool> UploadFile(IFormFile file, string folderPath);
		bool DeleteFile(string folderPath, string fileName);
	}
}
