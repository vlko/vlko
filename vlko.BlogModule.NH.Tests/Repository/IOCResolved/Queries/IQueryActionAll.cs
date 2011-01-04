using vlko.core.Repository;

namespace vlko.BlogModule.Tests.Repository.IOCResolved.Queries
{
    public interface IQueryActionAll<T> : IAction<T> where T : class
    {
        /// <summary>
        /// Executes queryAction.
        /// </summary>
        /// <returns>Query result.</returns>
        IQueryResult<T> Execute();
    }
}
