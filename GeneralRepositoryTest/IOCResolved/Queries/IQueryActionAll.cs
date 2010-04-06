using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericRepository;

namespace GeneralRepositoryTest.IOCResolved.Queries
{
    public interface IQueryActionAll<T> : IQueryAction<T> where T : class
    {
        /// <summary>
        /// Executes queryAction.
        /// </summary>
        /// <returns>Query result.</returns>
        IQueryResult<T> Execute();
    }
}
