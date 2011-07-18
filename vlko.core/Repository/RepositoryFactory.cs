using System;
using vlko.core.InversionOfControl;
using vlko.core.Repository.Exceptions;

namespace vlko.core.Repository
{
	/// <summary>
	/// Static IoC resolver class.
	/// </summary>
	public static class RepositoryFactory
	{

		private static IRepositoryFactoryResolver _factoryResolver;

		/// <summary>
		/// Gets the IoC resolver.
		/// </summary>
		/// <value>The IoC resolver.</value>
		public static IRepositoryFactoryResolver FactoryResolver
		{
			get
			{
				lock (typeof(RepositoryFactory))
				{
					if (_factoryResolver == null)
					{
						// Initialize repository IoC resolver
						_factoryResolver = IoC.Resolve<IRepositoryFactoryResolver>();
					}
					return _factoryResolver;
				}
			}

		}

		/// <summary>
		/// Intitialize this static instance the with specified factoryResolver.
		/// </summary>
		/// <param name="factoryResolver">The IoC resolver.</param>
		public static void IntitializeWith(IRepositoryFactoryResolver factoryResolver)
		{
			_factoryResolver = factoryResolver;
		}


		/// <summary>
		/// Starts the unit of work.
		/// </summary>
		/// <returns>New unit of work.</returns>
		public static IUnitOfWork StartUnitOfWork(IUnitOfWorkContext innerContext = null)
		{
			var unitOfWork = FactoryResolver.GetUnitOfWork();
			if (innerContext != null)
			{
				unitOfWork.InitUnitOfWorkContext(innerContext);
			}
			return unitOfWork;
		}

		/// <summary>
		/// Starts the transaction.
		/// </summary>
		/// <returns>New transaction.</returns>
		public static ITransaction StartTransaction(ITransactionContext innerContext = null)
		{
			var transaction = FactoryResolver.GetTransaction();
			if (innerContext != null)
			{
				transaction.InitTransactionContext(innerContext);
			}
			return transaction;
		}

		/// <summary>
		/// Gets the BaseRepository.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>Registered BaseRepository.</returns>
		public static IRepository<T> GetRepository<T>() where T : class
		{
			try
			{
				var repository = FactoryResolver.GetRepository<T>();
				if (repository == null)
				{
					throw new RepositoryNotRegisteredException(typeof (T), null);
				}
				return repository;
			}
			catch (Exception innerException)
			{
				throw new RepositoryNotRegisteredException(typeof (T), innerException);
			}
		}

		/// <summary>
		/// Gets the action.
		/// </summary>
		/// <typeparam name="TRepository">The type of the repository.</typeparam>
		/// <typeparam name="TAction">The type of the action.</typeparam>
		/// <returns>Action.</returns>
		[Obsolete("Should use Action to get action without repository type specify.")]
		public static TAction GetAction<TRepository, TAction>()
			where TRepository : class
			where TAction : class, IAction<TRepository>
		{
			return GetRepository<TRepository>().GetAction<TAction>();
		}

		/// <summary>
		/// Gets the action (initialize on action is called).
		/// </summary>
		/// <typeparam name="T">Type of action</typeparam>
		/// <returns></returns>
		public static T Action<T>() where T : class, IAction
		{
			var action = FactoryResolver.ResolveAction<T>();
			// initialize if not yet done
			if (!action.Initialized)
			{
				action.Initialize();
			}
			return action;
		}
	}
}
