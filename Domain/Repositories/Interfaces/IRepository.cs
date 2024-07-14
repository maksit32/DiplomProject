using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Repositories.Interfaces
{
	public interface IRepository<TEntity> where TEntity : IEntity
	{
		Task<TEntity> GetById(Guid id, CancellationToken ct);
		Task<IReadOnlyCollection<TEntity>> GetAll(CancellationToken ct);
		Task Add(TEntity entity, CancellationToken ct);
		Task Update(TEntity entity, CancellationToken ct);
		Task Delete(TEntity entity, CancellationToken ct);
	}
}
