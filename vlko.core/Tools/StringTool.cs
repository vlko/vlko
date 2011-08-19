using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

		/// <summary>
		/// Parses a camel cased or pascal cased string and returns an array
		/// of the words within the string.
		/// </summary>
		/// <example>
		/// The string "PascalCasing" will return an array with two
		/// elements, "Pascal" and "Casing".
		/// </example>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string[] SplitUpperCase(string source)
		{
			if (source == null)
				return new string[] { }; //Return empty array.

			if (source.Length == 0)
				return new string[] { "" };

			StringCollection words = new StringCollection();
			int wordStartIndex = 0;

			char[] letters = source.ToCharArray();
			// Skip the first letter. we don't care what case it is.
			for (int i = 1; i < letters.Length; i++)
			{
				if (Char.IsUpper(letters[i]))
				{
					//Grab everything before the current index.
					words.Add(new String(letters, wordStartIndex, i - wordStartIndex));
					wordStartIndex = i;
				}
			}

			//We need to have the last word.
			words.Add(new String(letters, wordStartIndex, letters.Length - wordStartIndex));

			//Copy to a string array.
			string[] wordArray = new string[words.Count];
			words.CopyTo(wordArray, 0);
			return wordArray;
		}
	}
}
