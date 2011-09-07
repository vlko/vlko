using vlko.core.Repository;
using vlko.core.Repository.RepositoryAction;

namespace vlko.core.NH.Repository.RepositoryAction
{
	public class CrudCommands<T> : CommandGroup<T>, ICrudCommands<T>, IFindByPkCommand<T>, ICreateCommand<T>, IUpdateCommand<T>, IDeleteCommand<T> where T : class
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
			return SessionFactory<T>.FindByPrimaryKey(id, throwOnNotFound);
		}

		/// <summary>
		/// Saves the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Saved item.</returns>
		public T Update(T item)
		{
			SessionFactory<T>.Update(item);
			return item;
		}

		/// <summary>
		/// Creates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Created item.</returns>
		public T Create(T item)
		{
			SessionFactory<T>.Create(item);
			return item;
		}

		/// <summary>
		/// Deletes the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public  void Delete(T item)
		{
			SessionFactory<T>.Delete(item);
		}
	}
}
