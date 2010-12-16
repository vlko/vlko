using vlko.core.Action;
using vlko.core.Repository;
using vlko.core.Roots;
using vlko.core.Action.Model;

namespace vlko.core.Base.Scheduler.Setting
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