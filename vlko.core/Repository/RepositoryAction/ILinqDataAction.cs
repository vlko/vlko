using System;
using System.Linq.Expressions;

namespace vlko.core.Repository.RepositoryAction
{
	public interface ILinqQueries<T> : ICommandGroup<T> where T : class
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
