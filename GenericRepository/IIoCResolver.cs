namespace GenericRepository
{
    public interface IRepositoryIoCResolver
    {
        /// <summary>
        /// Gets the BaseRepository.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Registered BaseRepository for type.</returns>
        BaseRepository<T> GetRepository<T>() where T : class; 

        /// <summary>
        /// Resolves the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Resolved query.</returns>
        T ResolveQuery<T>() where T : class;
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
