using System;
using System.Configuration;
using System.Linq;

namespace vlko.core.Base.Scheduler.Setting
{
	public class ConfigSettingProvider : ISettingProvider
	{
		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>Setting Value for specified name.</returns>
		public bool GetValue(string name, ref string value)
		{
			if (ConfigurationManager.AppSettings.AllKeys.Contains(name))
			{
				value = ConfigurationManager.AppSettings[name];
				return true;
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
			// we are not allowed to save values to config file
			throw new NotImplementedException();
		}
	}
}