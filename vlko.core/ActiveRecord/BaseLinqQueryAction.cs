using System;
using System.Linq;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Linq;
using GenericRepository;

namespace vlko.core.ActiveRecord
{

	public class BaseLinqQueryAction<T> : BaseAction<T>, IQueryAction<T> where T : class
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


