namespace GenericRepository
{
	public interface ITransaction : ITransactionContext
	{
		ITransactionContext TransactionContext { get; }
		void InitTransactionContext(ITransactionContext transactionContext);
	}
}
