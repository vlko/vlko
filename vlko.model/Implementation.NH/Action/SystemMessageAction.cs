using System;
using System.Linq;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using vlko.model.Repository;

namespace vlko.model.Action
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
				ActiveRecordLinqBase<SystemMessage>.Queryable);
		}

		/// <summary>
		/// Creates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Created item.</returns>
		public SystemMessage Create(SystemMessage item)
		{
			ActiveRecordMediator<SystemMessage>.Create(item);

			return item;
		}
	}
}