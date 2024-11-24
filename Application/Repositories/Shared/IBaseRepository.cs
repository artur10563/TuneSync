using Domain.Entities.Shared;
using System.Linq.Expressions;

namespace Application.Repositories.Shared
{
    public interface IBaseRepository<TEntity> where TEntity : EntityBase
    {
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);

        Task<bool> ExistsAsync(
            Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = false,
            params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity?> GetByGuidAsync(
            Guid guid,
            bool asNoTracking = false,
            params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = false,
            params Expression<Func<TEntity, object>>[] includes);

        IQueryable<TEntity> Queryable();

        IQueryable<TEntity> Where(
            Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = false,
            params Expression<Func<TEntity, object>>[] includes);
    }
}
