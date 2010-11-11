using System.Collections.Generic;
using System.Linq;
using System.Text;
using vlko.core.Repository;
using vlko.model.Roots;

namespace vlko.model.Action
{
	public interface ISystemMessageAction : IAction<SystemMessage>
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
