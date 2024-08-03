using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interfaces
{
	public interface IScienceEventService
	{
		Task<string> ReadAllActualEventsToStringAsync(CancellationToken token);
		Task<string> ReadAllActualEvAdminToStringAsync(CancellationToken token);
		ScienceEvent? CreateAddAdminEvent(TelegramUser user, string lowerCaseMessage, CancellationToken token);
		Task<ScienceEvent?> CreateUpdateAdminEvent(TelegramUser user, string lowerCaseMessage, CancellationToken token);
		Task<ScienceEvent?> CreateDeleteAdminEvent(TelegramUser user, string lowerCaseMessage, CancellationToken token);
	}
}
