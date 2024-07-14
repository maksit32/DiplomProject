using Domain.Entities;
using Domain.Repositories.Interfaces;

namespace DiplomProject.Server.Repositories
{
	public class TelegramUserRepository : ITelegramUserRepository
	{
		public Task Add(TelegramUser entity, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task Delete(TelegramUser entity, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<IReadOnlyCollection<TelegramUser>> GetAll(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<TelegramUser> GetById(Guid id, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task Update(TelegramUser entity, CancellationToken ct)
		{
			throw new NotImplementedException();
		}
	}
}
