using Application.Repositories.Shared;
using Domain.Entities.Shared;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Shared
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityBase
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<TEntity> _set;

        protected BaseRepository(AppDbContext context)
        {
            _context = context;
            _set = _context.Set<TEntity>();
        }

        public void Insert(TEntity entity)
        {
            _set.Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            entity.ModifiedAt = DateTime.Now.ToUniversalTime();
            _set.Update(entity);
        }
        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.ModifiedAt = DateTime.Now.ToUniversalTime();
            }
            _set.UpdateRange(entities);
        }

        public virtual void Delete(TEntity entity)
        {
            _set.Remove(entity);
        }

        public virtual async Task<TEntity?> GetByGuidAsync(Guid guid, bool asNoTracking = false,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _set;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(e => e.Guid == guid);
        }

        public virtual IQueryable<TEntity> Queryable()
        {
            return _set.AsQueryable();
        }
        
        public virtual IQueryable<TEntity> NoTrackingQueryable()
        {
            return _set.AsNoTracking().AsQueryable();
        }

        public virtual IQueryable<TEntity> Where(
            Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _set;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return query.Where(predicate);
        }

        public virtual async Task<bool> ExistsAsync(
            Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _set;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.AnyAsync(predicate);
        }

        public async Task<List<Guid>> GetUniqueExistingGuidsAsync(List<Guid> inputGuids)
        {
            if (inputGuids == null || inputGuids.Count == 0)
                return [];
            
            var distinctGuids = inputGuids.Distinct().ToList();
            
            var existingGuids = await _set
                .Where(entity => distinctGuids.Contains(entity.Guid))
                .Select(entity => entity.Guid)
                .ToListAsync();

            return existingGuids;
        }
        
        public virtual async Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _set;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.Where(predicate).FirstOrDefaultAsync();
        }
    }
}
