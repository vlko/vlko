using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using GenericRepository;
using vlko.core.ActiveRecord;
using vlko.core.Authentication;
using vlko.core.Authentication.Implementation;
using vlko.core.Components;
using vlko.core.Models;
using vlko.core.Models.Action;
using vlko.core.Models.Action.Implementation;
using vlko.core.Services;
using vlko.core.Services.Implementation;

namespace vlko.core
{
    public static class ApplicationInit
    {
        /// <summary>
        /// Lists the of model types.
        /// </summary>
        /// <returns>List of model types.</returns>
        public static Type[] ListOfModelTypes()
        {
            return new[]
                       {
                           typeof(Content),
                           typeof(User),
                           typeof(Comment),
                           typeof(CommentVersion),
                           typeof(StaticText),
                           typeof(StaticTextVersion),
                           typeof(RssFeed),
                           typeof(RssItem)
                       };
        }

        /// <summary>
        /// Initializes the repositories.
        /// </summary>
        public static void InitializeRepositories()
        {
            IWindsorContainer container = model.IoC.IoC.Container;
            container.Register(
                Component.For<IUnitOfWork>().ImplementedBy<UnitOfWork>().LifeStyle.Transient,
                Component.For<ITransaction>().ImplementedBy<Transaction>().LifeStyle.Transient,

                Component.For<BaseRepository<StaticText>>().ImplementedBy<Repository<StaticText>>(),
                Component.For<IStaticTextCrud>().ImplementedBy<StaticTextCrud>(),
                Component.For<IStaticTextData>().ImplementedBy<StaticTextData>(),

                Component.For<BaseRepository<Comment>>().ImplementedBy<Repository<Comment>>(),
                Component.For<ICommentCrud>().ImplementedBy<CommentCrud>(),

                Component.For<BaseRepository<User>>().ImplementedBy<Repository<User>>(),
                Component.For<IUserAuthentication>().ImplementedBy<UserAuthentication>(),
                Component.For<IUserAction>().ImplementedBy<UserAction>()
                );
        }

        /// <summary>
        /// Initializes the services.
        /// </summary>
        public static void InitializeServices()
        {
            IWindsorContainer container = model.IoC.IoC.Container;
            container.Register(
                Component.For<IAppInfoService>().ImplementedBy<AppInfoService>(),
                Component.For<IEmailService>().ImplementedBy<EmailService>(),
                Component.For<IFormsAuthenticationService>().ImplementedBy<FormsAuthenticationService>(),
                Component.For<IUserAuthenticationService>().ImplementedBy<UserAuthenticationService>()
                );
        }

        /// <summary>
        /// Registers the binders.
        /// </summary>
        public static void RegisterBinders()
        {
            ModelBinders.Binders.DefaultBinder = new ExtendedModelBinder();
        }
    }
}
