using System.ComponentModel.Composition;
using vlko.BlogModule.Roots;
using vlko.core.Repository;

namespace vlko.BlogModule.Action
{
	[InheritedExport]
	public interface ISystemMessageCommands : ICommandGroup<SystemMessage>
	{
		/// <summary>
		/// Gets all system messages.
		/// </summary>
		/// <returns>System messages query result.</returns>
		IQueryResult<SystemMessage> GetAll();

		/// <summary>
		/// Creates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Created item.</returns>
		SystemMessage Create(SystemMessage item);
	}
}
