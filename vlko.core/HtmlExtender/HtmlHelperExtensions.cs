using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace vlko.core.HtmlExtender
{
	public static class HtmlHelperExtensions
	{
		public static IDictionary<string, object> AnchorAttributes(string accessKey, string charset, string coords, string cssClass, string dir, string hrefLang, string id, string lang, string name, string rel, string rev, string shape, string style, string target, string title)
		{
			IDictionary<string, object> dictionary = Attributes(cssClass, id, style);
			AddOptional(dictionary, "accesskey", (object)accessKey);
			AddOptional(dictionary, "charset", (object)charset);
			AddOptional(dictionary, "coords", (object)coords);
			AddOptional(dictionary, "dir", (object)dir);
			AddOptional(dictionary, "hreflang", (object)hrefLang);
			AddOptional(dictionary, "lang", (object)lang);
			AddOptional(dictionary, "name", (object)name);
			AddOptional(dictionary, "rel", (object)rel);
			AddOptional(dictionary, "rev", (object)rev);
			AddOptional(dictionary, "shape", (object)shape);
			AddOptional(dictionary, "target", (object)target);
			AddOptional(dictionary, "title", (object)title);
			return dictionary;
		}

		public static IDictionary<string, object> FormAttributes(string accept, string acceptCharset, string cssClass, string dir, string encType, string id, string lang, string name, string style, string title)
		{
			IDictionary<string, object> dictionary = HtmlHelperExtensions.Attributes(cssClass, id, style);
			AddOptional(dictionary, "accept", (object)accept);
			AddOptional(dictionary, "accept-charset", (object)acceptCharset);
			AddOptional(dictionary, "dir", (object)dir);
			AddOptional(dictionary, "enctype", (object)encType);
			AddOptional(dictionary, "lang", (object)lang);
			AddOptional(dictionary, "name", (object)name);
			AddOptional(dictionary, "title", (object)title);
			return dictionary;
		}

		public static IDictionary<string, object> Attributes(string cssClass, string id, string style)
		{
			RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
			AddOptional((IDictionary<string, object>)routeValueDictionary, "class", (object)cssClass);
			AddOptional((IDictionary<string, object>)routeValueDictionary, "id", (object)id);
			AddOptional((IDictionary<string, object>)routeValueDictionary, "style", (object)style);
			return (IDictionary<string, object>)routeValueDictionary;
		}

		public static void AddOptional(this IDictionary<string, object> dictionary, string key, object value)
		{
			if (value == null)
				return;
			dictionary[key] = value;
		}
	}
}
