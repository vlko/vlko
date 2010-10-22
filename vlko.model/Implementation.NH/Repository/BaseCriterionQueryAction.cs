using NHibernate.Criterion;
using vlko.model.Repository;

namespace vlko.model.Implementation.NH.Repository
{
	public class BaseCriterionQueryAction<T> : BaseAction<T>, IQueryAction<T> where T : class
	{

		/// <summary>
		/// Gets or sets the criteria.
		/// </summary>
		/// <value>The criteria.</value>
		protected DetachedCriteria Criteria { get; set; }

		/// <summary>
		/// Initializes queryAction with the specified repository.
		/// </summary>
		/// <param name="initializeContext">The initialize context.</param>
		public override void Initialize(InitializeContext<T> initializeContext)
		{
			Criteria = DetachedCriteria.For<T>();
			base.Initialize(initializeContext);
		}

		/// <summary>
		/// Get queryAction result.
		/// </summary>
		/// <returns>Query result.</returns>
		public virtual IQueryResult<T> Result()
		{
			return new CriterionQueryResult<T>(Criteria);
		}
	}
}


