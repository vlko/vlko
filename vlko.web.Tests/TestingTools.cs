using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using vlko.core.Authentication;
using vlko.core.Base;
using vlko.core.InversionOfControl;
using vlko.web.Tests.Controllers.Admin;

namespace vlko.web.Tests
{
	public static class TestingTools
	{
		/// <summary>
		/// Binds the model.
		/// </summary>
		/// <typeparam name="TModel">The type of the model.</typeparam>
		/// <param name="controller">The controller.</param>
		/// <param name="valueProvider">The value provider.</param>
		/// <returns>Model instance</returns>
		public static TModel BindModel<TModel>(this Controller controller, IValueProvider valueProvider) where TModel : class
		{
			IModelBinder binder = ModelBinders.Binders.GetBinder(typeof(TModel));
			ModelBindingContext bindingContext = new ModelBindingContext()
			{
				FallbackToEmptyPrefix = true,
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(TModel)),
				ModelName = "NotUsedButNotNull",
				ModelState = controller.ModelState,
				PropertyFilter = (name => { return true; }),
				ValueProvider = valueProvider
			};

			return (TModel)binder.BindModel(controller.ControllerContext, bindingContext);
		}

		/// <summary>
		/// Mocks the user.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="userName">Name of the user.</param>
		public static void MockUser(this BaseController controller, string userName)
		{
			IWindsorContainer container = IoC.Container;
			container.Register(
				Component.For<IUserAuthenticationService>().ImplementedBy<StaticPageControllerTest.UserAuthenticationServiceMock>()
				);
			controller.UserInfo = new UserInfo(userName);
		}

	}
}
