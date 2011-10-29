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
		/// <summary>
		/// Registers the routes.
		/// </summary>
		/// <param name="routes">The routes.</param>
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


			routes.MapRoute(
				"ConfirmRegistration", // Route name
				"Account/ConfirmRegistration/{verifyToken}", // URL with parameters
				new { controller = "Account", action = "ConfirmRegistration" } // Parameter defaults
			);

			routes.MapRoute(
				"ConfirmResetPassword", // Route name
				"Account/ConfirmResetPassword/{verifyToken}", // URL with parameters
				new { controller = "Account", action = "ConfirmResetPassword" } // Parameter defaults
			);

			// page specific routes
			routes.MapRoute(
				"PageComment", // Route name
				"Page/{friendlyUrl}/NewComment/{sort}", // URL with parameters
				new { controller = "Page", action = "NewComment", sort = "tree" /* flat/desc/tree */ } // Parameter defaults
			);
			routes.MapRoute(
				"PageCommentReply", // Route name
				"Page/{friendlyUrl}/Reply/{parentId}/{sort}", // URL with parameters
				new { controller = "Page", action = "Reply", sort = "tree" /* flat/desc/tree */ } // Parameter defaults
			);
			routes.MapRoute(
				"PageView", // Route name
				"Page/{friendlyUrl}/{sort}", // URL with parameters
				new { controller = "Page", action = "ViewPage", sort = "tree" /* flat/desc/tree */ } // Parameter defaults
			);

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);

		}

		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);

			var dataExists = Directory.Exists(HttpContext.Current.Server.MapPath("~/App_Data/Index.Lucene"));

			// check from settings if we should use ravendb
			if (new SettingValue<bool>("UseRavenDB", false, new ConfigSettingProvider()).Value)
			{
				ConfigureForRavenDb(dataExists);
			}
			else
			{
				ConfigureForNHibernate(dataExists);
			}
		}

		/// <summary>
		/// Configures application for Raven DB.
		/// </summary>
		/// <param name="dataExists">if set to <c>true</c> [data exists].</param>
		private static void ConfigureForRavenDb(bool dataExists)
		{
			IoC.AddCatalogAssembly(Assembly.Load("vlko.core.RavenDB"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule.RavenDB"));

			BlogModule.RavenDB.ApplicationInit.FullInit();
			var documentStore = new EmbeddableDocumentStore()
			{
				Configuration =
				{
					DataDirectory = HttpContext.Current.Server.MapPath("~/App_Data/RavenDB"),
				}
			};

			documentStore.Initialize();

			core.RavenDB.DBInit.RegisterDocumentStore(documentStore);

			ConfigureSearchProvider(dataExists);

			if (!dataExists)
			{
				RepositoryFactory.Command<IUserCommands>().CreateAdmin("vlko", "vlko@zilina.net", "test");
				SessionFactory.WaitForStaleIndexes();
				CreateSomeData();
			}
		}

		/// <summary>
		/// Configures application for NHibernate.
		/// </summary>
		/// <param name="dataExists">if set to <c>true</c> [data exists].</param>
		private void ConfigureForNHibernate(bool dataExists)
		{
			IoC.AddCatalogAssembly(Assembly.Load("vlko.core.NH"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule.NH"));

			BlogModule.NH.ApplicationInit.FullInit();
			var config = new Configuration();
			config.Configure();
			core.NH.DbInit.InitMappings(config);

			ConfigureSearchProvider(dataExists);

			if (!dataExists)
			{
				var schema = new SchemaExport(config);
				schema.Create(false, true);

				RepositoryFactory.Command<IUserCommands>().CreateAdmin("vlko", "vlko@zilina.net", "test");
				CreateSomeData();
			}

			SchemaUpdate(config);
		}

		/// <summary>
		/// Configures the search provider.
		/// </summary>
		/// <param name="dataExists">if set to <c>true</c> [data exists].</param>
		private static void ConfigureSearchProvider(bool dataExists)
		{
			// set search folder
			var indexDirectory = HttpContext.Current.Server.MapPath("~/App_Data/Index.Lucene");

			if (!dataExists)
			{
				// delete previous search index
				if (Directory.Exists(indexDirectory))
				{
					Directory.Delete(indexDirectory, true);
				}

				// create if not exists
				Directory.CreateDirectory(indexDirectory);
			}

			IoC.Resolve<ISearchProvider>().Initialize(indexDirectory);
		}

		/// <summary>
		/// Schemas the update.
		/// </summary>
		private void SchemaUpdate(Configuration config)
		{
			var updater = new SchemaUpdate(config);
			updater.Execute(false, true);

			StringBuilder errorLog = new StringBuilder();
			foreach (var error in updater.Exceptions)
			{
				errorLog.AppendLine(error.ToString());
			}
			if (errorLog.Length > 0)
			{
				LogManager.GetLogger("DbChanges").Info("Errors generated during schema update:\n {0}", errorLog.ToString());
			}
		}

		/// <summary>
		/// Creates some data.
		/// </summary>
		private static void CreateSomeData()
		{
			if (new SettingValue<bool>("CreateSampleData", false, new ConfigSettingProvider()).Value)
			{
				using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
				{
					var searchAction = RepositoryFactory.Command<ISearchCommands>();
					var admin = RepositoryFactory.Command<IUserCommands>().GetByName("vlko");
					var home = RepositoryFactory.Command<IStaticTextCrud>().Create(
						new StaticTextCRUDModel
							{
								AllowComments = false,
								Creator = admin,
								Title = "About",
								FriendlyUrl = "about",
								ChangeDate = DateTime.Now,
								PublishDate = DateTime.Now,
								Text = "Some about me text",
								Description = "Some about me text"
							});
					searchAction.IndexStaticText(tran, home);
					if (Settings.CreateSampleData.Value)
					{
						for (int i = 0; i < 30; i++)
						{
							searchAction.IndexComment(tran,
							                          RepositoryFactory.Command<ICommentCrud>().Create(
							                          	new CommentCRUDModel()
							                          		{
							                          			AnonymousName = "User",
							                          			ChangeDate = DateTime.Now.AddDays(-i),
							                          			ClientIp = "127.0.0.1",
							                          			ContentId = home.Id,
							                          			Name = "Comment" + i,
							                          			Text = "Home commment" + i,
							                          			UserAgent = "Mozzilla"
							                          		}));
						}
						for (int i = 0; i < 1000; i++)
						{
							var text = RepositoryFactory.Command<IStaticTextCrud>().Create(
								new StaticTextCRUDModel
									{
										AllowComments = true,
										Creator = admin,
										Title = "StaticPage" + i,
										FriendlyUrl = "StaticPage" + i,
										ChangeDate = DateTime.Now.AddDays(-i),
										PublishDate = DateTime.Now.AddDays(-i),
										Text = "Static page" + i,
										Description = "Static page" + i
									});
							searchAction.IndexStaticText(tran, text);
							searchAction.IndexComment(tran,
							                          RepositoryFactory.Command<ICommentCrud>().Create(
							                          	new CommentCRUDModel()
							                          		{
							                          			AnonymousName = "User",
							                          			ChangeDate = DateTime.Now.AddDays(-i),
							                          			ClientIp = "127.0.0.1",
							                          			ContentId = text.Id,
							                          			Name = "Comment" + i,
							                          			Text = "Static page" + i,
							                          			UserAgent = "Mozzilla"
							                          		}));
						}
					}
					tran.Commit();
				}
			}
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