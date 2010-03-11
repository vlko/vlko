using GenericRepository;

namespace vlko.model.IoC
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
            return IoC.Resolve<T>();
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <returns></returns>
        public IUnitOfWork GetUnitOfWork()
        {
            return IoC.Resolve<IUnitOfWork>();
        }

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        /// <returns></returns>
        public ITransaction GetTransaction()
        {
            return IoC.Resolve<ITransaction>();
        }

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Registered repository for type.</returns>
        public Repository<T> GetRepository<T>() where T : class
        {
            return IoC.Resolve<Repository<T>>();
        }

    }
}


