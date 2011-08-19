using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using vlko.core.Base;

namespace vlko.core.HtmlExtender
{
	public static class FormExtensions
	{
		public static MvcForm BeginForm(this HtmlHelper htmlHelper, object dynamicRoute, FormMethod method = FormMethod.Post, string accept = null, string acceptCharset = null, string cssClass = null, string dir = null, string encType = null, string id = null, string lang = null, string name = null, string style = null, string title = null)
		{
			var dynamicRoutes = (DynamicRoutesResult)dynamicRoute;
			return System.Web.Mvc.Html.FormExtensions.BeginRouteForm(htmlHelper, dynamicRoutes.RouteName, dynamicRoutes.RouteValues, method, HtmlHelperExtensions.FormAttributes(accept, acceptCharset, cssClass, dir, encType, id, lang, name, style, title));
		}
	}
}
