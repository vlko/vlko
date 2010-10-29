using vlko.model;
using vlko.model.Action;
using vlko.model.Repository;

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
				var dbValue = RepositoryFactory.Action<IAppSettingAction>().Get(name);

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
				RepositoryFactory.Action<IAppSettingAction>().Save(
					new AppSetting
						{
							Name = name,
							Value = value
						});
			}
		}
	}
}