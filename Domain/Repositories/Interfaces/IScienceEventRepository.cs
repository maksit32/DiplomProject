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
		Task<bool> AddEventAsync(string lowerCaseMessage, CancellationToken token);
		Task AddEventAsync(ScienceEvent newEvent, CancellationToken token);
		Task<List<ScienceEvent>> ReadAllEventsAsync(CancellationToken token);
		Task<List<ScienceEvent>> ReadAllActualEventsAsync(CancellationToken token);
		Task<ScienceEvent?> GetScienceEventById(Guid Id, CancellationToken token);
		Task<string> ReadAllEventsToStringAsync(CancellationToken token);
		Task<string> ReadAllActualEventsToStringAsync(CancellationToken token);
		Task<ScienceEvent?> ReadScienceEventByIdAsync(Guid id, CancellationToken token);
		Task<ScienceEvent?> ReadScienceEventByNameAsync(string name, CancellationToken token);
		Task<bool> UpdateEventNameAsync(string oldName, string newName, CancellationToken token);
		Task<bool> UpdateEventDateAsync(string name, DateTime newDate, CancellationToken token);
		Task<bool> UpdateEventPlaceAsync(string name, string place, CancellationToken token);
		Task<bool> UpdateEventRequirementsAsync(string name, string require, CancellationToken token);
		Task<bool> UpdateEventInformationAsync(string name, string information, CancellationToken token);
		Task<ScienceEvent?> UpdateFullEventAsync(string lowerCaseMessage, CancellationToken token);
		Task<ScienceEvent?> UpdateFullEventAsync(ScienceEvent sEvent, CancellationToken token);
		Task<ScienceEvent?> DeleteEventByIdAsync(string lowerCaseMessage, CancellationToken token);
		Task DeleteEventByIdAsync(Guid Id, CancellationToken token);
		Task<bool> DeleteEventByNameAsync(string name, CancellationToken token);
		Task<ScienceEvent?> GetLastCreatedEventAsync(CancellationToken token);
		Task<string> ReadAllActualEvAdminToStringAsync(CancellationToken token);
	}
}
