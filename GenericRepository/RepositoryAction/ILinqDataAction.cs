using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GenericRepository.RepositoryAction
{
	public interface ILinqDataAction<T> : IAction<T> where T : class
	{
		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns>Returns query result set.</returns>
		IQueryResult<T> GetAll();

		/// <summary>
		/// Gets the where.
		/// </summary>
		/// <param name="whereCondition">The where condition.</param>
		/// <returns>Returns query result set with applied where condition.</returns>
		IQueryResult<T> GetWhere(Expression<Func<T, bool>> whereCondition);
	}
}
