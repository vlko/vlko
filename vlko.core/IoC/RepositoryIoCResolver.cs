using GenericRepository;

namespace vlko.core.IoC   
{
    public class RepositoryIoCResolver : IRepositoryIoCResolver
    {
        /// <summary>
        /// Resolves the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ResolveQuery<T>() where T : class
        {
            return model.IoC.IoC.Resolve<T>();
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <returns></returns>
        public IUnitOfWork GetUnitOfWork()
        {
            return model.IoC.IoC.Resolve<IUnitOfWork>();
        }

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        /// <returns></returns>
        public ITransaction GetTransaction()
        {
            return model.IoC.IoC.Resolve<ITransaction>();
        }

        /// <summary>
        /// Gets the BaseRepository.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Registered BaseRepository for type.</returns>
        public BaseRepository<T> GetRepository<T>() where T : class
        {
            return model.IoC.IoC.Resolve<BaseRepository<T>>();
        }

    }
}


