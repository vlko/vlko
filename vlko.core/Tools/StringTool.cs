using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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
		public static string Truncate(this string source, int length, string continueText = null)
		{
			if (source != null && source.Length > length)
			{
				source = source.Substring(0, length) + continueText;
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


        /// <summary>
        /// Removes the diacritics.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Text with removed diacritic.</returns>
        public static String RemoveDiacritics(this String text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            String normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        public static string RemoveLineBreakAfterParagraph(this string htmlText)
        {
            return Regex.Replace(htmlText, @"\<\/p\>(\s*)", "</p>");
        }

        public static string ReduceSpaces(this string text)
        {
            return Regex.Replace(text, @"(\s{2,})", " ");
        }

        public static string ConvertEncoding(this string text, Encoding sourceEncoding, Encoding destEncoding )
        {
            // Create two different encodings.
            byte[] myBytes = Encoding.UTF8.GetBytes(text);

            // Perform the conversion from one encoding to the other.
            byte[] originalBytes = Encoding.Convert(Encoding.UTF8, sourceEncoding, myBytes);
            byte[] convertedBytes = Encoding.Convert(destEncoding, Encoding.UTF8, originalBytes);

            return Encoding.UTF8.GetString(convertedBytes);
        }

        public static string ShortHash(this string text)
        {
            var allowedSymbols = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
            var hash = new char[6];

            for (int i = 0; i < text.Length; i++)
            {
                hash[i % 6] = (char)(hash[i % 6] ^ text[i]);
            }

            for (int i = 0; i < 6; i++)
            {
                hash[i] = allowedSymbols[hash[i] % allowedSymbols.Length];
            }

            return new string(hash);
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        public static string ToHex(byte[] bytes, bool upperCase = false)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }
        public static string sha256(IEnumerable<string> values)
        {
            return sha256(string.Join(",", values));
        }

        public static string sha256(string value)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                return ToHex(mySHA256.ComputeHash(Encoding.UTF8.GetBytes(value)));
            }
        }
    }
}
