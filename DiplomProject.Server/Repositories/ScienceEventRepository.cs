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
			return await ScienceEvents.ToListAsync(token);
		}
		public async Task<List<ScienceEvent>> ReadAllActualEventsAsync(CancellationToken token)
		{
			return await ScienceEvents.Where(e => e.DateEvent > DateTime.UtcNow).ToListAsync(token);
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

		public async Task<ScienceEvent?> GetScienceEventByIdAsync(Guid Id, CancellationToken token)
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
