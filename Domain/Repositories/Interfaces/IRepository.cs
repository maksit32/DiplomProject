using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;


namespace Domain.Repositories.Interfaces
{
	public interface IRepository<TEntity> where TEntity : IEntity
	{

	}
}
