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

		public async Task<bool> AddEventAsync(string lowerCaseMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
			{
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			}

			lowerCaseMessage = lowerCaseMessage.Replace("/addevent/", "");
			var dataArray = lowerCaseMessage.Split("/");


			//NameEvent
			string nameEvent = char.ToUpper(dataArray[0][0]) + dataArray[0].Substring(1);
			//DateEvent
			var cultureInfo = new CultureInfo("ru-RU");
			DateTime dateEvent = DateTime.Parse(dataArray[1], cultureInfo);
			if (dateEvent < DateTime.UtcNow) { throw new IrrelevatDateTimeException($"{AlertEmj}Неверно указана дата события!"); }
			//PlaceEvent
			string placeEvent = char.ToUpper(dataArray[2][0]) + dataArray[2].Substring(1);
			//Requirement
			string requirement = dataArray[3];
			//Information
			string information = dataArray[4];
			//ChatId
			long chatId = long.Parse(dataArray[5]);
			ScienceEvent sEvent = new ScienceEvent(nameEvent, dateEvent, placeEvent, requirement, information, chatId);


			if (ScienceEvents.ToList().Exists(e => e.NameEvent.ToLower() == sEvent.NameEvent.ToLower()))
			{
				return false;
			}

			await ScienceEvents.AddAsync(sEvent);
			await _dbContext.SaveChangesAsync(token);
			return true;
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
		public async Task<string> ReadAllActualEventsToStringAsync(CancellationToken token)
		{
			var events = await ScienceEvents.ToListAsync();
			string messageToSend = "";
			foreach (var e in events)
			{
				if (e.DateEvent > DateTime.UtcNow)
				{
					messageToSend += e.ToString();
				}
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
		public async Task<ScienceEvent?> UpdateFullEventAsync(string lowerCaseMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
			{
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			}

			lowerCaseMessage = lowerCaseMessage.Replace("/chevent/", "");
			var dataArray = lowerCaseMessage.Split("/");

			//IdEvent
			Guid idEvent = Guid.Parse(dataArray[0]);
			//NameEvent
			string nameEvent = char.ToUpper(dataArray[1][0]) + dataArray[1].Substring(1);
			//проверка на повтор названия
			if (ScienceEvents.ToList().Exists(e => e.NameEvent.ToLower() == nameEvent.ToLower() && e.Id != idEvent))
			{
				return null;
			}
			//DateEvent
			var cultureInfo = new CultureInfo("ru-RU");
			DateTime dateEvent = DateTime.Parse(dataArray[2], cultureInfo);
			if (dateEvent < DateTime.UtcNow) { throw new IrrelevatDateTimeException($"{AlertEmj}Неверно указана дата события!"); }
			//PlaceEvent
			string placeEvent = char.ToUpper(dataArray[3][0]) + dataArray[3].Substring(1);
			//Requirement
			string requirement = dataArray[4];
			//Information
			string information = dataArray[5];
			//ChatId
			long chatId = long.Parse(dataArray[6]);


			//получаем старое мероприятие и меняем его значения
			var sEvent = await GetScienceEventByIdAsync(idEvent);
			if (sEvent == null) return null;


			sEvent.NameEvent = nameEvent;
			sEvent.DateEvent = dateEvent;
			sEvent.PlaceEvent = placeEvent;
			sEvent.RequirementsEvent = requirement;
			sEvent.InformationEvent = information;
			sEvent.DateEventCreated = DateTime.UtcNow;
			sEvent.AddByAdminChatId = chatId;


			await _dbContext.SaveChangesAsync(token);
			return sEvent;
		}
		public async Task<ScienceEvent?> DeleteEventByIdAsync(string lowerCaseMessage, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(lowerCaseMessage))
			{
				throw new ArgumentException($"\"{nameof(lowerCaseMessage)}\" не может быть пустым или содержать только пробел.", nameof(lowerCaseMessage));
			}

			lowerCaseMessage = lowerCaseMessage.Replace("/deleteevent", "");
			lowerCaseMessage.Replace(" ", "");
			Guid id = Guid.Parse(lowerCaseMessage);


			var sEvent = await GetScienceEventByIdAsync(id);
			var noticeObject = sEvent;
			if (sEvent is not null)
			{
				ScienceEvents.Remove(sEvent);
				await _dbContext.SaveChangesAsync(token);
				return noticeObject;
			}
			return null;
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
				return await ScienceEvents.OrderBy(e => e.Id).LastAsync();
			}
			return null;
		}

		public async Task AddEventAsync(ScienceEvent newEvent, CancellationToken token)
		{
			await ScienceEvents.AddAsync(newEvent, token);
			await _dbContext.SaveChangesAsync(token);
		}

		public async Task<ScienceEvent?> GetScienceEventById(Guid Id, CancellationToken token)
		{
			return await ScienceEvents.FirstOrDefaultAsync(e => e.Id == Id);
		}

		public async Task UpdateFullEventAsync(ScienceEvent sEvent, CancellationToken token)
		{
			var oldEvent = await ScienceEvents.FirstOrDefaultAsync(e => e.Id == sEvent.Id);
			if (oldEvent is null) return;

			oldEvent.NameEvent = sEvent.NameEvent;
			oldEvent.DateEvent = sEvent.DateEvent;
			oldEvent.PlaceEvent = sEvent.PlaceEvent;
			oldEvent.RequirementsEvent = sEvent.RequirementsEvent;
			oldEvent.InformationEvent = sEvent.InformationEvent;
			oldEvent.DateEventCreated = sEvent.DateEventCreated;

			await _dbContext.SaveChangesAsync(token);
		}

		public async Task DeleteEventByIdAsync(Guid Id, CancellationToken token)
		{
			var evt = await ScienceEvents.FirstOrDefaultAsync(e => e.Id == Id);
			if(evt is null) return;

			ScienceEvents.Remove(evt);
			await _dbContext.SaveChangesAsync(token);
		}
	}
}
