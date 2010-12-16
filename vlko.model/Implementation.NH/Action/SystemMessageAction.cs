using vlko.core.Repository;
using vlko.model.Action;
using vlko.model.Implementation.NH.Repository;
using vlko.model.Roots;

namespace vlko.model.Implementation.NH.Action
{
	public class SystemMessageAction : BaseAction<SystemMessage>, ISystemMessageAction
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