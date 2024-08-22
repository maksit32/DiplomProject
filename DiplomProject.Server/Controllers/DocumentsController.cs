using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DiplomProject.Server.Controllers
{
	[ApiController]
	[Authorize("Jwt")]
	[Route("api/[controller]")]
	public class DocumentsController : ControllerBase
	{
		private readonly IDocumentService _documentService;
		private readonly IDtoConverterService _dtoConverterService;
		private readonly IConfiguration _configuration;
		private readonly string _SNOFolderPath;
		private readonly string _SMUFolderPath;

		public DocumentsController(IConfiguration configuration, IDocumentService documentService, IDtoConverterService dtoConverterService)
		{
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_dtoConverterService = dtoConverterService ?? throw new ArgumentNullException(nameof(dtoConverterService));
			_documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
			_SNOFolderPath = _configuration["SNOFolderPath"] ?? throw new ArgumentNullException(nameof(_SNOFolderPath));
			_SMUFolderPath = _configuration["SMUFolderPath"] ?? throw new ArgumentNullException(nameof(_SMUFolderPath));
		}
		[HttpGet("sno")]
		public IActionResult GetSNOWordFiles(CancellationToken ct)
		{
			try
			{
				var wordFileNames = _documentService.GetFilesList(_SNOFolderPath);
				var dtoList = _dtoConverterService.ConvertToFileDtoList(wordFileNames, ct);
				return Ok(dtoList);
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpGet("smu")]
		public IActionResult GetSMUWordFiles(CancellationToken ct)
		{
			try
			{
				var wordFileNames = _documentService.GetFilesList(_SMUFolderPath);
				var dtoList = _dtoConverterService.ConvertToFileDtoList(wordFileNames, ct);
				return Ok(dtoList);
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
				var snoFile = await _documentService.GetFile(_SNOFolderPath, fileName);
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
				var smuFile = await _documentService.GetFile(_SMUFolderPath, fileName);
				return File(smuFile, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpDelete("sno")]
		public IActionResult DeleteSNODocument([FromQuery] string fileName)
		{
			try
			{
				_documentService.DeleteFile(_SNOFolderPath, fileName);
				return Ok();
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpDelete("smu")]
		public IActionResult DeleteSMUDocument([FromQuery] string fileName)
		{
			try
			{
				_documentService.DeleteFile(_SMUFolderPath, fileName);
				return Ok();
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
	}
}