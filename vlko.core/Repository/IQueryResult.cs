using System;
using System.Linq.Expressions;

namespace vlko.core.Repository
{
    public interface IQueryResult<T>
    {
        /// <summary>
        /// Orders by the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Ordered IQueryAction.</returns>
		IQueryResult<T> OrderBy<TKey>(Expression<Func<T, TKey>> query);

        /// <summary>
        /// Orders by descending the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Ordered IQueryAction.</returns>
		IQueryResult<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> query);

        /// <summary>
        /// Counts of items in query.
        /// </summary>
        /// <returns>Counts of items in query.</returns>
        int Count();

        /// <summary>
        /// Return the result as array.
        /// </summary>
        /// <returns>All items from query.</returns>
        T[] ToArray();

        /// <summary>
        /// Return the paged result.
        /// </summary>
        /// <param name="startIndex">The start index (zero based).</param>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <returns>All items in the specified page.</returns>
        T[] ToPage(int startIndex, int itemsPerPage);
    }
}
