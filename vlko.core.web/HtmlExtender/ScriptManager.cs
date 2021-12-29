using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using vlko.core.web.Tools;

namespace vlko.core.web.HtmlExtender
{
    public static class ScriptManager
    {
        /// <summary>
        /// Gets the registered script includes.
        /// </summary>
        /// <returns>Registered script includes.</returns>
        private static SortedList<int, string> GetRegisteredRootScriptIncludes()
        {
            var registeredScriptIncludes = WebHostedEnvironment.CurrentHttpContext.Items["RegisteredRootScriptIncludes"] as SortedList<int, string>;

            if (registeredScriptIncludes == null)
            {
                registeredScriptIncludes = new SortedList<int, string>();
                WebHostedEnvironment.CurrentHttpContext.Items["RegisteredRootScriptIncludes"] = registeredScriptIncludes;
            }

            return registeredScriptIncludes;
        }

        /// <summary>
        /// Gets the registered inline script includes.
        /// </summary>
        /// <returns>Registered inline script includes.</returns>
        private static List<string> GetRegisteredInlineScriptIncludes()
        {
            var registeredScriptIncludes = WebHostedEnvironment.CurrentHttpContext.Items["RegisteredInlineScriptIncludes"] as List<string>;

            if (registeredScriptIncludes == null)
            {
                registeredScriptIncludes = new List<string>();
                WebHostedEnvironment.CurrentHttpContext.Items["RegisteredInlineScriptIncludes"] = registeredScriptIncludes;
            }

            return registeredScriptIncludes;
        }

        /// <summary>
        /// Gets the registered script includes.
        /// </summary>
        /// <returns>Registered script includes.</returns>
        private static SortedList<int, string> GetRegisteredScriptIncludes()
        {
            var registeredScriptIncludes = WebHostedEnvironment.CurrentHttpContext.Items["RegisteredScriptIncludes"] as SortedList<int, string>;

            if (registeredScriptIncludes == null)
            {
                registeredScriptIncludes = new SortedList<int, string>();
                WebHostedEnvironment.CurrentHttpContext.Items["RegisteredScriptIncludes"] = registeredScriptIncludes;
            }

            return registeredScriptIncludes;
        }


        /// <summary>
        /// Include root script (skipped in ajax).
        /// </summary>
        /// <param name="urlHelper">The url helper.</param>
        /// <param name="releaseFile">The release file.</param>
        /// <param name="fallbackTypeCheck">The fallback type check (value to be checked for undefined to load fallback file).</param>
        /// <param name="fallbackFile">The fallback file.</param>
        public static void ScriptRootInclude(this IUrlHelper urlHelper, string releaseFile, string fallbackTypeCheck = null, string fallbackFile = null)
        {
            ScriptManager.ScriptRootDebugInclude(urlHelper, releaseFile, releaseFile, fallbackTypeCheck, fallbackFile);
        }

        /// <summary>
        /// Include root script (skipped in ajax).
        /// </summary>
        /// <param name="urlHelper">The url helper.</param>
        /// <param name="releaseFile">The release file.</param>
        /// <param name="debugFile">The debug file.</param>
        /// <param name="fallbackTypeCheck">The fallback type check (value to be checked for undefined to load fallback file).</param>
        /// <param name="fallbackFile">The fallback file.</param>
        public static void ScriptRootDebugInclude(this IUrlHelper urlHelper, string releaseFile, string debugFile, string fallbackTypeCheck = null, string fallbackFile = null)
        {
            string file = WebHostedEnvironment.IsDevelMode ? debugFile : releaseFile;
            var script = RenderScriptContent(urlHelper, file);

            if (!string.IsNullOrEmpty(fallbackTypeCheck) && !string.IsNullOrEmpty(fallbackFile))
            {
                script +=
                    string.Format(
                        @"<script type=""text/javascript"">
if (typeof({0}) == 'undefined')
{{
    document.write(unescape(""%3Cscript src='{1}' type='text/javascript'%3E%3C/script%3E""));
}}
</script>",
                        fallbackTypeCheck, GenerateContentUrl(urlHelper, fallbackFile));
            }
            var getRegisteredRootScriptIncludes = GetRegisteredRootScriptIncludes();
            if (!getRegisteredRootScriptIncludes.ContainsValue(script))
            {
                getRegisteredRootScriptIncludes.Add(getRegisteredRootScriptIncludes.Count, script);
            }
        }

        /// <summary>
        /// Include script (loaded only once in ajax).
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="releaseFile">The release file.</param>
        /// <param name="fallbackFile">The fallback file.</param>
        /// <param name="async">if set to <c>true</c> [async] (default true).</param>
        public static void ScriptInclude(this IUrlHelper urlHelper, string releaseFile, string fallbackFile = null, bool async = true)
        {
            ScriptManager.ScriptDebugInclude(urlHelper, releaseFile, releaseFile, fallbackFile, async);
        }


