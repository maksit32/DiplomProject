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


		public UserCreatedEventRepository(DiplomDbContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}
		public async Task AddUserCreatedEventAsync(UserCreatedEvent userCreatedEvent, CancellationToken token)
		{
			if(userCreatedEvent is null) throw new ArgumentNullException(nameof(userCreatedEvent));

			await UserCreatedEvents.AddAsync(userCreatedEvent, token);
			await _dbContext.SaveChangesAsync(token);
		}
		public async Task<List<UserCreatedEvent>> ReadAllUserEventsAsync(TelegramUser tgUser, CancellationToken token)
		{
			var lst = from e in UserCreatedEvents
					  where e.TgUser.Id == tgUser.Id
					  select e;
			return await lst.ToListAsync(token);
		}
		public async Task<UserCreatedEvent?> GetUserCreatedEventByIdAsync(Guid uEventId, CancellationToken token)
		{
			return await UserCreatedEvents.FirstOrDefaultAsync(e => e.Id == uEventId);
		}
		public async Task<List<UserCreatedEvent>> ReadAllEventsAsync(CancellationToken token)
		{
			return await UserCreatedEvents.ToListAsync(token);
		}
		public async Task<List<UserCreatedEvent>> ReadAllUserEventsAsync(Guid UserId, CancellationToken token)
		{
			return await UserCreatedEvents.Where(e => e.TgUser.Id == UserId).ToListAsync(token);
		}
		public async Task DeleteUserCreatedEventByIdAsync(Guid Id, CancellationToken token)
		{
			var createdEvent = await GetUserCreatedEventByIdAsync(Id, token);
			if (createdEvent is null) return;

			UserCreatedEvents.Remove(createdEvent);
			await _dbContext.SaveChangesAsync(token);
		}
		public async Task UpdateUserCreatedEventAsync(UserCreatedEvent newEvent, CancellationToken token)
		{
			if(newEvent is null) throw new ArgumentNullException("event is null!");
			UserCreatedEvents.Update(newEvent);
			await _dbContext.SaveChangesAsync(token);
		}
		public async Task DeleteUserCreatedEvent(UserCreatedEvent ev, CancellationToken token)
		{
			if (ev is null) throw new ArgumentNullException("event is null!");
			UserCreatedEvents.Remove(ev);
			await _dbContext.SaveChangesAsync(token);
		}
	}
}
