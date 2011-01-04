using vlko.BlogModule.NH.Repository;
using vlko.core.Repository;

namespace vlko.BlogModule.Tests.Repository.IOCResolved.Queries
{
    public class QueryActionAllCriterion<T> : BaseLinqQueryAction<T>, IQueryActionAll<T> where T : class
    {
        /// <summary>
        /// Executes queryAction.
        /// </summary>
        /// <returns>Query result.</returns>
        public IQueryResult<T> Execute()
        {
        	return Result(Queryable);
        }
    }
}
