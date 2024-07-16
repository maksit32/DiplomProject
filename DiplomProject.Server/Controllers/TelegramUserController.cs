using Domain.Entities;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DiplomProject.Server.Controllers
{
	[ApiController]
	[Route("api/tgusers")]
	public class TelegramUserController : ControllerBase
	{
		private readonly ITelegramUserRepository _tgusersRepo;
		private readonly ILogger<TelegramUserController> _logger;

		public TelegramUserController(ITelegramUserRepository repo, ILogger<TelegramUserController> logger)
		{
			_tgusersRepo = repo;
			_logger = logger;
		}

		[HttpGet("get")]
		public async Task<ActionResult<List<TelegramUser>>> GetTgUsers(CancellationToken token)
		{
			try
			{
				var order = await _tgusersRepo.GetUsersListAsync(token);
				return order;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
	}
}
