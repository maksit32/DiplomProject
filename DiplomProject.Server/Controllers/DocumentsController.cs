using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DiplomProject.Server.Controllers
{
	[ApiController]
	//[Authorize("Jwt")]
	[Route("api/[controller]")]
	public class DocumentsController : ControllerBase
	{
		private readonly IDocumentService _documentService;
		private readonly IConfiguration _configuration;
		private readonly string _wordSNOFolderPath;
		private readonly string _wordSMUFolderPath;

		public DocumentsController(IConfiguration configuration, IDocumentService documentService)
		{
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
			_wordSNOFolderPath = _configuration["SNOFolderPath"] ?? throw new ArgumentNullException(nameof(_wordSNOFolderPath));
			_wordSMUFolderPath = _configuration["SMUFolderPath"] ?? throw new ArgumentNullException(nameof(_wordSMUFolderPath));
		}
		[HttpGet("sno")]
		public IActionResult GetSNOWordFiles()
		{
			try
			{
				var wordFileNames = _documentService.GetWordList(_wordSNOFolderPath);
				return Ok(wordFileNames);
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpGet("smu")]
		public IActionResult GetSMUWordFiles()
		{
			try
			{
				var wordFileNames = _documentService.GetWordList(_wordSMUFolderPath);
				return Ok(wordFileNames);
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpGet("sno/{fileName}")]
		public async Task<IActionResult> GetSNOWordFile(string fileName)
		{
			try
			{
				var snoFile = await _documentService.GetWordFile(_wordSNOFolderPath, fileName);
				return File(snoFile, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpGet("smu/{fileName}")]
		public async Task<IActionResult> GetSMUWordFile(string fileName)
		{
			try
			{
				var smuFile = await _documentService.GetWordFile(_wordSMUFolderPath, fileName);
				return File(smuFile, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpDelete("sno")]
		public IActionResult DeleteSNODocument([FromQuery]string fileName)
		{
			try
			{
				_documentService.DeleteWordFile(_wordSNOFolderPath, fileName);
				return Ok();
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpDelete("smu")]
		public IActionResult DeleteSMUDocument([FromQuery]string fileName)
		{
			try
			{
				_documentService.DeleteWordFile(_wordSMUFolderPath, fileName);
				return Ok();
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
	}
}