using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Security.Application;
using System.Text.RegularExpressions;

namespace vlko.core.Tools
{
	public static class HtmlManipulation
	{
		/// <summary>
		/// Removes the tags.
		/// </summary>
		/// <param name="htmlInput">The HTML input.</param>
		/// <returns>Html input with removed tags.</returns>
		public static string RemoveTags(string htmlInput)
		{
			string result = AntiXss.GetSafeHtmlFragment(htmlInput).Trim();
			return Regex.Replace(result, @"<(.|\n)*?>", string.Empty);
		}
	}
}
