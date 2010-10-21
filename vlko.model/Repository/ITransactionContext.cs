using System;

namespace vlko.model.Repository
{
	public interface ITransactionContext : IDisposable
	{
		void Commit();
		void Rollback();	
	}
}