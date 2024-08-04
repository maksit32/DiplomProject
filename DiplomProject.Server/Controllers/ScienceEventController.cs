﻿using DiplomProject.Server.Services;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static Domain.Constants.TelegramTextConstants;


namespace DiplomProject.Server.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/scienceevents")]
	public class ScienceEventController : ControllerBase
	{
		private readonly IScienceEventRepository _scEventsRepo;
		private readonly ILogger<ScienceEventController> _logger;
		private readonly INotifyService _notificationService;

		public ScienceEventController(IScienceEventRepository scEvRepo, ILogger<ScienceEventController> logger, INotifyService notifyService)
		{
			_scEventsRepo = scEvRepo ?? throw new ArgumentNullException(nameof(scEvRepo));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_notificationService = notifyService ?? throw new ArgumentNullException(nameof(notifyService));
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
				var scEvent = await _scEventsRepo.GetScienceEventByIdAsync(id, token);
				if (scEvent == null) return NotFound();

				return scEvent;
			}
			catch (Exception)
			{
				return NotFound();
			}
		}
		//валидация нужна
		[HttpPut("update")]
		public async Task<ActionResult> UpdateScienceEvent([FromBody] ScienceEvent updatedEv, CancellationToken token)
		{
			try
			{
				await _scEventsRepo.UpdateFullEventAsync(updatedEv, token);
				await _notificationService.NotifyEventChangingUsersAsync(updatedEv, ChangeEventNotification, token);
				return Ok();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
		//валидация нужна
		[HttpPost("add")]
		public async Task<ActionResult> AddScienceEvent([FromBody] ScienceEvent newEvent, CancellationToken token)
		{
			try
			{
				await _scEventsRepo.AddEventAsync(newEvent, token);
				await _notificationService.NotifyLastAddEventUsersAsync(NewEventNotification, token);
				return Created();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
		[HttpDelete("delete")]
		public async Task<ActionResult> DeleteScienceEvent([FromQuery] ScienceEvent scEvent, CancellationToken token)
		{
			try
			{
				await _scEventsRepo.DeleteEventAsync(scEvent, token);
				await _notificationService.NotifyEventChangingUsersAsync(scEvent, DeleteEventNotification, token);
				return Ok();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
	}
}
