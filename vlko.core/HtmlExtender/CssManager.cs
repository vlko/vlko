using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace vlko.core.HtmlExtender
{
    public static class CssManager
    {
        /// <summary>
        /// Gets the registered CSS includes.
        /// </summary>
        /// <returns>Registered CSS includes.</returns>
        private static SortedList<int, string> GetRegisteredCssIncludes()
        {
            var registeredCssIncludes = System.Web.HttpContext.Current.Items["RegisteredCssIncludes"] as SortedList<int, string>;

            if (registeredCssIncludes == null)
            {
                registeredCssIncludes = new SortedList<int, string>();
                System.Web.HttpContext.Current.Items["RegisteredCssIncludes"] = registeredCssIncludes;
            }

            return registeredCssIncludes;
        }

        /// <summary>
        /// Registers the css include.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="cssFile">The CSS file.</param>
        public static void CssInclude(this HtmlHelper htmlHelper, string cssFile)
        {
            CssInclude(htmlHelper, cssFile, null);
        }

        /// <summary>
        /// Registers the css include.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="cssFile">The CSS file.</param>
        /// <param name="mediaType">Type of the media.</param>
        public static void CssInclude(this HtmlHelper htmlHelper, string cssFile, string mediaType)
        {
            var css = Microsoft.Web.Mvc.CssExtensions.Css(htmlHelper, cssFile, mediaType).ToString();
            var registeredCssIncludes = GetRegisteredCssIncludes();
            if (!registeredCssIncludes.ContainsValue(css))
            {
                registeredCssIncludes.Add(registeredCssIncludes.Count, css);
            }
        }

        /// <summary>
        /// Renders the CSS resources.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <returns>Rendered css resources.</returns>
        public static string RenderCss(this HtmlHelper htmlHelper)
        {
            var registeredCssIncludes = GetRegisteredCssIncludes();
            var cssResources = new StringBuilder();
            foreach (string css in registeredCssIncludes.Values)
            {
                cssResources.AppendLine(css);
            }
            return cssResources.ToString();
        }

    }
}
