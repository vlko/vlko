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
        /// <exception cref="RepositoryFactoryNotInitializeException">BaseRepository not initialized.</exception>
        /// <exception cref="QueryNotRegisteredException">If type TQType not registered in BaseRepository.</exception>
        public TQType GetQuery<TQType>() where TQType : class, IQueryAction<T>
        {
            var query = GetInternalAction<TQType>();
            if (query == null)
            {
                throw new QueryNotRegisteredException(typeof(TQType), this.GetType(), typeof(T));
            }
            return query;
        }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <typeparam name="TAType">The type of the action.</typeparam>
        /// <returns>Query.</returns>
        /// <exception cref="RepositoryFactoryNotInitializeException">BaseRepository not initialized.</exception>
        /// <exception cref="ActionNotRegisteredException">If type TAType not registered in BaseRepository.</exception>
        public TAType GetAction<TAType>() where TAType : class, IAction<T>
        {
            var action = GetInternalAction<TAType>();
            if (action == null)
            {
                throw new ActionNotRegisteredException(typeof(TAType), this.GetType(), typeof(T));
            }

            return action;
        }

        /// <summary>
        /// Gets the internal action.
        /// </summary>
        /// <typeparam name="TAType">The type of the Action.</typeparam>
        /// <returns>Action</returns>
        private TAType GetInternalAction<TAType>() where TAType : class, IAction<T>
        {
            var action = RepositoryFactory.FactoryResolver.ResolveAction<TAType>();
            if (action != null)
            {
                if (!action.Initialized)
                {
                    InitalizeAction(action);
                }
            }
            return action;
        }

        /// <summary>
        /// Initalizes the queryAction.
        /// </summary>
        /// <param name="action">The action.</param>
        public virtual void InitalizeAction(IAction<T> action)
        {
            action.Initialize(new InitializeContext<T>(this));
        }
    }
}
