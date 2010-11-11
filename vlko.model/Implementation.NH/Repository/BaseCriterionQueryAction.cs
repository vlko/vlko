using NHibernate.Criterion;
using vlko.core.Repository;

namespace vlko.model.Implementation.NH.Repository
{
	public class BaseCriterionQueryAction<T> : BaseAction<T>, IAction<T> where T : class
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
		public override void Initialize(IInitializeContext<T> initializeContext)
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


