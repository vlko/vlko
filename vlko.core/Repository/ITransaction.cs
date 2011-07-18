using System.ComponentModel.Composition;

namespace vlko.core.Repository
{
	[InheritedExport]
	public interface ITransaction : ITransactionContext
	{
		ITransactionContext TransactionContext { get; }
		void InitTransactionContext(ITransactionContext transactionContext);
	}
}
