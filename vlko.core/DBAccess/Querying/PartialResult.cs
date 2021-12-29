using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vlko.core.DBAccess.Querying
{
    /// <summary>
    /// Page query result data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PartialResult<T> where T : class
    {
        public PartialResult(IEnumerable<T> pageData, int totalCount)
        {
            PageData = pageData;
            TotalCount = totalCount;
        }
        public IEnumerable<T> PageData { get; private set; }
        public int TotalCount { get; private set; }
    }
}
