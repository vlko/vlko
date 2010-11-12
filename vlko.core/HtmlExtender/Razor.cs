using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace vlko.core.HtmlExtender
{
	public static class Razor
	{
		/// <summary>
		/// Display text in raw format.
		/// </summary>
		/// <param name="htmlHelper">The HTML helper.</param>
		/// <param name="rawText">The raw text.</param>
		/// <returns></returns>
		public static MvcHtmlString Raw(this HtmlHelper htmlHelper, string rawText)
		{
			return MvcHtmlString.Create(rawText);
		}
	}
}
