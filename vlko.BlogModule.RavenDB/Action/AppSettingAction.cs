using vlko.BlogModule.RavenDB.Repository;
using vlko.BlogModule.Roots;
using vlko.core.Action;
using vlko.core.Action.Model;
using vlko.core.Roots;
using vlko.core.Repository;

namespace vlko.BlogModule.RavenDB.Action
{
	public class AppSettingAction : BaseAction<IAppSetting>, IAppSettingAction
	{
		/// <summary>
		/// Saves the specified item (create or update).
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Application setting.</returns>
		public AppSettingModel Save(AppSettingModel item)
		{
			var oldItem = SessionFactory<AppSetting>.Load(item.Name, false);
			if (oldItem == null)
			{
				SessionFactory<AppSetting>.Store(new AppSetting
				                                        	{
																Id = item.Name,
																Value = item.Value
				                                        	});
			}
			else
			{
				oldItem.Value = item.Value;
				SessionFactory<AppSetting>.Store(oldItem);
			}
			return item;
		}

		/// <summary>
		/// Gets application setting for the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>Application setting.</returns>
		public AppSettingModel Get(string name)
		{
			var item = SessionFactory<AppSetting>.Load(name, false);
			if (item != null)
			{
				return new AppSettingModel
				       	{
				       		Name = item.Id,
				       		Value = item.Value
				       	};
			}
			return null;
		}
	}
}