using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.ActiveRecord;
using Castle.Windsor;
using vlko.core;
using vlko.core.Models.Action;
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

            IoC.InitializeWith(new WindsorContainer());
            ApplicationInit.InitializeRepositories();
            ApplicationInit.InitializeServices();

            ActiveRecordStarter.Initialize();
            ActiveRecordStarter.RegisterTypes(ApplicationInit.ListOfModelTypes());
            ActiveRecordStarter.CreateSchema();

            IoC.Resolve<ICreateAdminAction>().CreateAdmin("vlko", "vlko@zilina.net", "test");
        }
    }
}