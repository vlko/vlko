using GenericRepository;

namespace vlko.core.InversionOfControl   
{
	public class RepositoryFactoryResolver : IRepositoryFactoryResolver
	{
		/// <summary>
		/// Resolves the action or query.
		/// </summary>
		/// <typeparam name="T">Generic type.</typeparam>
		/// <returns>Resolved action or query.</returns>
		public T ResolveAction<T>() where T : class
		{
			return InversionOfControl.IoC.Resolve<T>();
		}

		/// <summary>
		/// Gets the session.
		/// </summary>
		/// <returns></returns>
		public IUnitOfWork GetUnitOfWork()
		{
			return InversionOfControl.IoC.Resolve<IUnitOfWork>();
		}

		/// <summary>
		/// Gets the transaction.
		/// </summary>
		/// <returns>New transaction.</returns>
		public ITransaction GetTransaction()
		{
			return InversionOfControl.IoC.Resolve<ITransaction>();
		}


		/// <summary>
		/// Gets the BaseRepository.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>Registered BaseRepository for type.</returns>
		public BaseRepository<T> GetRepository<T>() where T : class
		{
			return InversionOfControl.IoC.Resolve<BaseRepository<T>>();
		}

	}
}


