using DiplomProject.Server.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;
using Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace DiplomProject.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class BotController : ControllerBase
	{
		private readonly IOptions<BotConfiguration> _config;
		private readonly IPasswordHasherService _passwordHasherService;
		private readonly INotifyService _notificationService;

		public BotController(IOptions<BotConfiguration> config, IPasswordHasherService passwordHasherService, INotifyService notifyService)
		{
			_config = config ?? throw new ArgumentNullException(nameof(config));
			_passwordHasherService = passwordHasherService ?? throw new ArgumentNullException(nameof(passwordHasherService));
			_notificationService = notifyService ?? throw new ArgumentNullException(nameof(notifyService));
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] Update update, [FromServices] ITelegramBotClient bot, [FromServices] UpdateHandler handleUpdateService, CancellationToken ct)
		{
			try
			{
				await handleUpdateService.HandleUpdateAsync(bot, update, ct);
			}
			catch (Exception exception)
			{
				await handleUpdateService.HandlePollingErrorAsync(bot, exception, ct);
			}
			return Ok();
		}
	}
}
