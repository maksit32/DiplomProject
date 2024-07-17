using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
	public class BotConfiguration
	{
		public string BotToken { get; init; } = default!;
		public Uri BotWebhookUrl { get; init; } = default!;
		public string SecretToken { get; init; } = default!;
	}
}
