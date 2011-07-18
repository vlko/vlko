using vlko.core.InversionOfControl;
using vlko.core.Repository;

namespace vlko.BlogModule.NH.Repository   
{
	public class RepositoryFactoryResolver : IRepositoryFactoryResolver
	{
		/// <summary>
		/// Resolves the action or query.
		/// </summary>
		/// <typeparam name="T">Generic type.</typeparam>
		/// <returns>Resolved action or query.</returns>
		public T ResolveAction<T>() where T : class, IAction
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
		/// <returns>New transaction.</returns>
		public ITransaction GetTransaction()
		{
			return IoC.Resolve<ITransaction>();
		}


		/// <summary>
		/// Gets the BaseRepository.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>Registered BaseRepository for type.</returns>
		public IRepository<T> GetRepository<T>() where T : class
		{
			return new Repository<T>();
		}

	}
}


