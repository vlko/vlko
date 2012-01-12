using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using vlko.BlogModule.Commands;
using vlko.BlogModule.Commands.CRUDModel;
using vlko.BlogModule.Search;
using vlko.core;
using vlko.core.Base.Scheduler.Setting;
using vlko.core.Commands;
using vlko.core.InversionOfControl;
using vlko.core.RavenDB.Repository;
using vlko.core.Repository;

[assembly: WebActivator.PreApplicationStartMethod(typeof(vlko.web.RegisterBlog), "Register", Order = 1)]

namespace vlko.web
{
	[Export(typeof(IAppInit))]
	[Order(2)]
	public class RegisterBlog : IAppInit
	{
		/// <summary>
		/// Registers this instance.
		/// </summary>
		public static void Register()
		{
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule"));
			if (new SettingValue<bool>("UseRavenDB", false, new ConfigSettingProvider()).Value)
			{
				IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule.RavenDB"));
			}
			else
			{
				IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule.NH"));
			}
		}

		/// <summary>
		/// Registers the routes.
		/// </summary>
		/// <param name="routes">The routes.</param>
		public void RegisterRoutes(RouteCollection routes)
		{
			// page specific routes
			routes.MapRoute(
				"PageComment", // Route name
				"Page/{friendlyUrl}/NewComment/{sort}", // URL with parameters
				new {controller = "Page", action = "NewComment", sort = "tree" /* flat/desc/tree */} // Parameter defaults
				);
			routes.MapRoute(
				"PageCommentReply", // Route name
				"Page/{friendlyUrl}/Reply/{parentId}/{sort}", // URL with parameters
				new {controller = "Page", action = "Reply", sort = "tree" /* flat/desc/tree */} // Parameter defaults
				);
			routes.MapRoute(
				"PageView", // Route name
				"Page/{friendlyUrl}/{sort}", // URL with parameters
				new {controller = "Page", action = "ViewPage", sort = "tree" /* flat/desc/tree */} // Parameter defaults
				);
		}

		/// <summary>
		/// Configures the db.
		/// </summary>
		/// <param name="registerNewDatabase">if set to <c>true</c> [register new database].</param>
		public void ConfigureDb(bool registerNewDatabase)
		{
			if (registerNewDatabase)
			{
				if (new SettingValue<bool>("UseRavenDB", false, new ConfigSettingProvider()).Value)
				{
					SessionFactory.WaitForStaleIndexes();
				}
				CreateData();
			}
		}

		/// <summary>
		/// Creates data.
		/// </summary>
		private static void CreateData()
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
					if (vlko.BlogModule.Settings.CreateSampleData.Value)
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
	}
}