using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace vlko.core.Base
{
	public class BaseViewPage : WebViewPage
	{

		/// <summary>
		/// Dynamic Routes helper.
		/// </summary>
		public dynamic Routes { get; private set; }

		/// <summary>
		/// Initializes the <see cref="T:System.Web.Mvc.AjaxHelper"/>, <see cref="T:System.Web.Mvc.HtmlHelper"/>, and <see cref="T:System.Web.Mvc.UrlHelper"/> classes.
		/// </summary>
		public override void InitHelpers()
		{
			base.InitHelpers();
			Routes = new DynamicRoutes(Url);
		}

		/// <summary>
		/// Executes this instance.
		/// </summary>
		public override void Execute()
		{
			base.ExecutePageHierarchy();
		}
	}

	public abstract class BaseViewPage<TModel> : WebViewPage<TModel>
	{
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
                return ViewContext.RequestContext.RouteData.GetRequiredString("action").ToLower();
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
                return ViewContext.RequestContext.RouteData.GetRequiredString("controller").ToLower();
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
                var area = ViewContext.RequestContext.RouteData.Values["area"] as string;
                if (area != null)
                {
                    area = area.ToLower();
                }
                return area;
            }
        }

        /// <summary>
        /// Determines whether [is current route] [the specified route name].
        /// </summary>
        /// <param name="routeName">Name of the route.</param>
        /// <returns>
        ///   <c>true</c> if [is current route] [the specified route name]; otherwise, <c>false</c>.
        /// </returns>
	    public bool IsCurrentRoute(string routeName)
	    {
	        return ViewContext.RouteData.Route == System.Web.Routing.RouteTable.Routes[routeName];
	    }

        /// <summary>
        /// Gets the query string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Query value as string.</returns>
        public string GetQueryString(string key)
        {
            return ViewContext.RequestContext.HttpContext.Request.QueryString[key];
        }

        /// <summary>
        /// Gets the form string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Form value as string.</returns>
        public string GetFormString(string key)
        {
            return ViewContext.RequestContext.HttpContext.Request.Form[key];
        }

	    /// <summary>
		/// Initializes the <see cref="T:System.Web.Mvc.AjaxHelper"/>, <see cref="T:System.Web.Mvc.HtmlHelper"/>, and <see cref="T:System.Web.Mvc.UrlHelper"/> classes.
		/// </summary>
		public override void InitHelpers()
		{
			base.InitHelpers();
			Routes = new DynamicRoutes(Url);
		}

		/// <summary>
		/// Executes this instance.
		/// </summary>
		public override void Execute()
		{
			base.ExecutePageHierarchy();
		}
	}
}
