using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace vlko.core.web.HtmlExtender
{
    public static class CssManager
    {
        /// <summary>
        /// Gets the registered CSS includes.
        /// </summary>
        /// <returns>Registered CSS includes.</returns>
        private static SortedList<int, string> GetRegisteredCssIncludes()
        {
            var registeredCssIncludes = WebHostedEnvironment.CurrentHttpContext.Items["RegisteredCssIncludes"] as SortedList<int, string>;

            if (registeredCssIncludes == null)
            {
                registeredCssIncludes = new SortedList<int, string>();
                WebHostedEnvironment.CurrentHttpContext.Items["RegisteredCssIncludes"] = registeredCssIncludes;
            }

            return registeredCssIncludes;
        }

        /// <summary>
        ///Registers the css include for asp.net.
        /// </summary>
        /// <param name="cssLink">The CSS link.</param>
        public static IHtmlContent CssAspNetInclude(string cssLink)
        {
            var registeredCssIncludes = GetRegisteredCssIncludes();
            if (!registeredCssIncludes.ContainsValue(cssLink))
            {
                registeredCssIncludes.Add(registeredCssIncludes.Count, cssLink);
            }
            return null;
        }

        /// <summary>
        /// Registers the css include.
        /// </summary>
        /// <param name="urlHelper">The url helper.</param>
        /// <param name="cssFile">The CSS file.</param>
        /// <param name="mediaType">Type of the media.</param>
        public static IHtmlContent CssInclude(this IUrlHelper urlHelper, string cssFile, string develCssFile = null)
        {
            string css = WebHostedEnvironment.IsDevelMode && !string.IsNullOrEmpty(develCssFile)
                ? RenderStylesheetLink(urlHelper, develCssFile)
                : RenderStylesheetLink(urlHelper, cssFile);
            var registeredCssIncludes = GetRegisteredCssIncludes();
            if (!registeredCssIncludes.ContainsValue(css))
            {
                registeredCssIncludes.Add(registeredCssIncludes.Count, css);
            }
            return null;
        }

        internal static string RenderStylesheetLink(IUrlHelper urlHelper, string file, string hash = null)
        {
            string src;
            if (IsRelativeToDefaultPath(file))
            {
                src = "~/Content/" + file;
            }
            else
            {
                src = file;
            }

            if (!string.IsNullOrEmpty(hash))
            {
                src += (src.IndexOf('?') < 0 ? "?_" : "&_") + "v=" + hash;
            }
            var css = string.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", urlHelper.Content(src));
            return css;
        }

        /// <summary>
        /// Renders the CSS resources.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <returns>Rendered css resources.</returns>
        public static IHtmlContent RenderCss(this IHtmlHelper htmlHelper)
        {
            var registeredCssIncludes = GetRegisteredCssIncludes();
            var cssResources = new StringBuilder();
            foreach (string css in registeredCssIncludes.Values)
            {
                cssResources.AppendLine(css);
            }
            return htmlHelper.Raw(cssResources.ToString());
        }

        /// <summary>
        /// Determines whether [is relative to default path] [the specified file].
        /// </summary>
        /// <param name="file">The file.</param>
        /// <remarks>Copy from MVCFutures.</remarks>
        /// <returns>
        /// 	<c>true</c> if [is relative to default path] [the specified file]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsRelativeToDefaultPath(string file)
        {
            return !file.StartsWith("~", StringComparison.Ordinal) && !file.StartsWith("../", StringComparison.Ordinal) && !file.StartsWith("/", StringComparison.Ordinal) && !file.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !file.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }

    }
}
