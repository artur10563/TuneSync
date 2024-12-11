using Domain.Entities.Shared;
using System.Linq.Expressions;

namespace Application.Repositories.Shared
{
    public interface ILinkEntityRepository<TEntity> where TEntity : LinkEntity
    {
        void Insert(TEntity entity);
        void InsertRange(List<TEntity> entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);

        Task<TEntity?> FirstOrDefaultAsync(
           Expression<Func<TEntity, bool>> predicate,
           bool asNoTracking = false);

        IQueryable<TEntity> Queryable();
        IEnumerable<TEntity> Where(
            Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = false);
    }
}
