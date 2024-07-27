using DiplomProject.Server.DbContexts;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;


namespace DiplomProject.Server.Repositories
{
	public class UserCreatedEventRepository : IUserCreatedEventRepository
	{
		private readonly DiplomDbContext _dbContext;
		private DbSet<UserCreatedEvent> UserCreatedEvents => _dbContext.Set<UserCreatedEvent>();
		private DbSet<TelegramUser> TelegramUsers => _dbContext.Set<TelegramUser>();


		public UserCreatedEventRepository(DiplomDbContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public async Task<List<UserCreatedEvent>> ReadAllUserEventsAsync(TelegramUser tgUser, CancellationToken token)
		{
			var lst = from e in UserCreatedEvents
					  where e.TgUser.Id == tgUser.Id
					  select e;
			return await lst.ToListAsync();
		}
		public async Task<bool> AddUserCreatedEventAsync(string message, long chatId, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				throw new ArgumentException($"\"{nameof(message)}\" не может быть пустым или содержать только пробел.", nameof(message));
			}

			message = message.Replace("/adduserevent/", "");
			var dataArr = message.Split('/');
			//NameEvent
			string nameEvent = char.ToUpper(dataArr[0][0]) + dataArr[0].Substring(1);
			//PlaceEvent
			string placeEvent = char.ToUpper(dataArr[1][0]) + dataArr[1].Substring(1);
			//DateEvent
			var cultureInfo = new CultureInfo("ru-RU");
			DateTime dateEvent = DateTime.Parse(dataArr[2], cultureInfo);
			//IsWinner
			bool isWinner = bool.Parse(dataArr[3]);
			//TgUser
			TelegramUser? tgUser = await GetTgUserByIdAsync(chatId);
			if (tgUser is null) return false;

			UserCreatedEvent uEvent = new UserCreatedEvent(nameEvent, placeEvent, dateEvent, isWinner, tgUser);

			//проверка на уже добавленное ранее событие этим пользователем
			if (UserCreatedEvents.ToList().Exists(e => e.NameEvent.ToLower() == uEvent.NameEvent.ToLower() && e.TgUser.Id == tgUser.Id))
			{
				return false;
			}

			await UserCreatedEvents.AddAsync(uEvent);
			await _dbContext.SaveChangesAsync();
			return true;
		}
		public async Task<UserCreatedEvent?> GetUserCreatedEventByIdAsync(Guid uEventId, CancellationToken token)
		{
			return await UserCreatedEvents.FirstOrDefaultAsync(e => e.Id == uEventId);
		}
		public async Task<bool> UpdateUserCreatedEventAsync(string message, long chatId, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				throw new ArgumentException($"\"{nameof(message)}\" не может быть пустым или содержать только пробел.", nameof(message));
			}

			message = message.Replace("/updateuserevent/", "");
			var dataArr = message.Split('/');
			//NameEvent
			string nameEvent = char.ToUpper(dataArr[0][0]) + dataArr[0].Substring(1);
			//PlaceEvent
			string placeEvent = char.ToUpper(dataArr[1][0]) + dataArr[1].Substring(1);
			//DateEvent
			var cultureInfo = new CultureInfo("ru-RU");
			DateTime dateEvent = DateTime.Parse(dataArr[2], cultureInfo);
			//IsWinner
			bool isWinner = bool.Parse(dataArr[3]);
			//EventId
			Guid eventId = Guid.Parse(dataArr[4]);

			//проверка, что событие именно этого пользователя
			var evToChange = await GetUserCreatedEventByIdAsync(eventId, token);
			if (evToChange is null) return false;
			var tgUser = await GetTgUserByIdAsync(chatId);
			if (tgUser is null) return false;

			if (evToChange.TgUser.Id == tgUser.Id)
			{
				//название прочих мероприятий равны
				if (UserCreatedEvents.ToList().Exists(e => e.NameEvent == nameEvent && e.Id != evToChange.Id && e.TgUser.Id == tgUser.Id))
				{
					return false;
				}

				evToChange.NameEvent = nameEvent;
				evToChange.PlaceEvent = placeEvent;
				evToChange.DateEvent = dateEvent;
				evToChange.IsWinner = isWinner;

				await _dbContext.SaveChangesAsync();
				return true;
			}
			return false;
		}
		public async Task<UserCreatedEvent?> DeleteUserCreatedEventByIdAsync(string message, long chatId, CancellationToken token)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				throw new ArgumentException($"\"{nameof(message)}\" не может быть пустым или содержать только пробел.", nameof(message));
			}

			message = message.Replace("/deleteuserevent/", "");
			Guid eventToDeleteId = Guid.Parse(message);

			//проверка, что событие именно этого пользователя
			var user = await GetTgUserByIdAsync(chatId);
			if (user is null) return null;
			var evToChange = await GetUserCreatedEventByIdAsync(eventToDeleteId, token);
			if (evToChange is null) return null;
			//принадлежит пользователю
			if (evToChange.TgUser.Id == user.Id)
			{
				UserCreatedEvents.Remove(evToChange);
				await _dbContext.SaveChangesAsync();
				return evToChange; //удаленное событие
			}
			return null;
		}
		public async Task<List<UserCreatedEvent>> ReadAllEventsAsync(CancellationToken token)
		{
			return await UserCreatedEvents.ToListAsync(token);
		}
		public async Task<List<UserCreatedEvent>> ReadAllUserEventsAsync(Guid Id, CancellationToken token)
		{
			return await UserCreatedEvents.Where(e => e.TgUser.Id == Id).ToListAsync(token);
		}
		public async Task DeleteUserCreatedEventByIdAsync(Guid Id, CancellationToken token)
		{
			var createdEvent = await GetUserCreatedEventByIdAsync(Id, token);
			if (createdEvent is null) return;

			UserCreatedEvents.Remove(createdEvent);
			await _dbContext.SaveChangesAsync();
		}
		public async Task UpdateUserCreatedEventAsync(UserCreatedEvent newEvent, CancellationToken token)
		{
			var oldEvent = await GetUserCreatedEventByIdAsync(newEvent.Id, token);
			if (oldEvent is null) return;
			
			oldEvent.NameEvent = newEvent.NameEvent;
			oldEvent.PlaceEvent = newEvent.PlaceEvent;
			oldEvent.DateEvent = newEvent.DateEvent;
			oldEvent.IsWinner = newEvent.IsWinner;

			await _dbContext.SaveChangesAsync();
		}
		private async Task<TelegramUser?> GetTgUserByIdAsync(long chatId)
		{
			return await TelegramUsers.FirstOrDefaultAsync(e => e.TgChatId == chatId);
		}
		private async Task<TelegramUser?> GetTgUserByIdAsync(Guid Id)
		{
			return await TelegramUsers.FirstOrDefaultAsync(e => e.Id == Id);
		}
	}
}
