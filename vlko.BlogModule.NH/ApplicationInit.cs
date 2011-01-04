using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using vlko.BlogModule.Action;
using vlko.BlogModule.Base.Scheduler;
using vlko.BlogModule.Implementation.OtherTech.Action;
using vlko.BlogModule.NH.Action;
using vlko.BlogModule.NH.Repository;
using vlko.BlogModule.Roots;
using vlko.BlogModule.Search;
using vlko.core.Action;
using vlko.core.Authentication;
using vlko.core.Authentication.Implementation;
using vlko.core.Base.Scheduler;
using vlko.core.Components;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.core.Roots;
using vlko.core.Services;
using vlko.core.Services.Implementation;

namespace vlko.BlogModule.NH
{
	public static class ApplicationInit
	{

		/// <summary>
		/// Do init part in one step and right order.
		/// </summary>
		public static void FullInit()
		{
			IoC.InitializeWith(new WindsorContainer());
			InitializeRepositories();
			InitializeServices();
			RegisterBinders();
			InitializeScheduler();
		}



		/// <summary>
		/// Initializes the repositories.
		/// </summary>
		public static void InitializeRepositories()
		{
			IWindsorContainer container = IoC.Container;
			container.Register(
				Component.For<IUnitOfWork>().ImplementedBy<UnitOfWork>().LifeStyle.Transient,
				Component.For<ITransaction>().ImplementedBy<Transaction>().LifeStyle.Transient,

				Component.For<IRepository<AppSetting>>().ImplementedBy<Repository<AppSetting>>(),
				Component.For<IAppSettingAction>().ImplementedBy<AppSettingAction>(),

				Component.For<IRepository<SystemMessage>>().ImplementedBy<Repository<SystemMessage>>(),
				Component.For<ISystemMessageAction>().ImplementedBy<SystemMessageAction>(),

				Component.For<IRepository<Content>>().ImplementedBy<Repository<Content>>(),
				Component.For<ITimeline>().ImplementedBy<Timeline>(),

				Component.For<IRepository<StaticText>>().ImplementedBy<Repository<StaticText>>(),
				Component.For<IStaticTextCrud>().ImplementedBy<StaticTextCrud>(),
				Component.For<IStaticTextData>().ImplementedBy<StaticTextData>(),

				Component.For<IRepository<Comment>>().ImplementedBy<Repository<Comment>>(),
				Component.For<ICommentCrud>().ImplementedBy<CommentCrud>(),
				Component.For<ICommentData>().ImplementedBy<CommentData>(),

				Component.For<IRepository<IUser>>().ImplementedBy<Repository<User>>(),
				Component.For<IUserAuthentication>().ImplementedBy<UserAuthentication>(),
				Component.For<IUserAction>().ImplementedBy<UserAction>(),

				Component.For<IFileBrowserAction>().ImplementedBy<FileBrowserAction>()
					.DynamicParameters((kernel, parameters) =>
					                   	{
					                   		var appInfo = IoC.Resolve<IAppInfoService>();
					                   		parameters["rootUrl"] = appInfo.RootUrl;
					                   		parameters["rootPath"] = appInfo.RootPath;
					                   	}),

				Component.For<IRepository<RssFeed>>().ImplementedBy<Repository<RssFeed>>(),
				Component.For<IRssFeedAction>().ImplementedBy<RssFeedAction>(),
				Component.For<IRssFeedConnection>().ImplementedBy<RssFeedConnection>(),

				Component.For<IRepository<RssItem>>().ImplementedBy<Repository<RssItem>>(),
				Component.For<IRssItemAction>().ImplementedBy<RssItemAction>(),

				Component.For<IRepository<TwitterStatus>>().ImplementedBy<Repository<TwitterStatus>>(),
				Component.For<ITwitterConnection>().ImplementedBy<TwitterConnection>(),
				Component.For<ITwitterStatusAction>().ImplementedBy<TwitterStatusAction>() 
				);
		}

		/// <summary>
		/// Initializes the services.
		/// </summary>
		public static void InitializeServices()
		{
			IWindsorContainer container = IoC.Container;
			container.Register(
				Component.For<IAppInfoService>().ImplementedBy<AppInfoService>(),
				Component.For<IEmailService>().ImplementedBy<EmailService>(),
				Component.For<IFormsAuthenticationService>().ImplementedBy<FormsAuthenticationService>(),
				Component.For<IUserAuthenticationService>().ImplementedBy<UserAuthenticationService>(),
				Component.For<BaseRepository<SearchRoot>>().ImplementedBy<Repository<SearchRoot>>(),
				Component.For<ISearchAction>().ImplementedBy<SearchAction>(),
				Component.For<ISearchProvider>().ImplementedBy<SearchProvider>(),
				Component.For<SearchContext>().ImplementedBy<SearchContext>().LifeStyle.Transient,
				Component.For<SearchUpdateContext>().ImplementedBy<SearchUpdateContext>().LifeStyle.Transient
				);
		}

		/// <summary>
		/// Registers the binders.
		/// </summary>
		public static void RegisterBinders()
		{
			ModelBinders.Binders.DefaultBinder = new ExtendedModelBinder();
		}

		/// <summary>
		/// Initializes the scheduler.
		/// </summary>
		public static void InitializeScheduler()
		{
			var scheduler = new Scheduler(new SchedulerTask[]
			                              	{
			                              		new KeepAliveTask(5, true),
												new UpdateTwitterTask(10, true),
												new UpdateRssFeedsTask(10, true),
			                              	}, 20);
			scheduler.Start();
		}
	}
}
