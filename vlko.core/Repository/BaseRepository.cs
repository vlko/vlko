using System;
using System.ComponentModel.Composition;
using vlko.core.Repository.Exceptions;

namespace vlko.core.Repository
{
	[InheritedExport]
	public interface IRepository<out T> where T : class
	{
		/// <summary>
		/// Gets the command group.
		/// </summary>
		/// <typeparam name="TCommangGroup">The type of the command group.</typeparam>
		/// <returns>Query.</returns>
		/// <exception cref="RepositoryFactoryNotInitializeException">BaseRepository not initialized.</exception>
		/// <exception cref="ActionNotRegisteredException">If type TCommangGroup not registered in BaseRepository.</exception>
		TCommangGroup GetCommand<TCommangGroup>() where TCommangGroup : class, ICommandGroup<T>;

		/// <summary>
		/// Initalizes the command group.
		/// </summary>
		/// <param name="commandGroup">The command group.</param>
		void InitalizeAction(ICommandGroup<T> commandGroup);
	}

	public abstract class BaseRepository<T> : IRepository<T> where T : class
	{
		/// <summary>
		/// Gets the command group.
		/// </summary>
		/// <typeparam name="TCommangGroup">The type of the command group.</typeparam>
		/// <returns>Query.</returns>
		/// <exception cref="RepositoryFactoryNotInitializeException">BaseRepository not initialized.</exception>
		/// <exception cref="ActionNotRegisteredException">If type TCommangGroup not registered in BaseRepository.</exception>
		public TCommandGroup GetCommand<TCommandGroup>() where TCommandGroup : class, ICommandGroup<T>
		{
			TCommandGroup action = null;

			try
			{
				action = RepositoryFactory.FactoryResolver.ResolveCommand<TCommandGroup>();
			}
			catch (Exception innerException)
			{
				throw new ActionNotRegisteredException(typeof(TCommandGroup), this.GetType(), typeof(T), innerException);
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
				throw new ActionNotRegisteredException(typeof(TCommandGroup), this.GetType(), typeof(T), null);
			}

			return action;
		}

		/// <summary>
		/// Initalizes the command group.
		/// </summary>
		/// <param name="commandGroup">The command group.</param>
		public virtual void InitalizeAction(ICommandGroup<T> commandGroup)
		{
			commandGroup.Initialize(new InitializeContext<T>(this));
		}
	}
}
