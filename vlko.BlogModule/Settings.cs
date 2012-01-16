using vlko.core.Base.Scheduler.Setting;

namespace vlko.BlogModule
{
	public class Settings : vlko.core.Settings
	{
		/// <summary>
		/// Base url.
		/// </summary>
		public static readonly SettingValue<string> BaseUrl 
			= new SettingValue<string>("BaseUrl", null, new ConfigSettingProvider());

		/// <summary>
		/// Indicate if to create sample data to new db.
		/// </summary>
		public static readonly SettingValue<bool> CreateSampleData
			= new SettingValue<bool>("CreateSampleData", false, new ConfigSettingProvider());

		public static class Twitter
		{
			/// <summary>
			/// Consumer key.
			/// </summary>
			public static readonly SettingValue<string> ConsumerKey 
				= new SettingValue<string>("ConsumerKey", null, new ConfigSettingProvider());

			/// <summary>
			/// Consumer secret.
			/// </summary>
			public static readonly SettingValue<string> ConsumerSecret
				= new SettingValue<string>("ConsumerSecret", null, new ConfigSettingProvider());

			/// <summary>
			/// Main twitter account.
			/// </summary>
			public static readonly SettingValue<string> TwitterAccount
				= new SettingValue<string>("TwitterAccount", null, new ConfigSettingProvider());

			/// <summary>
			/// Twitter authorization token.
			/// </summary>
			public static readonly SettingValue<string> Token
				= new SettingValue<string>("TwitterToken", null, new DatabaseSettingProvider());

			/// <summary>
			/// Twitter authorization token - secret part.
			/// </summary>
			public static readonly SettingValue<string> TokenSecret
				= new SettingValue<string>("TwitterTokenSecret", null, new DatabaseSettingProvider());
		}
	}
}
