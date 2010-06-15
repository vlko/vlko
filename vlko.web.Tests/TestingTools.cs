using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Rhino.Mocks;

namespace vlko.web.Tests
{
	public static class TestingTools
	{
		/// <summary>
		/// Mocks the request ajax.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="valueProvider">The value provider.</param>
		public static void MockRequestAjax(this ControllerBase controller, NameValueCollection formValues = null, NameValueCollection queryStringValues = null)
		{
			var controllerContext = MockRepository.GenerateMock<ControllerContext>();
			var request = MockRepository.GenerateMock<HttpRequestBase>();
			controllerContext.Expect(c => c.HttpContext.Request).Return(request);
			request.Expect(c => c["X-Requested-With"]).Return("XMLHttpRequest");
			controller.ControllerContext = controllerContext;
			if (formValues != null)
			{
				request.Expect(c => c.Form).Return(formValues);
			}
			if (queryStringValues != null)
			{
				request.Expect(c => c.QueryString).Return(queryStringValues);
			}
		}

		/// <summary>
		/// Mocks the request.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="formValues">The form values.</param>
		/// <param name="queryStringValues">The query string values.</param>
		public static void MockRequest(this ControllerBase controller, NameValueCollection formValues = null, NameValueCollection queryStringValues = null)
		{
			var controllerContext = MockRepository.GenerateMock<ControllerContext>();
			var request = MockRepository.GenerateMock<HttpRequestBase>();
			controllerContext.Expect(c => c.HttpContext.Request).Return(request);
			request.Expect(c => c["X-Requested-With"]).Return("");
			controller.ControllerContext = controllerContext;
			if (formValues != null)
			{
				request.Expect(c => c.Form).Return(formValues);
			}
			if (queryStringValues != null)
			{
				request.Expect(c => c.QueryString).Return(queryStringValues);
			}
		}

		/// <summary>
		/// Mocks the value provider.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerName">Name of the controller.</param>
		public static void MockValueProvider(this ControllerBase controller, string controllerName)
		{
			var nameCollection = new NameValueCollection();
			nameCollection.Add("controller", controllerName);
			controller.ValueProvider = new NameValueCollectionValueProvider(nameCollection, CultureInfo.InvariantCulture); 
		}

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

	}
}
