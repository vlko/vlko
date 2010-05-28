using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.ActiveRecord;
using Castle.Windsor;
using GenericRepository;
using Microsoft.Web.Mvc.AspNet4;
using NLog;
using vlko.core;
using vlko.core.Models.Action;
using vlko.core.Models.Action.ActionModel;
using vlko.model.IoC;

namespace vlko.web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
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

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            // register data annotations provider for .net 4
            ModelMetadataProviders.Current = new DataAnnotations4ModelMetadataProvider();
            DataAnnotations4ModelValidatorProvider.RegisterProvider();

            IoC.InitializeWith(new WindsorContainer());
            ApplicationInit.InitializeRepositories();
            ApplicationInit.InitializeServices();
            ApplicationInit.RegisterBinders();

            ActiveRecordStarter.Initialize();
            ActiveRecordStarter.RegisterTypes(ApplicationInit.ListOfModelTypes());
            ActiveRecordStarter.CreateSchema();

            using (var tran = RepositoryFactory.StartTransaction())
            {

                IoC.Resolve<IUserAction>().CreateAdmin("vlko", "vlko@zilina.net", "test");
                var admin = IoC.Resolve<IUserAction>().GetByName("vlko"); 
                IoC.Resolve<IStaticTextCrud>().Create(
                    new StaticTextActionModel
                        {
                            AllowComments = false,
                            Creator = admin,
                            Title = "Home",
                            FriendlyUrl = "Home",
                            ChangeDate = DateTime.Now,
                            PublishDate = DateTime.Now,
                            Text = "Welcome to vlko"
                        });
                for (int i = 0; i < 1000; i++)
                {
                    IoC.Resolve<IStaticTextCrud>().Create(
                    new StaticTextActionModel
                    {
                        AllowComments = false,
                        Creator = admin,
                        Title = "StaticPage" + i,
                        FriendlyUrl = "StaticPage" + i,
                        ChangeDate = DateTime.Now,
                        PublishDate = DateTime.Now,
                        Text = "Static page" + i
                    });
                }
                tran.Commit();
            }
        }

        protected virtual void Application_Error(object sender, EventArgs e)
        {
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
                user, url, Server.GetLastError().Message),
                Server.GetLastError());
        }
    }
}