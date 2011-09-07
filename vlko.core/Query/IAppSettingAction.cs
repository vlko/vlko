using System.ComponentModel.Composition;
using vlko.core.Repository;
using vlko.core.Roots;
using vlko.core.Action.Model;

namespace vlko.core.Action
{
	[InheritedExport]
	public interface IAppSettingAction : IAction<AppSetting>
	{
		/// <summary>
		/// Saves the specified item (create or update).
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Application setting.</returns>
		AppSettingModel Save(AppSettingModel item);

		/// <summary>
		/// Gets application setting for the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>Application setting.</returns>
		AppSettingModel Get(string name);
	}
}
