using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vlko.model.Repository;

namespace vlko.model.Action
{
	public interface IAppSettingAction : IAction<AppSetting>
	{
		/// <summary>
		/// Saves the specified item (create or update).
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Application setting.</returns>
		AppSetting Save(AppSetting item);

		/// <summary>
		/// Gets application setting for the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>Application setting.</returns>
		AppSetting Get(string name);
	}
}
