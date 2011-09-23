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
		private static List<string> GetRegisteredInlineScriptIncludes()
		{
			var registeredScriptIncludes = System.Web.HttpContext.Current.Items["RegisteredInlineScriptIncludes"] as List<string>;

			if (registeredScriptIncludes == null)
			{
				registeredScriptIncludes = new List<string>();
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
		/// <param name="fallbackTypeCheck">The fallback type check (value to be checked for undefined to load fallback file).</param>
		/// <param name="fallbackFile">The fallback file.</param>
		public static void ScriptRootInclude(this HtmlHelper htmlHelper, string releaseFile, string fallbackTypeCheck = null, string fallbackFile = null)
		{
			ScriptRootDebugInclude(htmlHelper, releaseFile, releaseFile, fallbackTypeCheck, fallbackFile);
		}

		/// <summary>
		/// Include root script (skipped in ajax).
		/// </summary>
		/// <param name="htmlHelper">The HTML helper.</param>
		/// <param name="releaseFile">The release file.</param>
		/// <param name="debugFile">The debug file.</param>
		/// <param name="fallbackTypeCheck">The fallback type check (value to be checked for undefined to load fallback file).</param>
		/// <param name="fallbackFile">The fallback file.</param>
		public static void ScriptRootDebugInclude(this HtmlHelper htmlHelper, string releaseFile, string debugFile, string fallbackTypeCheck = null, string fallbackFile = null)
		{
			string file = htmlHelper.ViewContext.HttpContext.IsDebuggingEnabled ? debugFile : releaseFile;
			var script = string.Format("<script src='{0}' type='text/javascript'></script>", GenerateContentUrl(htmlHelper, file));

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
						fallbackTypeCheck, GenerateContentUrl(htmlHelper, fallbackFile));
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
		public static void ScriptInclude(this HtmlHelper htmlHelper, string releaseFile, string fallbackFile = null, bool async = true)
		{
			ScriptDebugInclude(htmlHelper, releaseFile, releaseFile, fallbackFile, async);
		}


		/// <summary>
		/// Include script (loaded only once in ajax).
		/// </summary>
		/// <param name="htmlHelper">The HTML helper.</param>
		/// <param name="releaseFile">The release file.</param>
		/// <param name="debugFile">The debug file.</param>
		/// <param name="fallbackFile">The fallback file.</param>
		/// <param name="async">if set to <c>true</c> [async] (default true).</param>
		public static void ScriptDebugInclude(this HtmlHelper htmlHelper, string releaseFile, string debugFile, string fallbackFile = null, bool async = true)
		{
			string file = htmlHelper.ViewContext.HttpContext.IsDebuggingEnabled ? debugFile : releaseFile;

			var script = string.Format("scriptCache.{1}(\"{0}\");", GenerateContentUrl(htmlHelper, file), async ? "load" : "loadSync");

			if (!string.IsNullOrEmpty(fallbackFile))
			{
				script = string.Format("scriptCache.{1}(\"{0}\", \"{2}\");",
					GenerateContentUrl(htmlHelper, file), async ? "load" : "loadSync", GenerateContentUrl(htmlHelper, fallbackFile));
			}

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
		private static string GenerateContentUrl(HtmlHelper htmlHelper, string file)
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
			src += (src.IndexOf('?') < 0 ? "?_" : "&_") + Settings.StaticContentVersion.Value;
			return UrlHelper.GenerateContentUrl(src, htmlHelper.ViewContext.HttpContext);
		}

		/// <summary>
		/// Include script
		/// </summary>
		/// <param name="htmlHelper">The HTML helper.</param>
		/// <param name="inlineScript">The inline script.</param>
		public static void ScriptInlineInclude<TModel>(this HtmlHelper<TModel> htmlHelper, Func<WebViewPage<TModel>, object> inlineScript)
		{
			var registeredInlneScriptIncludes = GetRegisteredInlineScriptIncludes();
			registeredInlneScriptIncludes.Add(inlineScript((WebViewPage<TModel>)htmlHelper.ViewDataContainer).ToString());
		}

		/// <summary>
		/// Include script
		/// </summary>
		/// <param name="htmlHelper">The HTML helper.</param>
		/// <param name="inlineScript">The inline script.</param>
		public static void ScriptInlineInclude(this HtmlHelper htmlHelper, Func<WebViewPage, object> inlineScript)
		{
			var registeredInlneScriptIncludes = GetRegisteredInlineScriptIncludes();
			registeredInlneScriptIncludes.Add(inlineScript((WebViewPage)htmlHelper.ViewDataContainer).ToString());
		}

		/// <summary>
		/// Renders the scripts.
		/// </summary>
		/// <param name="htmlHelper">The HTML helper.</param>
		/// <returns>Rendered script resources</returns>
		public static MvcHtmlString RenderScripts(this HtmlHelper htmlHelper)
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

			return MvcHtmlString.Create(scripts.ToString());
		}

		/// <summary>
		/// Renders the inline scripts.
		/// </summary>
		/// <param name="htmlHelper">The HTML helper.</param>
		public static MvcHtmlString RenderInlineScripts(this HtmlHelper htmlHelper)
		{
			var registeredInlineScriptIncludes = GetRegisteredInlineScriptIncludes();
			var inlineScripts = new StringBuilder();
			foreach (string script in registeredInlineScriptIncludes)
			{
				inlineScripts.AppendLine(script);
			}
			return MvcHtmlString.Create(inlineScripts.ToString());
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
