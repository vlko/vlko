using GenericRepository;
using NHibernate.Criterion;

namespace vlko.model.ActiveRecord
{
    public class BaseCriterionQuery<T> : IQuery<T> where T : class
    {

        /// <summary>
        /// Gets or sets the criteria.
        /// </summary>
        /// <value>The criteria.</value>
        protected DetachedCriteria Criteria { get; set; }

        /// <summary>
        /// Initializes query with the specified repository.
        /// </summary>
        /// <param name="initializeContext">The initialize context.</param>
        public void Initialize(QueryInitializeContext<T> initializeContext)
        {
            Criteria = DetachedCriteria.For<T>();
        }

        /// <summary>
        /// Get query result.
        /// </summary>
        /// <returns>Query result.</returns>
        public virtual IQueryResult<T> Result()
        {
            return new CriterionQueryResult<T>(Criteria);
        }
    }
}


