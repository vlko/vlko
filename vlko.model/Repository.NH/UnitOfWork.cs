using Castle.ActiveRecord;

namespace vlko.model.Repository.NH
{
	/// <summary>
	/// Session implementation for Active record.
	/// </summary>
	public class UnitOfWork : SessionScope, IUnitOfWork
	{
		public UnitOfWork()
			: base(FlushAction.Never)
		{
		}

		public IUnitOfWorkContext UnitOfWorkContext { get; private set; }

		/// <summary>
		/// Inits the unit of work context.
		/// </summary>
		/// <param name="unitOfWorkContext">The unit of work context.</param>
		public void InitUnitOfWorkContext(IUnitOfWorkContext unitOfWorkContext)
		{
			UnitOfWorkContext = unitOfWorkContext;
		}
		/// <summary>
		/// Performs the disposal.
		/// </summary>
		/// <param name="sessions">The sessions.</param>
		protected override void PerformDisposal(System.Collections.Generic.ICollection<NHibernate.ISession> sessions)
		{
			base.PerformDisposal(sessions);
			if (UnitOfWorkContext != null)
			{
				UnitOfWorkContext.Dispose();
			}
		}	
	}
}


