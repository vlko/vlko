using vlko.core.ActiveRecord;

namespace GeneralRepositoryTest.IOCResolved.Queries
{
    public class QueryAllCriterion<T> : BaseLinqQuery<T>, IQueryAll<T> where T : class
    {
        /// <summary>
        /// Executes query.
        /// </summary>
        /// <returns>Query result.</returns>
        public GenericRepository.IQueryResult<T> Execute()
        {
            return Result();
        }
    }
}
