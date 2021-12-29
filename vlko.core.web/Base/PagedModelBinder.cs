using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using vlko.core.Components;
using vlko.core.Tools;

namespace vlko.core.web.Base
{
    public class PagedModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (HasGenericTypeBase(context.Metadata.ModelType, typeof(PagedModel<>)))
            {
                return new PagedModelBinder();
            }

            return null;
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
            while (type != typeof(object) && type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType) return true;
                type = type.BaseType;
            }

            return false;
        }
    }
    public class PagedModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var pageModel = CreatePagedModel(bindingContext);
            bindingContext.Result = ModelBindingResult.Success(pageModel);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates the paged model.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="bindingContext">The binding context.</param>
        /// <returns>The bound object.</returns>
        public object CreatePagedModel(ModelBindingContext bindingContext)
        {
            object pagedModel = InstanceCreator.Create(bindingContext.ModelType);

            string page = bindingContext.HttpContext.Request.Query["page"];
            string perPage = bindingContext.HttpContext.Request.Query["perpage"];
            if (bindingContext.HttpContext.Request.Method != "GET" && bindingContext.HttpContext.Request.HasFormContentType)
            {
                page = bindingContext.HttpContext.Request.Form["page"];
                perPage = bindingContext.HttpContext.Request.Form["perpage"];
            }
            if (!string.IsNullOrEmpty(page) || !string.IsNullOrEmpty(perPage))
            {
                int currentPage;
                if (int.TryParse(page, out currentPage))
                {
                    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(pagedModel);
                    properties["CurrentPage"].SetValue(pagedModel, currentPage);
                }
                int perPageSize;
                if (int.TryParse(perPage, out perPageSize))
                {
                    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(pagedModel);
                    properties["PageSize"].SetValue(pagedModel, perPageSize);
                }
            }

            return pagedModel;
        }


    }
}
