using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace DiplomProject.Server.Controllers
{
	[ApiController]
	[Route("api/tgusers")]
	public class TelegramUserController : ControllerBase
	{
		private readonly ITelegramUserRepository _tgusersRepo;
		private readonly ILogger<TelegramUserController> _logger;
		private readonly IPasswordHasherService _passwordHasherService;

		public TelegramUserController(ITelegramUserRepository repo, IPasswordHasherService passwordHasherService, ILogger<TelegramUserController> logger)
		{
			_tgusersRepo = repo ?? throw new ArgumentNullException(nameof(repo));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_passwordHasherService = passwordHasherService ?? throw new ArgumentNullException(nameof(passwordHasherService)); 
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
		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!ВНИМАНИЕ. Добавление с React пользователя. Нужно хешировать пароль на сервере!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		[HttpPost("add")]
		public async Task<ActionResult> AddTgUser([FromBody] TelegramUser user, CancellationToken token)
		{
			try
			{
				var hashedPass = _passwordHasherService.HashPassword(user.HashedPassword);
				user.HashedPassword = hashedPass;

				await _tgusersRepo.AddTgUserAsync(user, token);
				return Created();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
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
		[HttpPut("update_sub")]
		public async Task<ActionResult> UpdateSubTgUser([FromQuery] Guid Id, [FromBody] bool subStatus, CancellationToken token)
		{
			try
			{
				await _tgusersRepo.UpdateSubStatusTgUserAsync(Id, subStatus, token);
				return Ok();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
		[HttpPut("update_adm")]
		public async Task<ActionResult> UpdateAdmTgUser([FromQuery] Guid Id, [FromBody] bool admStatus, CancellationToken token)
		{
			try
			{
				await _tgusersRepo.UpdateAdminStatusTgUserAsync(Id, admStatus, token);
				return Ok();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
	}
}