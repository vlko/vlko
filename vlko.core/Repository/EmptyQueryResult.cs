using System;
using System.Linq.Expressions;

namespace vlko.core.Repository
{
	public class EmptyQueryResult<T> : IQueryResult<T>
	{
		/// <summary>
		/// Orders by the query.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Ordered IQueryAction.</returns>
		public IQueryResult<T> OrderBy<TKey>(Expression<Func<T, TKey>> query)
		{
			return this;
		}

		/// <summary>
		/// Orders by descending the query.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Ordered IQueryAction.</returns>
		public IQueryResult<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> query)
		{
			return this;
		}

		/// <summary>
		/// Counts of items in query.
		/// </summary>
		/// <returns>Counts of items in query.</returns>
		public int Count()
		{
			return 0;
		}

		/// <summary>
		/// Return the result as array.
		/// </summary>
		/// <returns>All items from query.</returns>
		public T[] ToArray()
		{
			return new T[] {};
		}

		/// <summary>
		/// Return the paged result.
		/// </summary>
		/// <param name="startIndex">The start index (zero based).</param>
		/// <param name="itemsPerPage">The items per page.</param>
		/// <returns>All items in the specified page.</returns>
		public T[] ToPage(int startIndex, int itemsPerPage)
		{
			return new T[] { };
		}

        /// <summary>
        /// To the custom page.
        /// </summary>
        /// <param name="skipItems">The skip items.</param>
        /// <param name="numberOfItems">The number of items.</param>
        /// <returns>Items after skiped number.</returns>
	    public T[] ToCustomPage(int skipItems, int numberOfItems)
	    {
            return new T[] { };
	    }
	}
}
