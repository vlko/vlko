using System;

namespace vlko.core.Repository
{
	public interface ITransactionContext : IDisposable
	{
		void Commit();
		void Rollback();	
	}
}