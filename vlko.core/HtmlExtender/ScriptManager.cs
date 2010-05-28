using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace vlko.core.HtmlExtender
{
    public static class ScriptManager
    {
        /// <summary>
        /// Gets the registered script includes.
        /// </summary>
        /// <returns>Registered script includes.</returns>
        private static SortedList<int, string> GetRegisteredScriptIncludes()
        {
            var registeredScriptIncludes = System.Web.HttpContext.Current.Items["RegisteredScriptIncludes"] as SortedList<int, string>;

            if (registeredScriptIncludes == null)
            {
                registeredScriptIncludes = new SortedList<int, string>();
                System.Web.HttpContext.Current.Items["RegisteredScriptIncludes"] = registeredScriptIncludes;
            }

            return registeredScriptIncludes;
        }

        /// <summary>
        /// Gets the registered inline script includes.
        /// </summary>
        /// <returns>Registered inline script includes.</returns>
        private static List<Action> GetRegisteredInlineScriptIncludes()
        {
            var registeredScriptIncludes = System.Web.HttpContext.Current.Items["RegisteredInlineScriptIncludes"] as List<Action>;

            if (registeredScriptIncludes == null)
            {
                registeredScriptIncludes = new List<Action>();
                System.Web.HttpContext.Current.Items["RegisteredInlineScriptIncludes"] = registeredScriptIncludes;
            }

            return registeredScriptIncludes;
        }

        /// <summary>
        /// Scripts the include.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="releaseFile">The release file.</param>
        public static void ScriptInclude(this HtmlHelper htmlHelper, string releaseFile)
        {
            ScriptInclude(htmlHelper, releaseFile, releaseFile);
        }

        /// <summary>
        /// Scripts the include.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="releaseFile">The release file.</param>
        /// <param name="debugFile">The debug file.</param>
        public static void ScriptInclude(this HtmlHelper htmlHelper, string releaseFile, string debugFile)
        {
            var script = Microsoft.Web.Mvc.ScriptExtensions.Script(htmlHelper, releaseFile, debugFile).ToString();
            var registeredScriptIncludes = GetRegisteredScriptIncludes();
            if (!registeredScriptIncludes.ContainsValue(script))
            {
                registeredScriptIncludes.Add(registeredScriptIncludes.Count, script);
            }
        }

        /// <summary>
        /// Scripts the inline include.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="inlineScript">The inline script.</param>
        public static void ScriptInlineInclude(this HtmlHelper htmlHelper, Action inlineScript)
        {
            var registeredInlneScriptIncludes = GetRegisteredInlineScriptIncludes();
            registeredInlneScriptIncludes.Add(inlineScript);
        }

        /// <summary>
        /// Renders the scripts.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <returns>Rendered script resources</returns>
        public static string RenderScripts(this HtmlHelper htmlHelper)
        {
            var registeredScriptIncludes = GetRegisteredScriptIncludes();
            var scripts = new StringBuilder();
            foreach (string script in registeredScriptIncludes.Values)
            {
                scripts.AppendLine(script);
            }
            return scripts.ToString();
        }

        /// <summary>
        /// Renders the inline scripts.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        public static void RenderInlineScripts(this HtmlHelper htmlHelper)
        {
            var registeredInlineScriptIncludes = GetRegisteredInlineScriptIncludes();
            foreach (Action script in registeredInlineScriptIncludes)
            {
                script.Invoke();
            }
        }
    }
}
