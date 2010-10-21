namespace vlko.model.Repository
{
	public interface ITransaction : ITransactionContext
	{
		ITransactionContext TransactionContext { get; }
		void InitTransactionContext(ITransactionContext transactionContext);
	}
}
