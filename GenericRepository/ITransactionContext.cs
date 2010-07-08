using System;

namespace GenericRepository
{
	public interface ITransactionContext : IDisposable
	{
		void Commit();
		void Rollback();	
	}
}