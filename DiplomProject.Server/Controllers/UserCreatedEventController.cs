using DiplomProject.Server.Services;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DiplomProject.Server.Controllers
{
	[ApiController]
	//[Authorize]
	[Route("api/usercreatedevents")]
	public class UserCreatedEventController : ControllerBase
	{
		private readonly IUserCreatedEventRepository _userCreatedEventsRepo;
		private readonly ILogger<UserCreatedEventController> _logger;
		private readonly IValidationService _validationService;

		public UserCreatedEventController(IUserCreatedEventRepository userCreatedEvRepo, IValidationService validationService, ILogger<UserCreatedEventController> logger)
		{
			_userCreatedEventsRepo = userCreatedEvRepo ?? throw new ArgumentNullException(nameof(userCreatedEvRepo)); ;
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
		}

		[HttpGet("get")]
		public async Task<ActionResult<List<UserCreatedEvent>>> GetUserCreatedEvents(CancellationToken token)
		{
			try
			{
				var lstEvents = await _userCreatedEventsRepo.ReadAllEventsAsync(token);
				return lstEvents;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpGet("id")]
		public async Task<ActionResult<List<UserCreatedEvent>>> GetUserCreatedEventsById([FromQuery] Guid Id, CancellationToken token)
		{
			try
			{
				var lstEvents = await _userCreatedEventsRepo.ReadAllUserEventsAsync(Id, token);
				return lstEvents;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpPut("update")]
		public async Task<ActionResult> UpdateUserCreatedEvent([FromBody] UserCreatedEvent updatedEvent, CancellationToken token)
		{
			try
			{
				_validationService.ValidateUserCreatedEvent(updatedEvent, token);
				await _userCreatedEventsRepo.UpdateUserCreatedEventAsync(updatedEvent, token);
				return Ok();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
		[HttpDelete("delete")]
		public async Task<ActionResult> DeleteUserCreatedEvent([FromQuery] Guid id, CancellationToken token)
		{
			try
			{
				var deleteEvent = await _userCreatedEventsRepo.GetUserCreatedEventByIdAsync(id, token);
				await _userCreatedEventsRepo.DeleteUserCreatedEvent(deleteEvent, token);
				return Ok();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
	}
}
