using System;
using vlko.model.Repository.Exceptions;

namespace vlko.model.Repository
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
				if (_factoryResolver == null)
				{
					Exception ex = new RepositoryFactoryNotInitializeException();
					throw ex;
				}
				return _factoryResolver;
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
				return _factoryResolver != null;
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
		public static BaseRepository<T> GetRepository<T>() where T : class
		{
			return FactoryResolver.GetRepository<T>();
		}

		/// <summary>
		/// Gets the action.
		/// </summary>
		/// <typeparam name="TA">The type of the action.</typeparam>
		/// <typeparam name="T">Repository type.</typeparam>
		/// <returns>Action.</returns>
		public static TA GetAction<T, TA>()
			where T : class 
			where TA : class, IAction<T>
		{
			return GetRepository<T>().GetAction<TA>();
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
