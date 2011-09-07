using System.Linq;
using vlko.core.Repository;

namespace vlko.core.NH.Repository
{

	public class LinqQuery<T> : CommandGroup<T>, ICommandGroup<T> where T : class
	{
		/// <summary>
		/// Gets the queryable.
		/// </summary>
		/// <value>The queryable.</value>
		protected IQueryable<T> Queryable
		{
			get
			{
				return SessionFactory<T>.Queryable;
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


