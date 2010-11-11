using vlko.core.Base.Scheduler.Setting;

namespace vlko.core
{
	public class Settings
	{
		/// <summary>
		/// Admin role name constant.
		/// </summary>
		public const string AdminRole = "admin";

		/// <summary>
		/// Minimum length of password.
		/// </summary>
		public static readonly int MinRequiredPasswordLength = 8;

		/// <summary>
		/// Base url.
		/// </summary>
		public static readonly SettingValue<string> BaseUrl 
			= new SettingValue<string>("BaseUrl", null, new ConfigSettingProvider());

	}
}
