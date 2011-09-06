using vlko.core.NH.Repository;
using vlko.core.Repository;

namespace vlko.BlogModule.NH.Tests.Repository.IOCResolved.Queries
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
