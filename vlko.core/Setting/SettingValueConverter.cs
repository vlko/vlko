using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Globalization;
using System.Text.RegularExpressions;

namespace vlko.core.Setting
{
    public static class SettingValueConverter
    {
        /// <summary>
        /// Converts to value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stringValue">The string value.</param>
        /// <returns>Value from string.</returns>
        public static T ConvertToValue<T>(string stringValue)
        {
            return (T)ConvertToValue(stringValue, typeof(T));
        }

        /// <summary>
        /// Converts to value.
        /// </summary>
        /// <param name="stringValue">The string value.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object ConvertToValue(string stringValue, Type type)
        {
            if (type == typeof(int))
            {
                return Convert.ToInt32(stringValue, CultureInfo.InvariantCulture);
            }
            if (type == typeof(long))
            {
                return Convert.ToInt64(stringValue, CultureInfo.InvariantCulture);
            }
            if (type == typeof(double))
            {
                return Convert.ToBoolean(stringValue, CultureInfo.InvariantCulture);
            }
            if (type == typeof(double))
            {
                return Convert.ToDouble(stringValue, CultureInfo.InvariantCulture);
            }
            if (type == typeof(DateTime))
            {
                return Convert.ToDateTime(stringValue, CultureInfo.InvariantCulture);
            }
            if (type == typeof(DateTime?))
            {
                return Convert.ToDateTime(stringValue, CultureInfo.InvariantCulture);
            }
            if (type == typeof(bool))
            {
                return Convert.ToBoolean(stringValue, CultureInfo.InvariantCulture);
            }
            if (type == typeof(List<string>))
            {
                List<string> Values = new List<string>();
                MatchCollection matches = Regex.Matches(stringValue, "<item>(.*?)</item>", RegexOptions.Singleline);
                // if match then add items from string
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        Values.Add(HttpUtility.HtmlDecode(match.Groups[1].Value));
                    }
                }
                // return text if no match
                else
                {
                    Values.Add(stringValue);
                }
                return Values;
            }
            if (type == typeof(Dictionary<string, string>))
            {
                Dictionary<string, string> Values = new Dictionary<string, string>();
                MatchCollection matches = Regex.Matches(stringValue, "<item key='(.*?)'>(.*?)</item>", RegexOptions.Singleline);
                // if match then add items from string
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        Values[HttpUtility.HtmlDecode(match.Groups[1].Value)] = HttpUtility.HtmlDecode(match.Groups[2].Value);
                    }
                }

                return Values;
            }
            return stringValue;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ConvertToString(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value.GetType() == typeof(int))
            {
                return ((int)value).ToString(CultureInfo.InvariantCulture);
            }
            if (value.GetType() == typeof(long))
            {
                return ((long)value).ToString(CultureInfo.InvariantCulture);
            }
            if (value.GetType() == typeof(bool))
            {
                return ((bool)value).ToString(CultureInfo.InvariantCulture);
            }
            if (value.GetType() == typeof(double))
            {
                return ((double)value).ToString(CultureInfo.InvariantCulture);
            }
            if (value.GetType() == typeof(DateTime))
            {
                return ((DateTime)value).ToString(CultureInfo.InvariantCulture);
            }
            if (value.GetType() == typeof(List<string>))
            {
                StringBuilder result = new StringBuilder();
                foreach (string item in value as List<string>)
                {
                    result.AppendLine(string.Format("<item>{0}</item>", HttpUtility.HtmlEncode(item)));
                }
                return result.ToString();
            }
            if (value.GetType() == typeof(Dictionary<string, string>))
            {
                StringBuilder result = new StringBuilder();
                foreach (var item in value as Dictionary<string, string>)
                {
                    result.AppendLine(string.Format("<item key='{0}'>{1}</item>", HttpUtility.HtmlEncode(item.Key), HttpUtility.HtmlEncode(item.Value)));
                }
                return result.ToString();
            }
            return value.ToString();
        }
    }
}
