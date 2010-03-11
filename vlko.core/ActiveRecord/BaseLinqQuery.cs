using System.Linq;
using Castle.ActiveRecord.Linq;
using GenericRepository;

namespace vlko.core.ActiveRecord
{

    public class BaseLinqQuery<T> : IQuery<T> where T : class
    {
        /// <summary>
        /// Gets or sets the queryable.
        /// </summary>
        /// <value>The queryable.</value>
        protected IQueryable<T> Queryable { get; set; }

        /// <summary>
        /// Initializes query with the specified repository.
        /// </summary>
        /// <param name="initializeContext">The initialize context.</param>
        public void Initialize(QueryInitializeContext<T> initializeContext)
        {
            Queryable = ActiveRecordLinq.AsQueryable<T>();
        }

        /// <summary>
        /// Get query result.
        /// </summary>
        /// <returns>Query result.</returns>
        public virtual IQueryResult<T> Result()
        {
            return new QueryLinqResult<T>(Queryable);
        }
    }
}


