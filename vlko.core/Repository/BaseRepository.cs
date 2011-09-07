using System;
using vlko.core.Repository.Exceptions;

namespace vlko.core.Repository
{
	public abstract class BaseRepository<T> : IRepository<T> where T : class
	{
		/// <summary>
		/// Gets the command group.
		/// </summary>
		/// <typeparam name="TCommandGroup">The type of the command group.</typeparam>
		/// <returns>Query.</returns>
		/// <exception cref="RepositoryFactoryNotInitializeException">BaseRepository not initialized.</exception>
		/// <exception cref="CommandGroupNotRegisteredException">If type TCommangGroup not registered in BaseRepository.</exception>
		public TCommandGroup GetCommand<TCommandGroup>() where TCommandGroup : class, ICommandGroup<T>
		{
			TCommandGroup commandGroup = null;

			try
			{
				commandGroup = RepositoryFactory.FactoryResolver.ResolveCommand<TCommandGroup>();
			}
			catch (Exception innerException)
			{
				throw new CommandGroupNotRegisteredException(typeof(TCommandGroup), this.GetType(), typeof(T), innerException);
			}

			if (commandGroup != null)
			{
				if (!commandGroup.Initialized)
				{
					InitalizeAction(commandGroup);
				}
			}
			else
			{
				throw new CommandGroupNotRegisteredException(typeof(TCommandGroup), this.GetType(), typeof(T), null);
			}

			return commandGroup;
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
