using Application.Repositories.Shared;
using Domain.Entities.Shared;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Repositories.Shared
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityBase
	{

		protected readonly AppDbContext _context;
		private readonly DbSet<TEntity> _set;

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
			entity.ModifiedAt = DateTime.Now;
			_set.Update(entity);
		}

		public virtual void Delete(TEntity entity)
		{
			_set.Remove(entity);
		}

		public virtual TEntity? FirstOrDefault(Func<TEntity, bool> predicate)
		{
			return _set.Where(predicate).FirstOrDefault();
		}

		public virtual TEntity? GetByGuid(Guid guid)
		{
			return _set.Where(e => e.Guid == guid).FirstOrDefault();
		}

		public virtual IQueryable<TEntity> Queryable()
		{
			return _set.AsQueryable();
		}

		public virtual IEnumerable<TEntity> Where(Func<TEntity, bool> predicate)
		{
			return _set.Where(predicate).AsQueryable();
		}
	}
}