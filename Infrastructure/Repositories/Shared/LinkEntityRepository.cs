using Application.Repositories.Shared;
using Domain.Entities.Shared;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Shared
{
    public class LinkEntityRepository<TEntity> : ILinkEntityRepository<TEntity> where TEntity : LinkEntity
    {

        protected readonly AppDbContext _context;
        private readonly DbSet<TEntity> _set;

        public LinkEntityRepository(AppDbContext context)
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
            _set.Update(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            _set.Remove(entity);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
        {
            IQueryable<TEntity> query = _set;

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.Where(predicate).FirstOrDefaultAsync();
        }


        public IQueryable<TEntity> Queryable()
        {
            return _set.AsQueryable();
        }


        public IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
        {
            IQueryable<TEntity> query = _set;

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return query.Where(predicate);
        }
    }
}
