using System;
using GenericRepository.Exceptions;

namespace GenericRepository
{
    /// <summary>
    /// Static IoC resolver class.
    /// </summary>
    public static class RepositoryIoC
    {

        private static IRepositoryIoCResolver _ioCResolver;

        /// <summary>
        /// Gets the IoC resolver.
        /// </summary>
        /// <value>The IoC resolver.</value>
        public static IRepositoryIoCResolver IoCResolver
        {
            get
            {
                if (_ioCResolver == null)
                {
                    Exception ex = new RepositoryIoCNotInitializeException();
                    throw ex;
                }
                return _ioCResolver;
            }

        }

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public static bool IsInitialized
        {
            get
            {
                return _ioCResolver != null;
            }
        }

        /// <summary>
        /// Intitialize this static instance the with specified ioCResolver.
        /// </summary>
        /// <param name="ioCResolver">The IoC resolver.</param>
        public static void IntitializeWith(IRepositoryIoCResolver ioCResolver)
        {
            _ioCResolver = ioCResolver;
        }


        /// <summary>
        /// Starts the unit of work.
        /// </summary>
        /// <returns>New unit of work.</returns>
        public static IUnitOfWork StartUnitOfWork()
        {
            return IoCResolver.GetUnitOfWork();
        }

        /// <summary>
        /// Starts the transaction.
        /// </summary>
        /// <returns>New transaction.</returns>
        public static ITransaction StartTransaction()
        {
            return IoCResolver.GetTransaction();
        }

        /// <summary>
        /// Gets the BaseRepository.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Registered BaseRepository.</returns>
        public static BaseRepository<T> GetRepository<T>() where T : class
        {
            return IoCResolver.GetRepository<T>();
        }
    }
}
