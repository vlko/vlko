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
		/// <exception cref="CommandGroupNotRegisteredException">If type TCommangGroup not registered in BaseRepository.</exception>
		TCommangGroup GetCommand<TCommangGroup>() where TCommangGroup : class, ICommandGroup<T>;

		/// <summary>
		/// Initalizes the command group.
		/// </summary>
		/// <param name="commandGroup">The command group.</param>
		void InitalizeAction(ICommandGroup<T> commandGroup);
	}
}