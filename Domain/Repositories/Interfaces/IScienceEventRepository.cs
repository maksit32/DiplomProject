using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Repositories.Interfaces
{
	public interface IScienceEventRepository : IRepository<ScienceEvent>
	{
		Task AddEventAsync(ScienceEvent newEvent, CancellationToken token);
		Task<List<ScienceEvent>> ReadAllEventsAsync(CancellationToken token);
		Task<List<ScienceEvent>> ReadAllActualEventsAsync(CancellationToken token);
		Task<ScienceEvent?> GetScienceEventByIdAsync(Guid Id, CancellationToken token);
		Task UpdateFullEventAsync(ScienceEvent sEvent, CancellationToken token);
		Task DeleteEventByIdAsync(Guid Id, CancellationToken token);
		Task DeleteEventAsync(ScienceEvent sEvent, CancellationToken token);
		Task<ScienceEvent?> GetLastCreatedEventAsync(CancellationToken token);
	}
}
