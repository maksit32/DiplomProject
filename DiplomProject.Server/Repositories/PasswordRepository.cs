using Domain.Entities;
using Domain.Repositories.Interfaces;

namespace DiplomProject.Server.Repositories
{
	public class PasswordRepository : IPasswordRepostitory
	{
		public Task Add(Password entity, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task Delete(Password entity, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<IReadOnlyCollection<Password>> GetAll(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<Password> GetById(Guid id, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task Update(Password entity, CancellationToken ct)
		{
			throw new NotImplementedException();
		}
	}
}
