using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using vlko.core.Tools;

namespace vlko.core.Components
{
	public class ExtendedModelBinder : DefaultModelBinder {

		/// <summary>
		/// Binds the model by using the specified controller context and binding context.
		/// </summary>
		/// <param name="controllerContext">The context within which the controller operates. The context information includes the controller, HTTP content, request context, and route data.</param>
		/// <param name="bindingContext">The context within which the model is bound. The context includes information such as the model object, model name, model type, property filter, and value provider.</param>
		/// <returns>The bound object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="bindingContext "/>parameter is null.</exception>
		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

			 if (HasGenericTypeBase(bindingContext.ModelType, typeof(PagedModel<>)))
			 {
				 return BindPagedModel(controllerContext, bindingContext);
			 }
			 return base.BindModel(controllerContext, bindingContext);
		}

		/// <summary>
		/// Determines whether [has generic type base] [the specified type].
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="genericType">Type of the generic.</param>
		/// <returns>
		/// 	<c>true</c> if [has generic type base] [the specified type]; otherwise, <c>false</c>.
		/// </returns>
		private static bool HasGenericTypeBase(Type type, Type genericType)
		{
			while (type != typeof(object))
			{
				if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType) return true;
				type = type.BaseType;
			}

			return false;
		}


		/// <summary>
		/// Binds the paged model.
		/// </summary>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="bindingContext">The binding context.</param>
		/// <returns>The bound object.</returns>
		public object BindPagedModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			object pagedModel = InstanceCreator.Create(bindingContext.ModelType);

			string page = controllerContext.RequestContext.HttpContext.Request.QueryString.Get("page");
			if (controllerContext.RequestContext.HttpContext.Request.RequestType != "GET")
			{
				page = controllerContext.RequestContext.HttpContext.Request.Form.Get("page");
			}
			if (!string.IsNullOrEmpty(page))
			{
				int currentPage = 0;
				if (Int32.TryParse(page, out currentPage))
				{
					PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(pagedModel);
					properties["CurrentPage"].SetValue(pagedModel, currentPage);
				}
			}

			return pagedModel;
		}
	}
}
