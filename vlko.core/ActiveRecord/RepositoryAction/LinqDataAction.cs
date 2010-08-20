using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Castle.ActiveRecord.Linq;
using GenericRepository;
using GenericRepository.RepositoryAction;

namespace vlko.core.ActiveRecord.RepositoryAction
{
	public class LinqDataAction<T> : BaseLinqQueryAction<T>, ILinqDataAction<T> where T : class
	{
		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns>Returns query result set.</returns>
		public IQueryResult<T> GetAll()
		{
			return Result(Queryable);
		}

		/// <summary>
		/// Gets the where.
		/// </summary>
		/// <param name="whereCondition">The where condition.</param>
		/// <returns>Returns query result set with applied where condition.</returns>
		public IQueryResult<T> GetWhere(Expression<Func<T, bool>> whereCondition)
		{
			return Result(Queryable.Where(whereCondition));
		}
	}
}
