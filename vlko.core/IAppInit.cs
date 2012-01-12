using System.ComponentModel.Composition;
using System.Web.Routing;

namespace vlko.core
{
	
	public interface IAppInit
	{
		/// <summary>
		/// Registers the routes.
		/// </summary>
		/// <param name="routes">The routes.</param>
		void RegisterRoutes(RouteCollection routes);
		/// <summary>
		/// Configures the db.
		/// </summary>
		/// <param name="registerNewDatabase">if set to <c>true</c> [register new database].</param>
		void ConfigureDb(bool registerNewDatabase);
	}
}