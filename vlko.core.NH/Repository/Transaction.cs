using System;
using System.ComponentModel.Composition;
using vlko.core.Repository;
using ITransaction = vlko.core.Repository.ITransaction;

namespace vlko.core.NH.Repository
{
	/// <summary>
	/// Active record transaction implementation.
	/// </summary>
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public sealed class Transaction : ITransaction
	{


		internal object LockObject { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="UnitOfWork"/> class.
		/// </summary>
		public Transaction()
		{
			SessionFactory.RegisterTransaction(this);
		}

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
			SessionFactory.CommitTransaction(this);
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
			SessionFactory.RollbackTransaction(this);
			if (TransactionContext != null)
			{
				TransactionContext.Rollback();
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="UnitOfWork"/> is reclaimed by garbage collection.
		/// </summary>
		~Transaction()
		{
			Dispose(false);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				try
				{

					SessionFactory.UnregisterTransaction(this);
				}
				finally
				{
					LockObject = null;
					if (TransactionContext != null)
					{
						TransactionContext.Dispose();
					}
				}
			}
		}
	}
}


