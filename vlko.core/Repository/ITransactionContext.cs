using System;
using System.ComponentModel.Composition;

namespace vlko.core.Repository
{
	public interface ITransactionContext : IDisposable
	{
		void Commit();
		void Rollback();	
	}
}