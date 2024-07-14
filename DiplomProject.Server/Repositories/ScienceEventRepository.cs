using Domain.Entities;
using Domain.Repositories.Interfaces;

namespace DiplomProject.Server.Repositories
{
	public class ScienceEventRepository : IScienceEventRepository
	{
		public Task Add(ScienceEvent entity, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task Delete(ScienceEvent entity, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<IReadOnlyCollection<ScienceEvent>> GetAll(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<ScienceEvent> GetById(Guid id, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task Update(ScienceEvent entity, CancellationToken ct)
		{
			throw new NotImplementedException();
		}
	}
}
