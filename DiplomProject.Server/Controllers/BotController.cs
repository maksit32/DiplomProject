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


namespace DiplomProject.Server.Controllers
{
	[ApiController]
	[Route("/")]
	public class BotController(IOptions<BotConfiguration> Config) : ControllerBase
	{
		[HttpGet("setWebhook")]
		public async Task<string> SetWebHook([FromServices] ITelegramBotClient bot, CancellationToken ct)
		{
			var webhookUrl = Config.Value.BotWebhookUrl.AbsoluteUri;
			await bot.SetWebhookAsync(webhookUrl, allowedUpdates: [], secretToken: Config.Value.SecretToken, cancellationToken: ct);
			return $"Webhook set to {webhookUrl}";
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] Update update, [FromServices] ITelegramBotClient bot, [FromServices] UpdateHandler handleUpdateService, CancellationToken ct)
		{
			if (Request.Headers["X-Telegram-Bot-Api-Secret-Token"] != Config.Value.SecretToken)
				return Forbid();
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
		[HttpGet]
		public string Get()
		{
			return "Telegram bot was started";
		}
	}

}
