using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using vlko.core.Repository;

namespace vlko.model.Implementation.NH.Repository
{
	/// <summary>
	/// Criterion query result.
	/// Please note, that ProjectionQueryResult is immutable!
	/// </summary>
	public class ProjectionQueryResult<ARType, T> : IQueryResult<T>
		where T : class
		where ARType : class
	{
		private readonly IQueryOver<ARType, ARType> _criteria;

		private readonly ProjectionList _projectionList;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectionQueryResult&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="criteria">The criteria.</param>
		public ProjectionQueryResult(IQueryOver<ARType, ARType> criteria, ProjectionList projectionList)
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
			return new ProjectionQueryResult<ARType, T>(_criteria.Clone()
				.OrderBy(GetProjectionFromOrderQuery(query, _projectionList)).Asc,
				_projectionList);
		}

		/// <summary>
		/// Orders descending the by query.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Order query result.</returns>
		public IQueryResult<T> OrderByDescending(Expression<Func<T, object>> query)
		{
			return new ProjectionQueryResult<ARType, T>(_criteria.Clone()
				.OrderBy(GetProjectionFromOrderQuery(query, _projectionList)).Desc,
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
				alias = ((MemberExpression)((LambdaExpression)query).Body).Member.Name;
			}
			for (int i = 0; i < projectionList.Aliases.Length; i++)
			{
				var reducatedAlias = projectionList.Aliases[i];
				if (reducatedAlias.Contains("."))
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
			return _criteria.Clone().RowCount();
		}

		/// <summary>
		/// Return the result as array.
		/// </summary>
		/// <returns>All items from query.</returns>
		public T[] ToArray()
		{
			return _criteria.Clone()
				.Select(_projectionList)
				.TransformUsing(Transformers.AliasToBean<T>())
				.List<T>()
				.ToArray();
		}

		/// <summary>
		/// Return the paged result.
		/// </summary>
		/// <param name="startIndex">The start index.</param>
		/// <param name="itemsPerPage">The items per page.</param>
		/// <returns>All items in the specified page.</returns>
		public T[] ToPage(int startIndex, int itemsPerPage)
		{
			return _criteria.Clone()
				.Select(_projectionList)
				.TransformUsing(Transformers.AliasToBean<T>())
				.Skip(startIndex * itemsPerPage).Take(itemsPerPage)
				.List<T>()
				.ToArray();
		}

	}
}


