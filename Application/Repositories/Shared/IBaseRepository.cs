using Domain.Entities.Shared;

namespace Application.Repositories.Shared
{
	public interface IBaseRepository<TEntity> where TEntity : EntityBase
	{
		void Insert(TEntity entity);
		void Update(TEntity entity);
		void Delete(TEntity entity);


		TEntity? GetByGuid(Guid guid);
		TEntity? FirstOrDefault(Func<TEntity, bool> predicate);

		IQueryable<TEntity> Queryable();
		IEnumerable<TEntity> Where(Func<TEntity, bool> predicate);

	}
}