        /// <summary>
        /// Include script (loaded only once in ajax).
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="releaseFile">The release file.</param>
        /// <param name="debugFile">The debug file.</param>
        /// <param name="fallbackFile">The fallback file.</param>
        /// <param name="async">if set to <c>true</c> [async] (default true).</param>
        public static void ScriptDebugInclude(this IUrlHelper urlHelper, string releaseFile, string debugFile = null, string fallbackFile = null, bool async = true)
        {
            string file = WebHostedEnvironment.IsDevelMode && !string.IsNullOrEmpty(debugFile) ? debugFile : releaseFile;

            string script = RenderScriptContent(urlHelper, fallbackFile, async, file);

            var getRegisteredScriptIncludes = GetRegisteredScriptIncludes();
            if (!getRegisteredScriptIncludes.ContainsValue(script))
            {
                getRegisteredScriptIncludes.Add(getRegisteredScriptIncludes.Count, script);
            }
        }

        public static string RenderScriptContent(IUrlHelper urlHelper, string file, bool async = true, string fallbackFile = null, string hash = null)
        {
            var script = string.Format("<script src='{0}' {1}type='text/javascript'></script>", GenerateContentUrl(urlHelper, file, hash), async ? "async " : null);
            if (!string.IsNullOrEmpty(fallbackFile))
            {
                script = string.Format("scriptCache.{1}(\"{0}\", \"{2}\");",
                    GenerateContentUrl(urlHelper, file, hash), async ? "load" : "loadSync", GenerateContentUrl(urlHelper, fallbackFile, hash));
            }

            return script;
        }

        /// <summary>
        /// Include asp.net script (loaded only once in ajax).
        /// </summary>
        /// <param name="jsFile">The js file.</param>
        /// <param name="async">if set to <c>true</c> [async] (default true).</param>
        public static void ScriptAspNetInclude(string jsFile, bool async = true)
        {
            var script = string.Format("scriptCache.{1}(\"{0}\");", jsFile, async ? "load" : "loadSync");

            var getRegisteredScriptIncludes = GetRegisteredScriptIncludes();
            if (!getRegisteredScriptIncludes.ContainsValue(script))
            {
                getRegisteredScriptIncludes.Add(getRegisteredScriptIncludes.Count, script);
            }
        }

        /// <summary>
        /// Generates the content URL.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="file">The file.</param>
        /// <returns>Content url.</returns>
        private static string GenerateContentUrl(IUrlHelper urlHelper, string file, string hash = null)
        {
            string src;
            if (IsRelativeToDefaultPath(file))
            {
                src = "~/Scripts/" + file;
            }
            else
            {
                src = file;
            }

            if (!string.IsNullOrEmpty(hash))
            {
                src += (src.IndexOf('?') < 0 ? "?_" : "&_") + "v=" + hash;
            }
            return urlHelper.Content(src);
        }

        /// <summary>
        /// Include script
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="inlineScript">The inline script.</param>
        public static IHtmlContent ScriptInlineInclude(this IHtmlHelper helper, Func<IHtmlHelper, IHtmlContent> inlineScript)
        {
            var registeredInlneScriptIncludes = GetRegisteredInlineScriptIncludes();
            using (var writer = new System.IO.StringWriter())
            {
                inlineScript(helper).WriteTo(writer, HtmlEncoder.Default);
                registeredInlneScriptIncludes.Add(writer.ToString());
            }
            return null;
        }

        /// <summary>
        /// Renders the scripts.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <returns>Rendered script resources</returns>
        public static IHtmlContent RenderScripts(this IHtmlHelper htmlHelper)
        {
            var registeredRootScriptIncludes = GetRegisteredRootScriptIncludes();
            var registeredScriptIncludes = GetRegisteredScriptIncludes();

            var scripts = new StringBuilder();

            // root script load only if request is not ajax
            if (!htmlHelper.ViewContext.HttpContext.Request.IsAjaxRequest())
            {
                foreach (string script in registeredRootScriptIncludes.Values)
                {
                    scripts.AppendLine(script);
                }
            }

            // load script with cache support
            if (registeredScriptIncludes.Count > 0)
            {
                scripts.AppendLine("<script type=\"text/javascript\">");
                foreach (string script in registeredScriptIncludes.Values)
                {
                    scripts.AppendLine(script);
                }
                scripts.AppendLine("</script>");
            }

            return htmlHelper.Raw(scripts.ToString());
        }

        /// <summary>
        /// Renders the inline scripts.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        public static IHtmlContent RenderInlineScripts(this IHtmlHelper htmlHelper)
        {
            var registeredInlineScriptIncludes = GetRegisteredInlineScriptIncludes();
            var inlineScripts = new StringBuilder();
            foreach (string script in registeredInlineScriptIncludes)
            {
                inlineScripts.AppendLine(script);
            }
            return htmlHelper.Raw(inlineScripts.ToString());
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
