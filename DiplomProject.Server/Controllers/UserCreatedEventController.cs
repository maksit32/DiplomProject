using DiplomProject.Server.Services;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DiplomProject.Server.Controllers
{
	[ApiController]
	//[Authorize("Jwt")]
	[Route("api/usercreatedevents")]
	public class UserCreatedEventController : ControllerBase
	{
		private readonly IUserCreatedEventRepository _userCreatedEventsRepo;
		private readonly ITelegramUserRepository _tgUserRepository;
		private readonly ILogger<UserCreatedEventController> _logger;
		private readonly IValidationService _validationService;
		private readonly IDtoConverterService _dtoConverterService;

		public UserCreatedEventController(IUserCreatedEventRepository userCreatedEvRepo, IValidationService validationService, 
			ILogger<UserCreatedEventController> logger, IDtoConverterService dtoConverterService, ITelegramUserRepository tgUserRepository)
		{
			_userCreatedEventsRepo = userCreatedEvRepo ?? throw new ArgumentNullException(nameof(userCreatedEvRepo)); ;
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
			_dtoConverterService = dtoConverterService ?? throw new ArgumentNullException(nameof(dtoConverterService));
			_tgUserRepository = tgUserRepository ?? throw new ArgumentNullException(nameof(tgUserRepository));
		}

		[HttpGet("get")]
		public async Task<ActionResult<List<UserCreatedEventDto>>> GetUserCreatedEvents(CancellationToken token)
		{
			try
			{
				var lstEvents = await _userCreatedEventsRepo.ReadAllEventsAsync(token);
				var dtoLst = await _dtoConverterService.ConvertToUserCreatedEventDtoList(lstEvents, token);
				return dtoLst;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpGet("id")]
		public async Task<ActionResult<List<UserCreatedEventDto>>> GetUserCreatedEventsByUserId([FromQuery] long chatId, CancellationToken token)
		{
			try
			{
				var tgUser = await _tgUserRepository.GetTgUserByIdAsync(chatId, token);
				var lstEvents = await _userCreatedEventsRepo.ReadAllUserEventsAsync(tgUser, token);
				var dtoLst = await _dtoConverterService.ConvertToUserCreatedEventDtoList(lstEvents, token);
				return dtoLst;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpPut("update")]
		public async Task<ActionResult> UpdateUserCreatedEvent([FromBody] UserCreatedEventDto eventDto, CancellationToken token)
		{
			try
			{
				var updatedEvent = await _dtoConverterService.ConvertToUsCrEventFromUpdatedDto(eventDto, token);
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
