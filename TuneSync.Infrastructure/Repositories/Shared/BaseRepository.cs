using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using TuneSync.Application.Repositories;
using TuneSync.Domain.Entities.Shared;

namespace TuneSync.Infrastructure.Repositories
{
	public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityBase
	{
		private readonly IConfiguration _configuration;
		private readonly FirestoreDb _db;
		private readonly CollectionReference _collection;


		public BaseRepository(IConfiguration configuration, string collectionName)
		{
			_configuration = configuration;
			Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", _configuration["Firebase:JsonPath"]);
			_db = FirestoreDb.Create(_configuration["Firebase:Project"]);
			_collection = _db.Collection(collectionName);
		}
		public async Task Create(TEntity newEntity)
		{
			DocumentReference document = await _collection.AddAsync(newEntity);
		}

		public void Delete()
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<TEntity>> GetAsync(int limit = 50)
		{
			var snapshot = (await _collection.Limit(limit).GetSnapshotAsync());
			List<TEntity> entities = new List<TEntity>();

			foreach (var document in snapshot.Documents)
			{
				if (!document.Exists) continue;

				entities.Add(document.ConvertTo<TEntity>());
			}
			return entities;

		}

		/// <summary>
		/// Groups 'AND' and 'OR' QueryFilters, executes Filter.Or, Filter.And on those groups
		/// </summary>
		/// <param name="filters">List of QueryFilters, which are translated to Firebase Filters</param>
		/// <returns>Filtered records</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public async Task<IEnumerable<TEntity>> GetAsync(List<QueryFilter> filters, int limit= 50)
		{
			if (filters.Count == 0) return await GetAsync();
			Query query = _collection;

			List<Filter> fbFiltersAnd = [];
			List<Filter> fbFiltersOr = [];

			
			foreach (var filter in filters)
			{

				var fbFilter = filter.Operation switch
				{
					"==" => Filter.EqualTo(filter.Field, filter.Value),
					"!=" => Filter.NotEqualTo(filter.Field, filter.Value),
					">" => Filter.GreaterThan(filter.Field, filter.Value),
					"<" => Filter.LessThan(filter.Field, filter.Value),
					">=" => Filter.GreaterThanOrEqualTo(filter.Field, filter.Value),
					"<=" => Filter.LessThanOrEqualTo(filter.Field, filter.Value),
					"Contains" => Filter.ArrayContains(filter.Field, filter.Value),
					"In" => Filter.InArray(filter.Field, (IEnumerable)filter.Value),
					_ => throw new InvalidOperationException($"Unsupported operation: {filter.Operation}")
				};

				if (!string.IsNullOrEmpty(filter.Comparison))
				{
					if (filter.Comparison == QueryComparison.And)
					{
						fbFiltersAnd.Add(fbFilter);
					}
					else if (filter.Comparison == QueryComparison.Or)
					{
						fbFiltersOr.Add(fbFilter);
					}
				}
			}
			//TODO: broken is comparison is empty even if its last in list

			//TODO: recheck Contains

			//TODO: test if passed single QueryFilter, test results of monstrocity we made

			if (fbFiltersAnd.Count > 0)
				query = query.Where(Filter.And(fbFiltersAnd));

			if (fbFiltersOr.Count > 0)
				query = query.Where(Filter.Or(fbFiltersOr));


			//TODO: replace with extension method as this code is repeated
			var snapshot = await query.Limit(limit).GetSnapshotAsync();

			var entities = new List<TEntity>();

			foreach (var document in snapshot.Documents)
			{
				if (!document.Exists) continue;

				entities.Add(document.ConvertTo<TEntity>());
			}

			return entities;
		}

		

		public void Update()
		{
			throw new NotImplementedException();
		}
	}
}
