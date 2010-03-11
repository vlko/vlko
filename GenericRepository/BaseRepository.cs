using System;
using GenericRepository.Exceptions;

namespace GenericRepository
{
    public abstract class BaseRepository<T> where T : class
    {

        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="TQType">The type of the query.</typeparam>
        /// <returns>Query.</returns>
        /// <exception cref="RepositoryIoCNotInitializeException">BaseRepository not initialized.</exception>
        /// <exception cref="QueryNotRegisteredException">If type TQType not registered in BaseRepository.</exception>
        public TQType GetQuery<TQType>() where TQType : class, IQuery<T>
        {
            var query = RepositoryIoC.IoCResolver.ResolveQuery<TQType>();
            if (query == null)
            {
                throw new QueryNotRegisteredException(typeof(TQType), this.GetType(), typeof(T));
            }
            InitalizeQuery(query);
            return query;
        }

        /// <summary>
        /// Initalizes the query.
        /// </summary>
        /// <param name="query">The query.</param>
        protected virtual void InitalizeQuery(IQuery<T> query)
        {
            query.Initialize(new QueryInitializeContext<T>(this));
        }


        /// <summary>
        /// Finds the by PK.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Item matching id or exception if not exists.</returns>
        public T FindByPk(object id)
        {
            return FindByPk(id, true);
        }

        /// <summary>
        /// Finds item by PK.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="throwOnNotFound">if set to <c>true</c> [throw exception on not found].</param>
        /// <returns>Item matching id or null/exception if not exists.</returns>
        public abstract T FindByPk(object id, bool throwOnNotFound);

        /// <summary>
        /// Saves the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Saved item.</returns>
        public abstract T Save(T item);

        /// <summary>
        /// Creates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Created item.</returns>
        public abstract T Create(T item);

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public abstract void Delete(T item);
    }
}
