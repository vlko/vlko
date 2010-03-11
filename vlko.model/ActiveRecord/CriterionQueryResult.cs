using System;
using System.Linq.Expressions;
using Castle.ActiveRecord;
using GenericRepository;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.LambdaExtensions;

namespace vlko.model.ActiveRecord
{
    /// <summary>
    /// Criterion query result.
    /// Please note, that CriterionQueryResult is immutable!
    /// </summary>
    public class CriterionQueryResult<T> : IQueryResult<T> where T : class
    {
        private readonly DetachedCriteria _criteria;

        /// <summary>
        /// Initializes a new instance of the <see cref="CriterionQueryResult&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public CriterionQueryResult(DetachedCriteria criteria)
        {
            _criteria = criteria;
        }

        /// <summary>
        /// Orders the by query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Order query result.</returns>
        public IQueryResult<T> OrderBy(Expression<Func<T, object>> query)
        {
            return new CriterionQueryResult<T>(CriteriaTransformer.Clone(_criteria).AddOrder(query, Order.Asc));
        }

        /// <summary>
        /// Orders descending the by query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Order query result.</returns>
        public IQueryResult<T> OrderByDescending(Expression<Func<T, object>> query)
        {
            return new CriterionQueryResult<T>(CriteriaTransformer.Clone(_criteria).AddOrder(query, Order.Desc));
        }

        /// <summary>
        /// Counts of items in query.
        /// </summary>
        /// <returns>Counts of items in query.</returns>
        public int Count()
        {
            return ActiveRecordMediator<T>.Count(CriteriaTransformer.Clone(_criteria));
        }

        /// <summary>
        /// Return the result as array.
        /// </summary>
        /// <returns>All items from query.</returns>
        public T[] ToArray()
        {
            return ActiveRecordMediator<T>.FindAll(CriteriaTransformer.Clone(_criteria));
        }

        /// <summary>
        /// Return the paged result.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <returns>All items in the specified page.</returns>
        public T[] ToPage(int startIndex, int itemsPerPage)
        {
            return ActiveRecordMediator<T>.SlicedFindAll(startIndex * itemsPerPage, itemsPerPage, CriteriaTransformer.Clone(_criteria));
        }

    }
}


