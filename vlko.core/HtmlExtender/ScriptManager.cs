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
		private static SortedList<int, string> GetRegisteredRootScriptIncludes()
		{
			var registeredScriptIncludes = System.Web.HttpContext.Current.Items["RegisteredRootScriptIncludes"] as SortedList<int, string>;

			if (registeredScriptIncludes == null)
			{
				registeredScriptIncludes = new SortedList<int, string>();
				System.Web.HttpContext.Current.Items["RegisteredRootScriptIncludes"] = registeredScriptIncludes;
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
		/// Include root script (skipped in ajax).
		/// </summary>
		/// <param name="htmlHelper">The HTML helper.</param>
		/// <param name="releaseFile">The release file.</param>
		public static void ScriptRootInclude(this HtmlHelper htmlHelper, string releaseFile)
		{
			ScriptRootInclude(htmlHelper, releaseFile, releaseFile);
		}

		/// <summary>
		/// Include root script (skipped in ajax).
		/// </summary>
		/// <param name="htmlHelper">The HTML helper.</param>
		/// <param name="releaseFile">The release file.</param>
		/// <param name="debugFile">The debug file.</param>
		public static void ScriptRootInclude(this HtmlHelper htmlHelper, string releaseFile, string debugFile)
		{
			var script = Microsoft.Web.Mvc.ScriptExtensions.Script(htmlHelper, releaseFile, debugFile).ToString();
			var getRegisteredRootScriptIncludes = GetRegisteredRootScriptIncludes();
			if (!getRegisteredRootScriptIncludes.ContainsValue(script))
			{
				getRegisteredRootScriptIncludes.Add(getRegisteredRootScriptIncludes.Count, script);
			}
		}

		/// <summary>
		/// nclude script (loaded only once in ajax).
		/// </summary>
		/// <param name="htmlHelper">The HTML helper.</param>
		/// <param name="releaseFile">The release file.</param>
		public static void ScriptInclude(this HtmlHelper htmlHelper, string releaseFile)
		{
			ScriptInclude(htmlHelper, releaseFile, releaseFile);
		}


		/// <summary>
		/// Include script (loaded only once in ajax).
		/// </summary>
		/// <param name="htmlHelper">The HTML helper.</param>
		/// <param name="releaseFile">The release file.</param>
		/// <param name="debugFile">The debug file.</param>
		public static void ScriptInclude(this HtmlHelper htmlHelper, string releaseFile, string debugFile)
		{
			string src;
			string file = htmlHelper.ViewContext.HttpContext.IsDebuggingEnabled ? debugFile : releaseFile;
			if (IsRelativeToDefaultPath(file))
			{
				src = "~/Scripts/" + file;
			}
			else
			{
				src = file;
			}

			var script = string.Format("scriptCache.load(\"{0}\");", 
				UrlHelper.GenerateContentUrl(src, htmlHelper.ViewContext.HttpContext));

			var getRegisteredScriptIncludes = GetRegisteredScriptIncludes();
			if (!getRegisteredScriptIncludes.ContainsValue(script))
			{
				getRegisteredScriptIncludes.Add(getRegisteredScriptIncludes.Count, script);
			}
		}

		/// <summary>
		/// Include script
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
				scripts.AppendLine("<script type=\"text/javascript\">$(function() {");
				foreach (string script in registeredScriptIncludes.Values)
				{
					scripts.AppendLine(script);
				}
				scripts.AppendLine("});</script>");
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
			return (((!file.StartsWith("~", StringComparison.Ordinal) && !file.StartsWith("../", StringComparison.Ordinal)) && (!file.StartsWith("/", StringComparison.Ordinal) && !file.StartsWith("http://", StringComparison.OrdinalIgnoreCase))) && !file.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
		}
	}
}
