using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vlko.core.Tools
{
	/// <summary>
	/// Custom string utility methods.
	/// </summary>
	public static class StringTool
	{
		/// <summary>
		/// Get a substring of the first N characters.
		/// </summary>
		public static string Truncate(this string source, int length)
		{
			if (source != null && source.Length > length)
			{
				source = source.Substring(0, length);
			}
			return source;
		}
	}
}
