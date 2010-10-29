using System;
using Castle.ActiveRecord;
using vlko.model.Action;
using vlko.model.Repository;

namespace vlko.model.Implementation.NH.Action
{
	public class AppSettingAction : BaseAction<AppSetting>, IAppSettingAction
	{
		/// <summary>
		/// Saves the specified item (create or update).
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Application setting.</returns>
		public AppSetting Save(AppSetting item)
		{
			var oldItem = ActiveRecordMediator<AppSetting>.FindByPrimaryKey(item.Name, false);
			if (oldItem == null)
			{
				ActiveRecordMediator<AppSetting>.Create(item);
			}
			else
			{
				oldItem.Value = item.Value;
				ActiveRecordMediator<AppSetting>.Update(oldItem);
			}
			return item;
		}

		/// <summary>
		/// Gets application setting for the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>Application setting.</returns>
		public AppSetting Get(string name)
		{
			return ActiveRecordMediator<AppSetting>.FindByPrimaryKey(name, false);
		}
	}
}