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
