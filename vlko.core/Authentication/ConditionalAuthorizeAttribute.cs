using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using vlko.core.Base;

namespace vlko.core.Authentication
{
	/// <summary>
	/// Conditional authorize attribute using app setting to get bool value if to ignore (true) authorization for this controller or action or to perform authorization (false or missing value).
	/// </summary>
	public class ConditionalAuthorizeAttribute : AuthorizeAttribute
	{
		private static readonly ReaderWriterLockSlim CacheLock = new ReaderWriterLockSlim();
		private static readonly Dictionary<string,bool> Cache = new Dictionary<string, bool>();
		/// <summary>
		/// Gets or sets a value indicating whether [perform authorization].
		/// </summary>
		/// <value><c>true</c> if [perform authorization]; otherwise, <c>false</c>.</value>
		public bool IgnoreAuthorization { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ConditionalAuthorizeAttribute"/> class.
		/// </summary>
		/// <param name="configKeyIgnoreIdent">The config key ignore ident in application settings .</param>
		public ConditionalAuthorizeAttribute(string configKeyIgnoreIdent)
		{
			IgnoreAuthorization = GetConfigValueFromCache(configKeyIgnoreIdent);
		}

		/// <summary>
		/// Called when a process requests authorization.
		/// </summary>
		/// <param name="filterContext">The filter context, which encapsulates information for using <see cref="T:System.Web.Mvc.AuthorizeAttribute"/>.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="filterContext"/> parameter is null.</exception>
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			// if not ignore authorization
			if (!IgnoreAuthorization)
			{
				base.OnAuthorization(filterContext);
			}
		}

		/// <summary>
		/// Gets the config value from cache.
		/// </summary>
		/// <param name="configKeyIgnoreIdent">The config key ident.</param>
		/// <returns>Config value.</returns>
		private static bool GetConfigValueFromCache(string configKeyIgnoreIdent)
		{
			bool? ignoreAuthorization = null;

			using (CacheLock.ReadLock())
			{
				if (Cache.ContainsKey(configKeyIgnoreIdent))
				{
					ignoreAuthorization = Cache[configKeyIgnoreIdent];
				}
			}

			if (ignoreAuthorization == null)
			{
				using (CacheLock.WriteLock())
				{
					if (!Cache.ContainsKey(configKeyIgnoreIdent))
					{
						ignoreAuthorization = LoadConfigValueFromSettings(configKeyIgnoreIdent);
						Cache.Add(configKeyIgnoreIdent, ignoreAuthorization.Value);
					}
					else
					{
						ignoreAuthorization = Cache[configKeyIgnoreIdent];
					}
				}
			}

			return ignoreAuthorization.Value;
		}

		/// <summary>
		/// Loads the config value.
		/// </summary>
		/// <param name="configKeyIgnoreIdent">The config key ident.</param>
		/// <returns>Config value.</returns>
		private static bool LoadConfigValueFromSettings(string configKeyIgnoreIdent)
		{
			string configValue = ConfigurationManager.AppSettings[configKeyIgnoreIdent];
			bool ignoreAuthorization = false;
			if (bool.TryParse(configValue, out ignoreAuthorization))
			{
				return ignoreAuthorization;
			}
			return false;
		}
	}
}
