using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TuneSync.Domain.Entities.Shared;

namespace TuneSync.Application.Repositories
{
	public interface IBaseRepository<TEntity> where TEntity : EntityBase
	{
		Task Create(TEntity newEntity);
		void Update();
		void Delete();
		Task<IEnumerable<TEntity>> GetAsync();
		Task<IEnumerable<object>> GetAsync(Expression<Func<TEntity, bool>> predicate);
	}
}
