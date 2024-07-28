using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace DiplomProject.Server.Controllers
{
	[ApiController]
	[Route("api/scienceevents")]
	public class ScienceEventController : ControllerBase
	{
		private readonly IScienceEventRepository _scEventsRepo;
		private readonly ILogger<ScienceEventController> _logger;
		private readonly INotifyService _notificationService;

		public ScienceEventController(IScienceEventRepository repo, ILogger<ScienceEventController> logger, INotifyService notifyService)
		{
			_scEventsRepo = repo;
			_logger = logger;
			_notificationService = notifyService;
		}

		[HttpGet("get")]
		public async Task<ActionResult<List<ScienceEvent>>> GetScienceEvents(CancellationToken token)
		{
			try
			{
				var lstScienceEvents = await _scEventsRepo.ReadAllEventsAsync(token);
				return lstScienceEvents;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpGet("get_actual_scienceevents")]
		public async Task<ActionResult<List<ScienceEvent>>> GetActualScienceEvents(CancellationToken token)
		{
			try
			{
				var lstScienceEvents = await _scEventsRepo.ReadAllActualEventsAsync(token);
				return lstScienceEvents;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpGet("id")]
		public async Task<ActionResult<ScienceEvent>> GetScienceEventById([FromQuery] Guid id, CancellationToken token)
		{
			try
			{
				var scEvent = await _scEventsRepo.GetScienceEventById(id, token);
				if (scEvent == null) return NotFound();

				return scEvent;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		[HttpPut("update")]
		public async Task<ActionResult> UpdateScienceEvent([FromBody] ScienceEvent sEvent, CancellationToken token)
		{
			try
			{
				await _scEventsRepo.UpdateFullEventAsync(sEvent, token);
				return Ok();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
		[HttpPost("add")]
		public async Task<ActionResult> AddScienceEvent([FromBody] ScienceEvent newEvent, CancellationToken token)
		{
			try
			{
				await _scEventsRepo.AddEventAsync(newEvent, token);
				return Created();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
		[HttpDelete("delete")]
		public async Task<ActionResult> DeleteScienceEvent([FromQuery] Guid id, CancellationToken token)
		{
			try
			{
				await _scEventsRepo.DeleteEventByIdAsync(id, token);
				return Ok();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
	}
}
