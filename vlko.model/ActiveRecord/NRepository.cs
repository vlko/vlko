using GenericRepository;
using Castle.ActiveRecord;
using NotFoundException = GenericRepository.Exceptions.NotFoundException;

namespace vlko.model.ActiveRecord
{
    public class NRepository<T> : Repository<T> where T : class
    {
        /// <summary>
        /// Finds item by PK.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="throwOnNotFound">if set to <c>true</c> [throw exception on not found].</param>
        /// <returns>Item matching id or null/exception if not exists.</returns>
        public override T FindByPk(object id, bool throwOnNotFound)
        {
            try
            {
                return ActiveRecordMediator<T>.FindByPrimaryKey(id, throwOnNotFound);
            }
            catch (Castle.ActiveRecord.NotFoundException exception)
            {
                throw new NotFoundException(typeof(T), id, exception);
            }
        }

        /// <summary>
        /// Saves the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Saved item.</returns>
        public override T Save(T item)
        {
            return ActiveRecordMediator<T>.SaveCopy(item);
        }

        /// <summary>
        /// Creates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Created item.</returns>
        public override T Create(T item)
        {
            ActiveRecordMediator<T>.Create(item);
            return item;
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public override void Delete(T item)
        {
            ActiveRecordMediator<T>.Delete(item);
        }
    }
}


