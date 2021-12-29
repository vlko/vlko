using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace vlko.core.web.Base
{
    public abstract class BaseViewPage<TModel> : RazorPage<TModel>
    {
        public BaseViewPage()
        {

        }

        [RazorInject]
        public IUrlHelper UrlHelper
        {
            set
            {
                Routes = new DynamicRoutes(value);
            }
        }

        /// <summary>
        /// Dynamic Routes helper.
        /// </summary>
        public dynamic Routes { get; private set; }

        /// <summary>
        /// Gets the current action.
        /// </summary>
        /// <value>
        /// The current action (lower case!).
        /// </value>
	    public string CurrentAction
        {
            get
            {
                return ViewContext.RouteData.Values["action"]?.ToString().ToLower();
            }
        }

        /// <summary>
        /// Gets the current controller (lower case!).
        /// </summary>
        /// <value>
        /// The current controller.
        /// </value>
        public string CurrentController
        {
            get
            {
                return ViewContext.RouteData.Values["controller"]?.ToString().ToLower();
            }
        }

        /// <summary>
        /// Gets the current area (lower case!).
        /// </summary>
        /// <value>
        /// The current area.
        /// </value>
        public string CurrentArea
        {
            get
            {
                var area = ViewContext.RouteData.DataTokens["area"] as string;
                if (area != null)
                {
                    area = area.ToLower();
                }
                return area;
            }
        }

        /// <summary>
        /// Gets the query string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Query value as string.</returns>
        public string GetQueryString(string key)
        {
            return Context.Request.Query[key];
        }

        /// <summary>
        /// Gets the form string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Form value as string.</returns>
        public string GetFormString(string key)
        {
            return Context.Request.HasFormContentType ? ViewContext.HttpContext.Request.Form[key].ToString() : null;
        }
    }
}
