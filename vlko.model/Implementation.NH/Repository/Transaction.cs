using Castle.ActiveRecord;
using vlko.core.Repository;

namespace vlko.model.Implementation.NH.Repository
{
	/// <summary>
	/// Active record transaction implementation.
	/// </summary>
	public class Transaction : TransactionScope, ITransaction
	{
		public ITransactionContext TransactionContext  { get; private set; }

		/// <summary>
		/// Inits the transaction context.
		/// </summary>
		/// <param name="transactionContext">The transaction context.</param>
		public void InitTransactionContext(ITransactionContext transactionContext)
		{
			TransactionContext = transactionContext;
		}

		/// <summary>
		/// Commits this instance.
		/// </summary>
		public void Commit()
		{
			VoteCommit();
			if (TransactionContext != null)
			{
				TransactionContext.Commit();
			}
		}

		/// <summary>
		/// Rollbacks this instance.
		/// </summary>
		public void Rollback()
		{
			VoteRollBack();
			if (TransactionContext != null)
			{
				TransactionContext.Rollback();
			}
		}

		/// <summary>
		/// Performs the disposal.
		/// </summary>
		/// <param name="sessions">The sessions.</param>
		protected override void PerformDisposal(System.Collections.Generic.ICollection<NHibernate.ISession> sessions)
		{
			 base.PerformDisposal(sessions);
			 if (TransactionContext != null)
			 {
				 TransactionContext.Dispose();
			 }
		}		
	}
}


