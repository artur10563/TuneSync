using System.Linq.Expressions;
using TuneSync.Domain.Entities.Shared;

namespace TuneSync.Application.Repositories
{
	public interface IBaseRepository<TEntity> where TEntity : EntityBase
	{
		Task Create(TEntity newEntity);
		void Update();
		void Delete();
		Task<IEnumerable<TEntity>> GetAsync(int limit = 50);
		Task<IEnumerable<TEntity>> GetAsync(List<QueryFilter> filters, int limit = 50);
	}

	public record QueryFilter(string Field, string Operation, object Value, string Comparison = QueryComparison.And);
	public static class QueryComparison
	{
		public const string And = "&&";
		public const string Or = "||";
	}
}
