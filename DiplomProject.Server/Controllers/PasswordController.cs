using Domain.Entities;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace DiplomProject.Server.Controllers
{
	[ApiController]
	[Route("api/orders")]
	public class PasswordController : ControllerBase
	{
		private readonly IPasswordRepostitory passwordRepo;
		private readonly ILogger<PasswordController> _logger;

		public PasswordController(IPasswordRepostitory repo, ILogger<PasswordController> logger)
		{
			passwordRepo = repo;
			_logger = logger;
		}

		[HttpGet("get")]
		public async Task<ActionResult<string>> GetOrder([FromQuery] long id, CancellationToken token)
		{
			try
			{
				var order = await passwordRepo.GetPasswordsByChatIdAsync(id, token);
				return order;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
	}
}
