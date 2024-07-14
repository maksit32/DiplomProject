using Domain.Entities;
using Domain.Repositories.Interfaces;


namespace DiplomProject.Server.Repositories
{
	public class UserCreatedEventRepository : IUserCreatedEventRepository
	{
		public Task Add(UserCreatedEvent entity, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task Delete(UserCreatedEvent entity, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<IReadOnlyCollection<UserCreatedEvent>> GetAll(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<UserCreatedEvent> GetById(Guid id, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task Update(UserCreatedEvent entity, CancellationToken ct)
		{
			throw new NotImplementedException();
		}
	}
}
