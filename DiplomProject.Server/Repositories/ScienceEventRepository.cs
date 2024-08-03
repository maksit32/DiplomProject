using DiplomProject.Server.DbContexts;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static Domain.Constants.EmojiConstants;


namespace DiplomProject.Server.Repositories
{
	public class ScienceEventRepository : IScienceEventRepository
	{
		private readonly DiplomDbContext _dbContext;
		private DbSet<ScienceEvent> ScienceEvents => _dbContext.Set<ScienceEvent>();

		public ScienceEventRepository(DiplomDbContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}
		public async Task<List<ScienceEvent>> ReadAllEventsAsync(CancellationToken token)
		{
			return await ScienceEvents.ToListAsync();
		}
		public async Task<List<ScienceEvent>> ReadAllActualEventsAsync(CancellationToken token)
		{
			return await ScienceEvents.Where(e => e.DateEvent > DateTime.UtcNow).ToListAsync();
		}
		public async Task<string> ReadAllEventsToStringAsync(CancellationToken token)
		{
			var events = await ScienceEvents.ToListAsync();
			string messageToSend = "";
			foreach (var e in events)
			{
				messageToSend += e.ToString();
			}
			return messageToSend;
		}
		public async Task<ScienceEvent?> ReadScienceEventByIdAsync(Guid id, CancellationToken token)
		{
			var sEvent = await GetScienceEventByIdAsync(id);
			return sEvent;
		}
		public async Task<ScienceEvent?> ReadScienceEventByNameAsync(string name, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException($"\"{nameof(name)}\" не может быть пустым или содержать только пробел.", nameof(name));
			}

			var sEvent = await GetScienceEventByNameAsync(name, token);
			return sEvent;
		}
		private async Task<ScienceEvent?> GetScienceEventByNameAsync(string name, CancellationToken token)
		{
			return await ScienceEvents.FirstOrDefaultAsync(e => e.NameEvent == name);
		}
		private async Task<ScienceEvent?> GetScienceEventByIdAsync(Guid id)
		{
			return await ScienceEvents.FirstOrDefaultAsync(e => e.Id == id);
		}
		public async Task<bool> UpdateEventNameAsync(string oldName, string newName, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(oldName))
			{
				throw new ArgumentException($"\"{nameof(oldName)}\" не может быть пустым или содержать только пробел.", nameof(oldName));
			}

			if (string.IsNullOrWhiteSpace(newName))
			{
				throw new ArgumentException($"\"{nameof(newName)}\" не может быть пустым или содержать только пробел.", nameof(newName));
			}

			//проверка на повтор названия
			if (ScienceEvents.ToList().Exists(e => e.NameEvent.ToLower() == newName.ToLower()))
			{
				return false;
			}

			var sEvent = await GetScienceEventByNameAsync(oldName, token);
			if (sEvent == null) return false;

			sEvent.NameEvent = newName;

			await _dbContext.SaveChangesAsync(token);
			return true;
		}
		public async Task<bool> UpdateEventDateAsync(string name, DateTime newDate, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException($"\"{nameof(name)}\" не может быть пустым или содержать только пробел.", nameof(name));
			}

			var sEvent = await GetScienceEventByNameAsync(name, token);
			if (sEvent == null) return false;

			sEvent.DateEvent = newDate;

			await _dbContext.SaveChangesAsync(token);
			return true;
		}
		public async Task<bool> UpdateEventPlaceAsync(string name, string place, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException($"\"{nameof(name)}\" не может быть пустым или содержать только пробел.", nameof(name));
			}

			if (string.IsNullOrWhiteSpace(place))
			{
				throw new ArgumentException($"\"{nameof(place)}\" не может быть пустым или содержать только пробел.", nameof(place));
			}


			var sEvent = await GetScienceEventByNameAsync(name, token);
			if (sEvent == null) return false;

			sEvent.PlaceEvent = place;

			await _dbContext.SaveChangesAsync(token);
			return true;
		}
		public async Task<bool> UpdateEventRequirementsAsync(string name, string require, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException($"\"{nameof(name)}\" не может быть пустым или содержать только пробел.", nameof(name));
			}

			if (string.IsNullOrWhiteSpace(require))
			{
				throw new ArgumentException($"\"{nameof(require)}\" не может быть пустым или содержать только пробел.", nameof(require));
			}


			var sEvent = await GetScienceEventByNameAsync(name, token);
			if (sEvent == null) return false;

			sEvent.RequirementsEvent = require;

			await _dbContext.SaveChangesAsync(token);
			return true;
		}
		public async Task<bool> UpdateEventInformationAsync(string name, string information, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException($"\"{nameof(name)}\" не может быть пустым или содержать только пробел.", nameof(name));
			}

			if (string.IsNullOrWhiteSpace(information))
			{
				throw new ArgumentException($"\"{nameof(information)}\" не может быть пустым или содержать только пробел.", nameof(information));
			}


			var sEvent = await GetScienceEventByNameAsync(name, token);
			if (sEvent == null) return false;

			sEvent.InformationEvent = information;

			await _dbContext.SaveChangesAsync(token);
			return true;
		}
		public async Task<bool> DeleteEventByNameAsync(string name, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException($"\"{nameof(name)}\" не может быть пустым или содержать только пробел.", nameof(name));
			}

			var sEvent = await GetScienceEventByNameAsync(name, token);
			if (sEvent is not null)
			{
				ScienceEvents.Remove(sEvent);
				await _dbContext.SaveChangesAsync(token);
				return true;
			}
			return false;
		}
		public async Task<ScienceEvent?> GetLastCreatedEventAsync(CancellationToken token)
		{
			if (ScienceEvents.Count() > 0)
			{
				return await ScienceEvents.OrderBy(e => e.DateEventCreated).LastAsync();
			}
			return null;
		}

		public async Task AddEventAsync(ScienceEvent newEvent, CancellationToken token)
		{
			if (newEvent is null) throw new ArgumentNullException(nameof(newEvent));

			await ScienceEvents.AddAsync(newEvent, token);
			await _dbContext.SaveChangesAsync(token);
		}

		public async Task<ScienceEvent?> GetScienceEventById(Guid Id, CancellationToken token)
		{
			return await ScienceEvents.FirstOrDefaultAsync(e => e.Id == Id);
		}

		public async Task UpdateFullEventAsync(ScienceEvent sEvent, CancellationToken token)
		{
			if (sEvent is null) throw new ArgumentNullException(nameof(sEvent));

			_dbContext.Update(sEvent);
			await _dbContext.SaveChangesAsync(token);
		}

		public async Task DeleteEventByIdAsync(Guid Id, CancellationToken token)
		{
			var evt = await ScienceEvents.FirstOrDefaultAsync(e => e.Id == Id);
			if (evt is null) return;

			ScienceEvents.Remove(evt);
			await _dbContext.SaveChangesAsync(token);
		}

		public async Task DeleteEventAsync(ScienceEvent sEvent, CancellationToken token)
		{
			if (sEvent is null) throw new ArgumentNullException(nameof(sEvent));

			ScienceEvents.Remove(sEvent);
			await _dbContext.SaveChangesAsync(token);
		}
	}
}
