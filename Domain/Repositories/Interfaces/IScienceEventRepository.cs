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
		Task<bool> AddEventAsync(string lowerCaseMessage);
		Task<IReadOnlyList<ScienceEvent>> ReadAllEventsAsync();
		Task<string> ReadAllEventsToStringAsync();
		Task<string> ReadAllActualEventsToStringAsync();
		Task<ScienceEvent?> ReadScienceEventByIdAsync(Guid id);
		Task<ScienceEvent?> ReadScienceEventByNameAsync(string name);
		Task<bool> UpdateEventNameAsync(string oldName, string newName);
		Task<bool> UpdateEventDateAsync(string name, DateTime newDate);
		Task<bool> UpdateEventPlaceAsync(string name, string place);
		Task<bool> UpdateEventRequirementsAsync(string name, string require);
		Task<bool> UpdateEventInformationAsync(string name, string information);
		Task<ScienceEvent?> UpdateFullEventAsync(string lowerCaseMessage);
		Task<ScienceEvent?> DeleteEventByIdAsync(string lowerCaseMessage);
		Task<bool> DeleteEventByNameAsync(string name);
		Task<ScienceEvent?> GetLastCreatedEventAsync();
	}
}
