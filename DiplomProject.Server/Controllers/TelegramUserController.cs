﻿using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace DiplomProject.Server.Controllers
{
	[ApiController]
	//[Authorize]
	[Route("api/tgusers")]
	public class TelegramUserController : ControllerBase
	{
		private readonly ITelegramUserRepository _tgusersRepo;
		private readonly ILogger<TelegramUserController> _logger;
		private readonly IValidationService _validationService;
		private readonly ITgUserService _tgUserService;

		public TelegramUserController(ITelegramUserRepository tgUserRepo, IValidationService validationService,
			ILogger<TelegramUserController> logger, ITgUserService tgUserService)
		{
			_tgusersRepo = tgUserRepo ?? throw new ArgumentNullException(nameof(tgUserRepo));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
			_tgUserService = tgUserService ?? throw new ArgumentNullException(nameof(tgUserService));
		}

		[HttpGet("get")]
		public async Task<ActionResult<List<TelegramUserDto>>> GetTgUsers(CancellationToken token)
		{
			try
			{
				var lstTgUsers = await _tgusersRepo.GetUsersListAsync(token);
				var lstTgUserDTO = _tgUserService.ConvertToTelegramUserDtoList(lstTgUsers, token);
				return lstTgUserDTO;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpGet("get_admins")]
		public async Task<ActionResult<List<TelegramUserDto>>> GetAdminsTgUsers(CancellationToken token)
		{
			try
			{
				var lstTgUsers = await _tgusersRepo.GetAdminUsersListAsync(token);
				var lstTgUserDTO = _tgUserService.ConvertToTelegramUserDtoList(lstTgUsers, token);
				return lstTgUserDTO;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpGet("get_subusers")]
		public async Task<ActionResult<List<TelegramUserDto>>> GetSubTgUsers(CancellationToken token)
		{
			try
			{
				var lstTgUsers = await _tgusersRepo.GetSubUsersListAsync(token);
				var lstTgUserDTO = _tgUserService.ConvertToTelegramUserDtoList(lstTgUsers, token);
				return lstTgUserDTO;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpGet("id")]
		public async Task<ActionResult<TelegramUserDto>> GetTgUserById([FromQuery] Guid id, CancellationToken token)
		{
			try
			{
				var tgUser = await _tgusersRepo.GetTgUserByIdAsync(id, token);
				if (tgUser == null) return NotFound();

				var tgUserDto = _tgUserService.ConvertToTelegramUserDto(tgUser, token);
				return tgUserDto;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpGet("chatid")]
		public async Task<ActionResult<TelegramUserDto>> GetTgUserByChatId([FromQuery] long chatId, CancellationToken token)
		{
			try
			{
				var tgUser = await _tgusersRepo.GetTgUserByIdAsync(chatId, token);
				if (tgUser == null) return NotFound();

				var tgUserDto = _tgUserService.ConvertToTelegramUserDto(tgUser, token);
				return tgUserDto;
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
		[HttpPut("update")]
		public async Task<ActionResult> UpdateTgUser([FromBody] TelegramUserDto userDto, CancellationToken token)
		{
			try
			{
				var user = await _tgUserService.ConvertToTelegramUser(userDto, token);
				_validationService.ValidateTgUser(user, token);
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