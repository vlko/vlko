using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Raven.Client.Linq;
using vlko.core.Repository;

namespace vlko.core.RavenDB.Repository
{
	/// <summary>
	/// Live projection query results.
	/// </summary>
	/// <typeparam name="TRoot">The root agregate type.</typeparam>
	/// <typeparam name="TReduce">The type of the reduce.</typeparam>
	/// <typeparam name="T">Type of output.</typeparam>
	public class LiveProjectionQueryResult<TRoot, TReduce, T> : IQueryResult<T>
		where T : class
		where TReduce : class
		where TRoot : class
	{
		private readonly Dictionary<string, LambdaExpression> _sortMappings;
		private readonly IQueryable<TReduce> _query;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectionQueryResult&lt;TRoot, T&gt;"/> class.
		/// </summary>
		/// <param name="query">The query.</param>
		public LiveProjectionQueryResult(IQueryable<TReduce> query)
			:this(query, null)
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectionQueryResult&lt;TRoot, T&gt;"/> class.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="sortMappings">The allowed orders.</param>
		private LiveProjectionQueryResult(IQueryable<TReduce> query, Dictionary<string, LambdaExpression> sortMappings)
		{
			_query = query;
			_sortMappings = sortMappings ?? new Dictionary<string, LambdaExpression>();
		}

		/// <summary>
		/// Adds the sort mapping.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="rootSort">The root sort.</param>
		/// <param name="transformSort">The transform sort.</param>
		/// <returns>Fluent interface.</returns>
		public LiveProjectionQueryResult<TRoot, TReduce, T> AddSortMapping<TKey>(Expression<Func<TReduce, TKey>> rootSort, Expression<Func<T, TKey>> transformSort)
		{
			string alias = GetAlias(transformSort);
			_sortMappings.Add(alias, rootSort);
			return this;
		}

		/// <summary>
		/// Gets the transform alias.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="sortFunction">The sort function.</param>
		/// <returns>Alias</returns>
		private static string GetAlias<TKey>(Expression<Func<T, TKey>> sortFunction)
		{
			string alias;
			if (sortFunction.Body is UnaryExpression)
			{
				alias = ((MemberExpression)((UnaryExpression)(sortFunction.Body)).Operand).Member.Name;
			}
			else
			{
				alias = ((MemberExpression)(sortFunction.Body)).Member.Name;
			}
			return alias;
		}

		/// <summary>
		/// Orders the by.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="query">The query.</param>
		/// <returns>Immutable copy of query results sorted by query.</returns>
		public IQueryResult<T> OrderBy<TKey>(Expression<Func<T, TKey>> query)
		{
			// resolve sort expression based on query
			Expression<Func<TReduce, TKey>> sortExpression = TryResolveSortGetExpression(query);

			// immutable copy as a result with sort
			return new LiveProjectionQueryResult<TRoot, TReduce, T>(
				_query.OrderBy(sortExpression), _sortMappings);
		}

		/// <summary>
		/// Orders the by descending.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="query">The query.</param>
		/// <returns>Immutable copy of query results sorted by query.</returns>
		public IQueryResult<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> query)
		{
			// resolve sort expression based on query
			Expression<Func<TReduce, TKey>> sortExpression = TryResolveSortGetExpression(query);

			// immutable copy as a result with sort
			return new LiveProjectionQueryResult<TRoot, TReduce, T>(
				_query.OrderByDescending(sortExpression), _sortMappings);

		}

		/// <summary>
		/// Tries the resolve sort get expression.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="query">The query.</param>
		/// <returns>Sort expression or exception, if no mapping defined.</returns>
		private Expression<Func<TReduce, TKey>> TryResolveSortGetExpression<TKey>(Expression<Func<T, TKey>> query)
		{
			// get alias based on query
			string alias = GetAlias(query);

			// if no mapping defined throw exception
			if (!_sortMappings.ContainsKey(alias))
			{
				throw new Exception(string.Format("No sort mapping defined for '{0}'.", query));
			}

			//get alias
			var sortExpression = _sortMappings[alias];
			// convert as strong typed lambda
			return Expression.Lambda<Func<TReduce, TKey>>(sortExpression.Body, sortExpression.Parameters);
		}

		/// <summary>
		/// Counts of items in query.
		/// </summary>
		/// <returns>Counts of items in query.</returns>
		public int Count()
		{
			return _query.Count();
		}

		/// <summary>
		/// Return the result as array.
		/// !Note! there is predefined max number of items by RavenDB to protect server and connection from override!
		/// </summary>
		/// <returns>All items from query.</returns>
		public T[] ToArray()
		{
			return _query.As<T>().ToArray();
		}

		/// <summary>
		/// Return the paged result.
		/// </summary>
		/// <param name="startIndex">The start index (zero based).</param>
		/// <param name="itemsPerPage">The items per page.</param>
		/// <returns>All items in the specified page.</returns>
		public T[] ToPage(int startIndex, int itemsPerPage)
		{
			return _query.Skip(startIndex * itemsPerPage).Take(itemsPerPage).As<T>().ToArray();
		}

        /// <summary>
        /// To the custom page.
        /// </summary>
        /// <param name="skipItems">The skip items.</param>
        /// <param name="numberOfItems">The number of items.</param>
        /// <returns>Items after skiped number.</returns>
        public T[] ToCustomPage(int skipItems, int numberOfItems)
        {
            return _query.Skip(skipItems).Take(numberOfItems).As<T>().ToArray();
        }
	}
}
