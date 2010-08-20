using Castle.ActiveRecord;
using GenericRepository;
using GenericRepository.RepositoryAction;
using GenericRepository.Exceptions;
using NotFoundException = GenericRepository.Exceptions.NotFoundException;

namespace vlko.core.ActiveRecord.RepositoryAction
{
    public class CRUDActions<T> : BaseAction<T>, ICRUDActions<T>, IFindByPkAction<T>, ICreateAction<T>, ISaveAction<T>, IDeleteAction<T> where T : class
    {

        /// <summary>
        /// Finds the by PK.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// Item matching id or exception if not exists.
        /// </returns>
        /// <exception cref="NotFoundException">If matching id was not found.</exception>
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
        public T FindByPk(object id, bool throwOnNotFound)
        {
            try
            {
                return ActiveRecordMediator<T>.FindByPrimaryKey(id, throwOnNotFound);
            }
            catch (Castle.ActiveRecord.NotFoundException exception)
            {
                if (throwOnNotFound)
                {
                    throw new NotFoundException(typeof (T), id, exception);
                }
            }
            return null;
        }

        /// <summary>
        /// Saves the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Saved item.</returns>
        public T Save(T item)
        {
            return ActiveRecordMediator<T>.SaveCopy(item);
        }

        /// <summary>
        /// Creates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Created item.</returns>
        public T Create(T item)
        {
            ActiveRecordMediator<T>.Create(item);
            return item;
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public  void Delete(T item)
        {
            ActiveRecordMediator<T>.Delete(item);
        }
    }
}
