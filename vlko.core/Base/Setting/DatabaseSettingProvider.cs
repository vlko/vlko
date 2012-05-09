using vlko.core.Commands;
using vlko.core.Commands.Model;
using vlko.core.Repository;

namespace vlko.core.Base.Setting
{
	public class DatabaseSettingProvider : ISettingProvider
	{
		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <returns>True if there is value, otherwise false;</returns>
		public bool GetValue(string name, ref string value)
		{
			using (var session = RepositoryFactory.StartUnitOfWork())
			{
				var dbValue = RepositoryFactory.Command<IAppSettingCommands>().Get(name);

				if (dbValue != null)
				{
					value = dbValue.Value;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Saves the value for specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public void SaveValue(string name, string value)
		{
			using (var tran = RepositoryFactory.StartTransaction())
			{
				RepositoryFactory.Command<IAppSettingCommands>().Save(
					new AppSettingModel
						{
							Name = name,
							Value = value
						});
				tran.Commit();
			}
		}
	}
}