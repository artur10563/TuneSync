using Domain.Entities.Shared;
using System.Linq.Expressions;

namespace Application.Repositories.Shared
{
    public interface IBaseRepository<TEntity> where TEntity : EntityBase
    {
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);

        Task<int> BulkUpdatePropertyAsync<TProperty>(
            Expression<Func<TEntity, bool>> predicate,
            Func<TEntity, TProperty> propertySelector,
            Func<TEntity, TProperty> valueSelector);
        void Delete(TEntity entity);

        Task<List<Guid>> GetUniqueExistingGuidsAsync(List<Guid> inputGuids);
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
        IQueryable<TEntity> NoTrackingQueryable();

        IQueryable<TEntity> Where(
            Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = false,
            params Expression<Func<TEntity, object>>[] includes);
        
        //Get entity with all includes. Required for cascade deletion
        Task<TEntity?> FirstOrDefaultWithDependantAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
