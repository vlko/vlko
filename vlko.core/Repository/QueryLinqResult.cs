using System;
using System.Linq;
using System.Linq.Expressions;

namespace vlko.core.Repository
{
    /// <summary>
    /// Default implementation using IQueryable.
    /// Please note, that QueryResult is immutable!
    /// </summary>
    public class QueryLinqResult<T> : IQueryResult<T>
    {
        private readonly IQueryable<T> _queryable;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="queryable">The queryable.</param>
        public QueryLinqResult(IQueryable<T> queryable)
        {
            _queryable = queryable;
        }

        /// <summary>
        /// Gets the new instance.
        /// </summary>
        /// <param name="queryable">The queryable.</param>
        /// <returns></returns>
        protected virtual QueryLinqResult<T> GetNewInstance(IQueryable<T> queryable)
        {
            return new QueryLinqResult<T>(queryable);
        }

        /// <summary>
        /// Orders by the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Ordered IQueryAction.</returns>
		public IQueryResult<T> OrderBy<TKey>(Expression<Func<T, TKey>> query)
        {
            return GetNewInstance(_queryable.OrderBy(query));
        }
        /// <summary>
        /// Orders by descending the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Ordered IQueryAction.</returns>
        public IQueryResult<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> query)
        {
            return GetNewInstance(_queryable.OrderByDescending(query));
        }
        /// <summary>
        /// Counts of items in query.
        /// </summary>
        /// <returns>Counts of items in query.</returns>
        public virtual int Count()
        {
            return _queryable.Count();
        }
        /// <summary>
        /// Return the result as array.
        /// </summary>
        /// <returns>All items from query.</returns>
        public virtual T[] ToArray()
        {
            return _queryable.ToArray();
        }

        /// <summary>
        /// Return the paged result.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <returns>All items in the specified page.</returns>
        public virtual T[] ToPage(int startIndex, int itemsPerPage)
        {
            return _queryable.Skip(startIndex * itemsPerPage).Take(itemsPerPage).ToArray();
        }
    }
}
