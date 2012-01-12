using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using vlko.core.InversionOfControl;

namespace vlko.core
{
	public static class AppInit
	{
		/// <summary>
		/// Registers the routes.
		/// </summary>
		/// <param name="routes">The routes.</param>
		public static void RegisterRoutes(RouteCollection routes)
		{
			foreach (var appInit in IoC.ResolveAllInstancesOrdered<IAppInit>())
			{
				appInit.RegisterRoutes(routes);
			}
		}

		/// <summary>
		/// Configures the db.
		/// </summary>
		/// <param name="registerNewDatabase">if set to <c>true</c> [register new database].</param>
		public static void ConfigureDb(bool registerNewDatabase)
		{
			foreach (var appInit in IoC.ResolveAllInstancesOrdered<IAppInit>())
			{
				appInit.ConfigureDb(registerNewDatabase);
			}
		}
	}
}
