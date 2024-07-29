using Domain.Entities;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace DiplomProject.Server.Controllers
{
	[ApiController]
	[Route("api/usercreatedevents")]
	public class UserCreatedEventController : ControllerBase
	{
		private readonly IUserCreatedEventRepository _userCreatedEventsRepo;
		private readonly ILogger<UserCreatedEventController> _logger;

		public UserCreatedEventController(IUserCreatedEventRepository repo, ILogger<UserCreatedEventController> logger)
		{
			_userCreatedEventsRepo = repo ?? throw new ArgumentNullException(nameof(repo)); ;
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
				await _userCreatedEventsRepo.UpdateUserCreatedEventAsync(updatedEvent, token);
				return Ok();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
		[HttpDelete("delete")]
		public async Task<ActionResult> DeleteUserCreatedEvent([FromQuery] Guid Id, CancellationToken token)
		{
			try
			{
				await _userCreatedEventsRepo.DeleteUserCreatedEventByIdAsync(Id, token);
				return Ok();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
	}
}
