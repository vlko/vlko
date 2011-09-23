using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Web.Mvc.Html;
using vlko.core.Base;

namespace vlko.core.HtmlExtender
{
	public static class LinkExtensions
	{
		/// <summary>
		/// Generate link for specified dynamic route.
		/// </summary>
		/// <param name="helper">The helper.</param>
		/// <param name="linkText">The link text.</param>
		/// <param name="dynamicRoute">The dynamic route.</param>
		/// <param name="htmlAttributes">The HTML attributes.</param>
		/// <returns></returns>
		public static MvcHtmlString Link(this HtmlHelper helper, string linkText, object dynamicRoute, object htmlAttributes)
		{
			var dynamicRoutes = (DynamicRoutesResult)dynamicRoute;
			return System.Web.Mvc.Html.LinkExtensions.RouteLink(helper, linkText, dynamicRoutes.RouteName, dynamicRoutes.RouteValues, (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

		/// <summary>
		/// Generate link for specified dynamic route.
		/// </summary>
		/// <param name="helper">The helper.</param>
		/// <param name="linkText">The link text.</param>
		/// <param name="dynamicRoute">The dynamic route.</param>
		/// <param name="accessKey">The access key.</param>
		/// <param name="charset">The charset.</param>
		/// <param name="coords">The coords.</param>
		/// <param name="cssClass">The CSS class.</param>
		/// <param name="dir">The dir.</param>
		/// <param name="hrefLang">The href lang.</param>
		/// <param name="id">The id.</param>
		/// <param name="lang">The lang.</param>
		/// <param name="name">The name.</param>
		/// <param name="rel">The rel.</param>
		/// <param name="rev">The rev.</param>
		/// <param name="shape">The shape.</param>
		/// <param name="style">The style.</param>
		/// <param name="target">The target.</param>
		/// <param name="title">The title.</param>
		/// <returns></returns>
		public static MvcHtmlString Link(this HtmlHelper helper, string linkText, object dynamicRoute, string accessKey = null, string charset = null, string coords = null, string cssClass = null, string dir = null, string hrefLang = null, string id = null, string lang = null, string name = null, string rel = null, string rev = null, string shape = null, string style = null, string target = null, string title = null)
		{
			var dynamicRoutes = (DynamicRoutesResult)dynamicRoute;
			return System.Web.Mvc.Html.LinkExtensions.RouteLink(helper, linkText, dynamicRoutes.RouteName, dynamicRoutes.RouteValues, HtmlHelperExtensions.AnchorAttributes(accessKey, charset, coords, cssClass, dir, hrefLang, id, lang, name, rel, rev, shape, style, target, title));
		}

		/// <summary>
		/// Actions the link.
		/// </summary>
		/// <param name="helper">The helper.</param>
		/// <param name="actionType">Type of the action.</param>
		/// <param name="linkText">The link text.</param>
		/// <param name="dynamicRoute">The dynamic route.</param>
		/// <param name="accessKey">The access key.</param>
		/// <param name="charset">The charset.</param>
		/// <param name="coords">The coords.</param>
		/// <param name="cssClass">The CSS class.</param>
		/// <param name="dir">The dir.</param>
		/// <param name="hrefLang">The href lang.</param>
		/// <param name="id">The id.</param>
		/// <param name="lang">The lang.</param>
		/// <param name="name">The name.</param>
		/// <param name="rel">The rel.</param>
		/// <param name="rev">The rev.</param>
		/// <param name="shape">The shape.</param>
		/// <param name="style">The style.</param>
		/// <param name="target">The target.</param>
		/// <param name="title">The title.</param>
		/// <returns></returns>
		public static MvcHtmlString Link(this HtmlHelper helper, string actionType, string linkText, object dynamicRoute, string accessKey = null, string charset = null, string coords = null, string cssClass = null, string dir = null, string hrefLang = null, string id = null, string lang = null, string name = null, string rel = null, string rev = null, string shape = null, string style = null, string target = null, string title = null)
		{
			var dynamicRoutes = (DynamicRoutesResult)dynamicRoute;

			string url = UrlHelper.GenerateUrl(dynamicRoutes.RouteName, null, null,
					dynamicRoutes.RouteValues, helper.RouteCollection, helper.ViewContext.RequestContext, false);

			TagBuilder tagBuilder = new TagBuilder("a")
			                        	{
											InnerHtml = string.Format("<span class=\"icon\"></span><span class=\"caption\">{0}</span>",
			                        		                          (!string.IsNullOrEmpty(linkText)
			                        		                           	? HttpUtility.HtmlEncode(linkText)
			                        		                           	: string.Empty))
			                        	};
			if (string.IsNullOrEmpty(title))
			{
				title = linkText;
			}
			cssClass = "action " + actionType + (cssClass != null ? " " + cssClass : string.Empty);
			tagBuilder.MergeAttributes<string, object>(HtmlHelperExtensions.AnchorAttributes(
																					accessKey, charset, coords, cssClass, dir,
																					hrefLang, id, lang, name, rel, rev, shape,
																					style, target, title));

			tagBuilder.MergeAttribute("href", url);

			return new MvcHtmlString(tagBuilder. ToString(TagRenderMode.Normal));
		}
	}
}
