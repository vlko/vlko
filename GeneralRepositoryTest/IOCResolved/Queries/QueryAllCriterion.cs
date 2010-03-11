using vlko.model.ActiveRecord;

namespace GeneralRepositoryTest.IOCResolved.Queries
{
    public class QueryAllLinq<T> : BaseCriterionQuery<T>, IQueryAll<T> where T : class
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
