using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NLog;
using Raven.Client.Embedded;
using vlko.BlogModule.Commands;
using vlko.BlogModule.Commands.CRUDModel;
using vlko.core;
using vlko.core.Base.Scheduler.Setting;
using vlko.core.Commands;
using vlko.core.InversionOfControl;
using vlko.core.RavenDB.Repository;
using vlko.core.Repository;
using vlko.BlogModule;
using vlko.BlogModule.Search;
using Settings = vlko.BlogModule.Settings;

namespace vlko.web
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public const string IndexLocationConst = "~/App_Data/Index.Lucene";


		/// <summary>
		/// Registers the routes.
		/// </summary>
		/// <param name="routes">The routes.</param>
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			AppInit.RegisterRoutes(routes);

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new {controller = "Home", action = "Index", id = UrlParameter.Optional} // Parameter defaults
				);

		}

		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		protected void Application_Start()
		{
			IoC.AddCatalogAssembly(Assembly.GetExecutingAssembly());

			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);

			var dataExists = Directory.Exists(HttpContext.Current.Server.MapPath(IndexLocationConst));
			AppInit.ConfigureDb(!dataExists);
		}

		/// <summary>
		/// Handles the Error event of the Application control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Application_Error(object sender, EventArgs e)
		{
			var exception = Server.GetLastError();
			// if 404 stop logging
			if (exception is HttpException 
				&& ((HttpException)exception).GetHttpCode() == 404)
			{
				return;
			}
			string user = "unknown";
			string url = "unknown";
			if ((User != null) && (User.Identity != null))
			{
				user = User.Identity.Name;
			}
			try
			{
				url = Request.Url.ToString();
			}
			catch
			{

			}
			LogManager.GetLogger("Error").ErrorException(string.Format("For user {0} on url {1} exception {2}",
				user, url, exception.Message),
				exception);
		}
	}
}