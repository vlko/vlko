using System;
using vlko.BlogModule.Action;
using vlko.BlogModule.Roots;
using vlko.BlogModule.RavenDB.Repository;
using vlko.core.Repository;

namespace vlko.BlogModule.RavenDB.Action
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
			if (item.Id == Guid.Empty)
			{
				item.Id = Guid.NewGuid();
			}
			SessionFactory<SystemMessage>.Store(item);

			return item;
		}
	}
}