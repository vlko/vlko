using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using vlko.core.Repository;

namespace vlko.core.NH.Repository
{
	/// <summary>
	/// Criterion query result.
	/// Please note, that CriterionQueryResult is immutable!
	/// </summary>
	public class CriterionQueryResult<T> : IQueryResult<T> where T : class
	{
		private readonly IQueryOver<T> _criteria;

		/// <summary>
		/// Initializes a new instance of the <see cref="CriterionQueryResult&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="criteria">The criteria.</param>
		public CriterionQueryResult(IQueryOver<T> criteria)
		{
			_criteria = criteria;
		}

		/// <summary>
		/// Orders the by query.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Order query result.</returns>
		public IQueryResult<T> OrderBy<TKey>(Expression<Func<T, TKey>> query)
		{
			return new CriterionQueryResult<T>(_criteria.Clone().OrderBy(Convert(query)).Asc);
		}


		/// <summary>
		/// Orders descending the by query.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Order query result.</returns>
		public IQueryResult<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> query)
		{
			return new CriterionQueryResult<T>(_criteria.Clone().OrderBy(Convert(query)).Desc);
		}


		/// <summary>
		/// Converts the specified query.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="query">The query.</param>
		private static Expression<Func<T, object>> Convert<TKey>(Expression<Func<T, TKey>> query)
		{
			return Expression.Lambda<Func<T, object>>(Expression.Convert(query.Body, typeof(object)), query.Parameters);
		}

		/// <summary>
		/// Counts of items in query.
		/// </summary>
		/// <returns>Counts of items in query.</returns>
		public int Count()
		{
			return _criteria.Clone().RowCount();
		}

		/// <summary>
		/// Return the result as array.
		/// </summary>
		/// <returns>All items from query.</returns>
		public T[] ToArray()
		{
			return _criteria.Clone().List().ToArray();
		}

		/// <summary>
		/// Return the paged result.
		/// </summary>
		/// <param name="startIndex">The start index.</param>
		/// <param name="itemsPerPage">The items per page.</param>
		/// <returns>All items in the specified page.</returns>
		public T[] ToPage(int startIndex, int itemsPerPage)
		{
			return _criteria.Clone().Skip(startIndex*itemsPerPage).Take(itemsPerPage).List().ToArray();
		}

	}
}


