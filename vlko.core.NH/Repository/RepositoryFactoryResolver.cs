using vlko.core.InversionOfControl;
using vlko.core.Repository;

namespace vlko.core.NH.Repository   
{
	public class RepositoryFactoryResolver : IRepositoryFactoryResolver
	{

		/// <summary>
		/// Resolves the command group.
		/// </summary>
		/// <typeparam name="T">Generic type.</typeparam>
		/// <returns>Resolved command group.</returns>
		public T ResolveCommand<T>() where T : class, ICommandGroup
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


