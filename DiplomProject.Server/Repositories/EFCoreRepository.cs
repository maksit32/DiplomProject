using DiplomProject.Server.DbContexts;
using Domain.Entities;
using Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiplomProject.Server.Repositories
{
	public class EFCoreRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
	{
		private readonly DiplomDbContext _dbContext;

		private DbSet<TEntity> Entities => _dbContext.Set<TEntity>();

		public EFCoreRepository(DiplomDbContext dbContext)
		{
			_dbContext = dbContext;
		}
	}
}
