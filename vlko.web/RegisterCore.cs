using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NLog;
using Raven.Client.Embedded;
using vlko.BlogModule.Search;
using vlko.core;
using vlko.core.Base.Scheduler.Setting;
using vlko.core.Commands;
using vlko.core.InversionOfControl;
using vlko.core.Repository;

[assembly: WebActivator.PreApplicationStartMethod(typeof(vlko.web.RegisterCore), "Register", Order = 1)]

namespace vlko.web
{
	[Export(typeof(IAppInit))]
	[Order(1)]
	public class RegisterCore : IAppInit
	{
		public static void Register()
		{
			if (new SettingValue<bool>("UseRavenDB", false, new ConfigSettingProvider()).Value)
			{
				IoC.AddCatalogAssembly(Assembly.Load("vlko.core.RavenDB"));
			}
			else
			{
				IoC.AddCatalogAssembly(Assembly.Load("vlko.core.NH"));
			}
		}

		public void RegisterRoutes(RouteCollection routes)
		{
			routes.MapRoute(
				"ConfirmRegistration", // Route name
				"Account/ConfirmRegistration/{verifyToken}", // URL with parameters
				new {controller = "Account", action = "ConfirmRegistration"} // Parameter defaults
				);

			routes.MapRoute(
				"ConfirmResetPassword", // Route name
				"Account/ConfirmResetPassword/{verifyToken}", // URL with parameters
				new {controller = "Account", action = "ConfirmResetPassword"} // Parameter defaults
				);
		}

		public void ConfigureDb(bool registerNewDatabase)
		{
			// check from settings if we should use ravendb
			if (new SettingValue<bool>("UseRavenDB", false, new ConfigSettingProvider()).Value)
			{
				ConfigureForRavenDb(registerNewDatabase);
			}
			else
			{
				ConfigureForNHibernate(registerNewDatabase);
			}

			ConfigureSearchProvider(!registerNewDatabase);

			if (registerNewDatabase)
			{
				RepositoryFactory.Command<IUserCommands>().CreateAdmin("vlko", "vlko@zilina.net", "test");
			}
		}

		/// <summary>
		/// Configures for raven db.
		/// </summary>
		/// <param name="registerNewDatabase">if set to <c>true</c> [register new database].</param>
		private void ConfigureForRavenDb(bool registerNewDatabase)
		{
			BlogModule.RavenDB.ApplicationInit.FullInit();
			var documentStore = new EmbeddableDocumentStore()
			{
				Configuration =
				{
					DataDirectory = HttpContext.Current.Server.MapPath(MvcApplication.IndexLocationConst),
				}
			};

			documentStore.Initialize();

			core.RavenDB.DBInit.RegisterDocumentStore(documentStore);
		}

		/// <summary>
		/// Configures for NHibernate.
		/// </summary>
		/// <param name="registerNewDatabase">if set to <c>true</c> [register new database].</param>
		private void ConfigureForNHibernate(bool registerNewDatabase)
		{
			BlogModule.NH.ApplicationInit.FullInit();
			var config = new Configuration();
			config.Configure();
			core.NH.DbInit.InitMappings(config);

			if (registerNewDatabase)
			{
				var schema = new SchemaExport(config);
				schema.Create(false, true);
			}
			SchemaUpdate(config);
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
	}
}