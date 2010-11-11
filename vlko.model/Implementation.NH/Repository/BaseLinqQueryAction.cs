using System.Linq;
using Castle.ActiveRecord.Framework;
using vlko.core.Repository;

namespace vlko.model.Implementation.NH.Repository
{

	public class BaseLinqQueryAction<T> : BaseAction<T>, IAction<T> where T : class
	{
		/// <summary>
		/// Gets the queryable.
		/// </summary>
		/// <value>The queryable.</value>
		protected IQueryable<T> Queryable
		{
			get
			{
				return ActiveRecordLinqBase<T>.Queryable;
			}
		}


		/// <summary>
		/// Get queryAction result.
		/// </summary>
		/// <param name="queryable">The queryable.</param>
		/// <returns>Query result.</returns>
		public static IQueryResult<T> Result(IQueryable<T> queryable)
		{
			return new QueryLinqResult<T>(queryable);
		}
	}
}


