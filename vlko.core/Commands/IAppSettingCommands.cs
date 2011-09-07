using System.ComponentModel.Composition;
using vlko.core.Commands.Model;
using vlko.core.Repository;
using vlko.core.Roots;

namespace vlko.core.Commands
{
	[InheritedExport]
	public interface IAppSettingCommands : ICommandGroup<AppSetting>
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
