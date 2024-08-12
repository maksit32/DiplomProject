using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomProject.Server.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class NotifyController : ControllerBase
	{
		private readonly INotifyService _notifyService;
		public NotifyController(INotifyService notifyService)
		{
			_notifyService = notifyService ?? throw new ArgumentNullException(nameof(notifyService));
		}
		[HttpPost("/sub")]
		public async Task<ActionResult> NotifySubUsers([FromBody] string notifyMessage, CancellationToken ct)
		{
			try
			{
				await _notifyService.NotifySubUsersAsync(notifyMessage, ct);
				return Ok();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
		[HttpPost("/adm")]
		public async Task<ActionResult> NotifyAdminUsers([FromBody] string notifyMessage, CancellationToken ct)
		{
			try
			{
				await _notifyService.NotifyAdminsAsync(notifyMessage, ct);
				return Ok();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
		[HttpPost("/all")]
		public async Task<ActionResult> NotifyAllUsers([FromBody] string notifyMessage, CancellationToken ct)
		{
			try
			{
				await _notifyService.NotifyAllUsersAsync(notifyMessage, ct);
				return Ok();
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
	}
}
