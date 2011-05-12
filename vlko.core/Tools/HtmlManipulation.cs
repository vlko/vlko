using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Security.Application;

namespace vlko.core.Tools
{
	public static class HtmlManipulation
	{
		/// <summary>
		/// Removes the tags.
		/// </summary>
		/// <param name="htmlInput">The HTML input.</param>
		/// <returns>Html input with removed tags.</returns>
		public static string RemoveTags(this string htmlInput)
		{
			string result = Sanitizer.GetSafeHtmlFragment(htmlInput).Trim();
			return Regex.Replace(result, @"<(.|\n)*?>", string.Empty);
		}

		/// <summary>
		/// Shortens the specified input.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="shortenLength">Length of the shorten.</param>
		/// <param name="additionalShortenText">The additional shorten text.</param>
		/// <returns>Shorten text</returns>
		public static string Shorten(this string input, int shortenLength, string additionalShortenText = null)
		{
			if (!string.IsNullOrEmpty(additionalShortenText))
			{
				shortenLength -= additionalShortenText.Length;
			}
			if (input.Length > shortenLength)
			{
				return input.Substring(0, shortenLength) 
					+ (string.IsNullOrEmpty(additionalShortenText) 
						? string.Empty
						: additionalShortenText);
			}

			return input;
		}
	}
}
