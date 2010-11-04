using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.ActiveRecord;
using Castle.Windsor;
using Microsoft.Web.Mvc.AspNet4;
using NLog;
using vlko.core;
using vlko.core.InversionOfControl;
using vlko.model.Action;
using vlko.model.Action.CRUDModel;
using vlko.model.Repository;
using vlko.model.Search;

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

		/// <summary>
		/// Application_s the start.
		/// </summary>
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RegisterRoutes(RouteTable.Routes);

			// register data annotations provider for .net 4
			ModelMetadataProviders.Current = new DataAnnotations4ModelMetadataProvider();
			DataAnnotations4ModelValidatorProvider.RegisterProvider();

			ApplicationInit.FullInit();

			ActiveRecordStarter.Initialize();
			ActiveRecordStarter.RegisterTypes(ApplicationInit.ListOfModelTypes());
			

			var dataExists = File.Exists(HttpContext.Current.Server.MapPath("~/App_Data/ActiveRecord.dat"));

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

			if (!dataExists)
			{
				CreateSomeData();
			}
		}

		/// <summary>
		/// Creates some data.
		/// </summary>
		private void CreateSomeData()
		{
			ActiveRecordStarter.CreateSchema();
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				var searchAction = RepositoryFactory.Action<ISearchAction>();


				RepositoryFactory.Action<IUserAction>().CreateAdmin("vlko", "vlko@zilina.net", "test");
				var admin = RepositoryFactory.Action<IUserAction>().GetByName("vlko");
				var home = RepositoryFactory.Action<IStaticTextCrud>().Create(
					new StaticTextCRUDModel
						{
							AllowComments = false,
							Creator = admin,
							Title = "Home",
							FriendlyUrl = "Home",
							ChangeDate = DateTime.Now,
							PublishDate = DateTime.Now,
							Text = "Welcome to vlko",
							Description = "Welcome to vlko"
						});
				searchAction.IndexStaticText(tran, home);
				for (int i = 0; i < 30; i++)
				{
					searchAction.IndexComment(tran,
					                          RepositoryFactory.Action<ICommentCrud>().Create(
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
					var text = RepositoryFactory.Action<IStaticTextCrud>().Create(
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
					                          RepositoryFactory.Action<ICommentCrud>().Create(
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
				tran.Commit();
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
			if (exception is HttpException)
			{
				
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