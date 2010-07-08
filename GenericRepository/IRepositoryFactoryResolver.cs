namespace GenericRepository
{
	public interface IRepositoryFactoryResolver
	{
		/// <summary>
		/// Gets the BaseRepository.
		/// </summary>
		/// <typeparam name="T">Generic type.</typeparam>
		/// <returns>Registered BaseRepository for type.</returns>
		BaseRepository<T> GetRepository<T>() where T : class; 

		/// <summary>
		/// Resolves the action or query.
		/// </summary>
		/// <typeparam name="T">Generic type.</typeparam>
		/// <returns>Resolved action or query.</returns>
		T ResolveAction<T>() where T : class;

		/// <summary>
		/// Gets the unit of work.
		/// </summary>
		/// <returns>New unit of work.</returns>
		IUnitOfWork GetUnitOfWork();

		/// <summary>
		/// Gets the transaction.
		/// </summary>
		/// <returns>New transaction.</returns>
		ITransaction GetTransaction();

	}
}
