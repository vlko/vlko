using System;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using vlko.core.Authentication;
using vlko.core.Authentication.Implementation;
using vlko.core.Components;
using vlko.core.InversionOfControl;
using vlko.core.Services;
using vlko.core.Services.Implementation;
using vlko.model;
using vlko.model.Action;
using vlko.model.Implementation.NH.Action;
using vlko.model.Implementation.NH.Repository;
using vlko.model.Repository;
using vlko.model.Search;

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
						   typeof(AppSetting),
						   typeof(SystemMessage),
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
			IWindsorContainer container = InversionOfControl.IoC.Container;
			container.Register(
				Component.For<IUnitOfWork>().ImplementedBy<UnitOfWork>().LifeStyle.Transient,
				Component.For<ITransaction>().ImplementedBy<Transaction>().LifeStyle.Transient,

				Component.For<BaseRepository<AppSetting>>().ImplementedBy<Repository<AppSetting>>(),
				Component.For<IAppSettingAction>().ImplementedBy<AppSettingAction>(),

				Component.For<BaseRepository<SystemMessage>>().ImplementedBy<Repository<SystemMessage>>(),
				Component.For<ISystemMessageAction>().ImplementedBy<SystemMessageAction>(),

				Component.For<BaseRepository<StaticText>>().ImplementedBy<Repository<StaticText>>(),
				Component.For<IStaticTextCrud>().ImplementedBy<StaticTextCrud>(),
				Component.For<IStaticTextData>().ImplementedBy<StaticTextData>(),

				Component.For<BaseRepository<Comment>>().ImplementedBy<Repository<Comment>>(),
				Component.For<ICommentCrud>().ImplementedBy<CommentCrud>(),
				Component.For<ICommentData>().ImplementedBy<CommentData>(),

				Component.For<BaseRepository<User>>().ImplementedBy<Repository<User>>(),
				Component.For<IUserAuthentication>().ImplementedBy<UserAuthentication>(),
				Component.For<IUserAction>().ImplementedBy<UserAction>(),

				Component.For<IFileBrowserAction>().ImplementedBy<FileBrowserAction>()
					.DynamicParameters((kernel, parameters) =>
					                   	{
					                   		var appInfo = IoC.Resolve<IAppInfoService>();
					                   		parameters["rootUrl"] = appInfo.RootUrl;
											parameters["rootPath"] = appInfo.RootPath;
					                   	})
				);
		}

		/// <summary>
		/// Initializes the services.
		/// </summary>
		public static void InitializeServices()
		{
			IWindsorContainer container = InversionOfControl.IoC.Container;
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
	}
}
