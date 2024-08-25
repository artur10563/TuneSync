using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System;
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

		public async Task<IEnumerable<TEntity>> GetAsync()
		{
			var snapshot = (await _collection.GetSnapshotAsync());
			List<TEntity> entities = new List<TEntity>();

			foreach (var document in snapshot.Documents)
			{
				if (!document.Exists) continue;

				entities.Add(document.ConvertTo<TEntity>());
			}
			return entities;

		}

		public async Task<IEnumerable<object>> GetAsync(Expression<Func<TEntity, bool>> predicate)
		{
			var v = ExpressionToFirebase(predicate);
			object vv = v;
			await Task.CompletedTask;
			return new List<object>() { v.leftV, v.operationV, v.rightV };
		}

		private object GetValueFromClosure(MemberExpression memberExp)
		{
			object value = null;
			string fieldName = memberExp?.Member.Name;
			while (memberExp != null)
			{
				if (!(memberExp.Expression is ConstantExpression constantExpression))
				{
					memberExp = memberExp.Expression as MemberExpression;
					continue;
				}

				value = GetValueFromConstant(constantExpression, fieldName);
			}
			return value;
		}


		private object GetValueFromConstant(ConstantExpression constantExp, string fieldName)
		{
			object value = null;
			var closureClass = constantExp.Value;
			var closureType = closureClass.GetType();

			// Find the field or property in the closure class
			FieldInfo fieldInfo = closureType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
											 .FirstOrDefault(f => f.FieldType == typeof(TEntity)); //Not always type of TEntity
			PropertyInfo propertyInfo = closureType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
												   .FirstOrDefault(p => p.PropertyType == typeof(TEntity));

			if (fieldInfo != null)
			{
				// Get the value from the field
				var fieldValue = fieldInfo.GetValue(closureClass);
				if (fieldValue != null)
				{
					// Get the property value from the field value
					var property = fieldValue.GetType().GetProperty(fieldName);
					value = property?.GetValue(fieldValue);
				}
			}
			else if (propertyInfo != null)
			{
				// Get the value from the property
				var propertyValue = propertyInfo.GetValue(closureClass);
				if (propertyValue != null)
				{
					// Get the property value from the property value
					var property = propertyValue.GetType().GetProperty(fieldName);
					value = property?.GetValue(propertyValue);
				}
			}
			return value;
		}

		private (string leftV, string operationV, string rightV) ExpressionToFirebase(Expression<Func<TEntity, bool>> predicate)
		{
			var binaryExpression = predicate.Body as BinaryExpression ?? throw new ArgumentNullException(nameof(predicate));

			var rightMember = binaryExpression.Right as MemberExpression;
			object rightValue = null;

			if (rightMember.Expression is ConstantExpression constant)
				rightValue = GetValueFromConstant(constant, rightMember.Member.Name);

			else if (rightMember.Expression is MemberExpression member)
				rightValue = GetValueFromClosure(member);
			var operation = binaryExpression.Method.Name;


			string l = (binaryExpression.Left as MemberExpression).Member.Name;
			return (l, operation, rightValue.ToString());

		}
		private string OperationToString()
		{
			return "";
		}

		public void Update()
		{
			throw new NotImplementedException();
		}
	}
}
