using vlko.model.Repository;

namespace vlko.model.Tests.Repository.IOCResolved.Queries
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
