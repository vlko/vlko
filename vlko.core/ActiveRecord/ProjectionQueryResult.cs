using System;
using System.Linq;
using System.Linq.Expressions;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Queries;
using GenericRepository;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.LambdaExtensions;

namespace vlko.core.ActiveRecord
{
    /// <summary>
    /// Criterion query result.
    /// Please note, that ProjectionQueryResult is immutable!
    /// </summary>
    public class ProjectionQueryResult<ARType, T> : IQueryResult<T> where T : class where ARType : class
    {
        private readonly DetachedCriteria _criteria;

        private readonly ProjectionList _projectionList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionQueryResult&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        public ProjectionQueryResult(DetachedCriteria criteria, ProjectionList projectionList)
        {
            _criteria = criteria;
            _projectionList = projectionList;
        }

        /// <summary>
        /// Orders the by query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Order query result.</returns>
        public IQueryResult<T> OrderBy(Expression<Func<T, object>> query)
        {
            return new ProjectionQueryResult<ARType, T>(CriteriaTransformer.Clone(_criteria)
                .AddOrder(new Order(GetProjectionFromOrderQuery(query, _projectionList), true)),
                _projectionList);
        }

        /// <summary>
        /// Orders descending the by query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Order query result.</returns>
        public IQueryResult<T> OrderByDescending(Expression<Func<T, object>> query)
        {
            return new ProjectionQueryResult<ARType, T>(CriteriaTransformer.Clone(_criteria)
                .AddOrder(new Order(GetProjectionFromOrderQuery(query, _projectionList), false)),
                _projectionList);
        }

        /// <summary>
        /// Gets the projection from order query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="projectionList">The projection list.</param>
        /// <returns>Projection based on query.</returns>
        private static IProjection GetProjectionFromOrderQuery(Expression<Func<T, object>> query, ProjectionList projectionList)
        {
            string alias = string.Empty;
            if (((LambdaExpression)query).Body is UnaryExpression)
            {
                alias = ((MemberExpression)((UnaryExpression)((LambdaExpression)query).Body).Operand).Member.Name;
            }
            else
            {
                alias = ((MemberExpression) ((LambdaExpression) query).Body).Member.Name;
            }
            for (int i = 0; i < projectionList.Aliases.Length; i++)
            {
                var reducatedAlias = projectionList.Aliases[i];
                if (reducatedAlias.Contains('.'))
                {
                    reducatedAlias = reducatedAlias.Remove(0, reducatedAlias.IndexOf('.') + 1);
                }
                if (reducatedAlias == alias)
                {
                    return projectionList[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Counts of items in query.
        /// </summary>
        /// <returns>Counts of items in query.</returns>
        public int Count()
        {
            return ActiveRecordMediator<ARType>.Count(CriteriaTransformer.TransformToRowCount(
                CriteriaTransformer.Clone(_criteria)));
        }

        /// <summary>
        /// Return the result as array.
        /// </summary>
        /// <returns>All items from query.</returns>
        public T[] ToArray()
        {
            var query = new ProjectionQuery<ARType, T>(CriteriaTransformer.Clone(_criteria), _projectionList);
            return query.Execute().ToArray();
        }

        /// <summary>
        /// Return the paged result.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="itemsPerPage">The items per page.</param>
        /// <returns>All items in the specified page.</returns>
        public T[] ToPage(int startIndex, int itemsPerPage)
        {
            var query = new ProjectionQuery<ARType, T>(CriteriaTransformer.Clone(_criteria), _projectionList);
            query.SetRange(startIndex*itemsPerPage, itemsPerPage);
            return query.Execute().ToArray();
        }

    }
}


