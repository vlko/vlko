using System;
using vlko.core.Repository.Exceptions;

namespace vlko.core.Repository
{
	public interface IRepository<out T> where T : class
	{
		/// <summary>
		/// Gets the action.
		/// </summary>
		/// <typeparam name="TAType">The type of the action.</typeparam>
		/// <returns>Query.</returns>
		/// <exception cref="RepositoryFactoryNotInitializeException">BaseRepository not initialized.</exception>
		/// <exception cref="ActionNotRegisteredException">If type TAType not registered in BaseRepository.</exception>
		TAType GetAction<TAType>() where TAType : class, IAction<T>;

		/// <summary>
		/// Initalizes the queryAction.
		/// </summary>
		/// <param name="action">The action.</param>
		void InitalizeAction(IAction<T> action);
	}

	public abstract class BaseRepository<T> : IRepository<T> where T : class
	{
		/// <summary>
		/// Gets the action.
		/// </summary>
		/// <typeparam name="TAType">The type of the action.</typeparam>
		/// <returns>Query.</returns>
		/// <exception cref="RepositoryFactoryNotInitializeException">BaseRepository not initialized.</exception>
		/// <exception cref="ActionNotRegisteredException">If type TAType not registered in BaseRepository.</exception>
		public TAType GetAction<TAType>() where TAType : class, IAction<T>
		{
			TAType action = null;

			try
			{
				action = RepositoryFactory.FactoryResolver.ResolveAction<TAType>();
			}
			catch (Exception innerException)
			{
				throw new ActionNotRegisteredException(typeof(TAType), this.GetType(), typeof(T), innerException);
			}

			if (action != null)
			{
				if (!action.Initialized)
				{
					InitalizeAction(action);
				}
			}
			else
			{
				throw new ActionNotRegisteredException(typeof(TAType), this.GetType(), typeof(T), null);
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
