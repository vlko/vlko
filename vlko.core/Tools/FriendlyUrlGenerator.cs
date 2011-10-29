﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace vlko.core.Tools
{
    public static class FriendlyUrlGenerator
    {
        /// <summary>
        /// Generates the specified title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns>Text to be used as friendly URL.</returns>
        public static string Generate(string title)
        {
            // remove diacritics
            title = RemoveDiacritics(title);

            // remove entities
            title = Regex.Replace(title, @"&\w+;", "");

            // remove anything that is not letters, numbers, dash, or space
            title = Regex.Replace(title, @"[^A-Za-z0-9\-\s]", "");

            // remove any leading or trailing spaces left over
            title = title.Trim();

            // replace spaces with single dash
            title = Regex.Replace(title, @"\s+", "-");

            // if we end up with multiple dashes, collapse to single dash
            title = Regex.Replace(title, @"\-{2,}", "-");

            // make it all lower case
            title = title.ToLower();

            // if it's too long, clip it
            if (title.Length > 80)
                title = title.Substring(0, 79);

            // remove trailing dash, if there is one
            if (title.EndsWith("-"))
                title = title.Substring(0, title.Length - 1);

            return title;
        }

        /// <summary>
        /// Removes the diacritics.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Text with removed diacritic.</returns>
        public static String RemoveDiacritics(String text)
        {
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
    }
}
