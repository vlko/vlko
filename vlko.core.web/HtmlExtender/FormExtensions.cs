using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using vlko.core.web.Base;

namespace vlko.core.web.HtmlExtender
{
    public static class FormExtensions
    {
        public static MvcForm BeginMvcForm(this IHtmlHelper htmlHelper, object dynamicRoute, FormMethod method = FormMethod.Post, string accept = null, string acceptCharset = null, string cssClass = null, string dir = null, string encType = null, string id = null, string lang = null, string name = null, string style = null, string title = null, bool? antiForgery = null)
        {
            var dynamicRoutes = (DynamicRoutesResult)dynamicRoute;
            if (antiForgery == null)
            {
                antiForgery = false;
            }
            return htmlHelper.BeginRouteForm(dynamicRoutes.RouteName, dynamicRoutes.RouteValues, method, antiForgery, FormAttributes(accept, acceptCharset, cssClass, dir, encType, id, lang, name, style, title));
        }
        public static MvcForm BeginMvcForm(this IHtmlHelper htmlHelper, object dynamicRoute, FormMethod method, IDictionary<string, object> htmlAttributes, bool? antiForgery = null)
        {
            var dynamicRoutes = (DynamicRoutesResult)dynamicRoute;
            if (antiForgery == null)
            {
                antiForgery = false;
            }
            return htmlHelper.BeginRouteForm(dynamicRoutes.RouteName, dynamicRoutes.RouteValues, method, antiForgery, htmlAttributes);
        }

        public static IDictionary<string, object> FormAttributes(string accept, string acceptCharset, string cssClass, string dir, string encType, string id, string lang, string name, string style, string title)
        {
            IDictionary<string, object> dictionary = Attributes(cssClass, id, style);
            dictionary.AddOptional("accept", accept);
            dictionary.AddOptional("accept-charset", acceptCharset);
            dictionary.AddOptional("dir", dir);
            dictionary.AddOptional("enctype", encType);
            dictionary.AddOptional("lang", lang);
            dictionary.AddOptional("name", name);
            dictionary.AddOptional("title", title);
            return dictionary;
        }

        public static IDictionary<string, object> Attributes(string cssClass, string id, string style)
        {
            IDictionary<string, object> routeValueDictionary = new Dictionary<string, object>();
            routeValueDictionary.AddOptional("class", cssClass);
            routeValueDictionary.AddOptional("id", id);
            routeValueDictionary.AddOptional("style", style);
            return routeValueDictionary;
        }

        public static void AddOptional(this IDictionary<string, object> dictionary, string key, object value)
        {
            if (value == null)
                return;
            dictionary[key] = value;
        }
    }
}
