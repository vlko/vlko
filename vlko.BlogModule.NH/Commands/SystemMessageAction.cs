using vlko.BlogModule.Commands;
using vlko.BlogModule.Roots;
using vlko.core.NH.Repository;
using vlko.core.Repository;

namespace vlko.BlogModule.NH.Commands
{
	public class SystemMessageCommands : CommandGroup<SystemMessage>, ISystemMessageCommands
	{
		/// <summary>
		/// Gets all system messages.
		/// </summary>
		/// <returns>System messages query result.</returns>
		public IQueryResult<SystemMessage> GetAll()
		{
			return new QueryLinqResult<SystemMessage>(
				SessionFactory<SystemMessage>.Queryable);
		}

		/// <summary>
		/// Creates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Created item.</returns>
		public SystemMessage Create(SystemMessage item)
		{
			SessionFactory<SystemMessage>.Create(item);

			return item;
		}
	}
}