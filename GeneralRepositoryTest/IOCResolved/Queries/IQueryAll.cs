using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericRepository;

namespace GeneralRepositoryTest.IOCResolved.Queries
{
    public interface IQueryAll<T> : IQuery<T> where T : class
    {
        /// <summary>
        /// Executes query.
        /// </summary>
        /// <returns>Query result.</returns>
        IQueryResult<T> Execute();
    }
}
