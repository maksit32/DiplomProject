using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace DiplomProject.Server.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/tgusers")]
	public class TelegramUserController : ControllerBase
	{
		private readonly ITelegramUserRepository _tgusersRepo;
		private readonly ILogger<TelegramUserController> _logger;

		public TelegramUserController(ITelegramUserRepository tgUserRepo, ILogger<TelegramUserController> logger)
		{
			_tgusersRepo = tgUserRepo ?? throw new ArgumentNullException(nameof(tgUserRepo));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet("get")]
		public async Task<ActionResult<List<TelegramUser>>> GetTgUsers(CancellationToken token)
		{
			try
			{
				var lstTgUsers = await _tgusersRepo.GetUsersListAsync(token);
				return lstTgUsers;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpGet("get_admins")]
		public async Task<ActionResult<List<TelegramUser>>> GetAdminsTgUsers(CancellationToken token)
		{
			try
			{
				var lstTgUsers = await _tgusersRepo.GetAdminUsersListAsync(token);
				return lstTgUsers;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpGet("get_subusers")]
		public async Task<ActionResult<List<TelegramUser>>> GetSubTgUsers(CancellationToken token)
		{
			try
			{
				var lstTgUsers = await _tgusersRepo.GetSubUsersListAsync(token);
				return lstTgUsers;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpGet("id")]
		public async Task<ActionResult<TelegramUser>> GetTgUserById([FromQuery] Guid id, CancellationToken token)
		{
			try
			{
				var tgUser = await _tgusersRepo.GetTgUserByIdAsync(id, token);
				if (tgUser == null) return NotFound();

				return tgUser;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpGet("chatid")]
		public async Task<ActionResult<TelegramUser>> GetTgUserByChatId([FromQuery] long chatId, CancellationToken token)
		{
			try
			{
				var tgUser = await _tgusersRepo.GetTgUserByIdAsync(chatId, token);
				if (tgUser == null) return NotFound();

				return tgUser;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpDelete("delete")]
		public async Task<ActionResult> DeleteTgUser([FromQuery] Guid id, CancellationToken token)
		{
			try
			{
				var res = await _tgusersRepo.DeleteTgUserByIdAsync(id, token);
				if (!res) return NotFound();
				return Ok();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
		//валидация нужна
		[HttpPut("update")]
		public async Task<ActionResult> UpdateTgUser([FromBody] TelegramUser user, CancellationToken token)
		{
			try
			{
				await _tgusersRepo.UpdateTgUserAsync(user, token);
				return Ok();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
	}
}