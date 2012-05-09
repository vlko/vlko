using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Globalization;
using System.Text.RegularExpressions;

namespace vlko.core.Base.Setting
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
        	if (type == typeof(Int32))
            {
                return Convert.ToInt32(stringValue, CultureInfo.InvariantCulture);
            }
        	if (type == typeof(Int64))
        	{
        		return Convert.ToInt64(stringValue, CultureInfo.InvariantCulture);
        	}
        	if (type == typeof(Double))
        	{
        		return Convert.ToBoolean(stringValue, CultureInfo.InvariantCulture);
        	}
        	if (type == typeof(Double))
        	{
        		return Convert.ToDouble(stringValue, CultureInfo.InvariantCulture);
        	}
        	if (type == typeof(DateTime))
        	{
        		return Convert.ToDateTime(stringValue, CultureInfo.InvariantCulture);
        	}
        	if (type == typeof(Boolean))
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
        	if (value.GetType() == typeof(Int32))
        	{
        		return ((Int32)value).ToString(CultureInfo.InvariantCulture);
        	}
        	if (value.GetType() == typeof(Int64))
        	{
        		return ((Int64)value).ToString(CultureInfo.InvariantCulture);
        	}
        	if (value.GetType() == typeof(bool))
        	{
        		return ((bool)value).ToString(CultureInfo.InvariantCulture);
        	}
        	if (value.GetType() == typeof(Double))
        	{
        		return ((Double)value).ToString(CultureInfo.InvariantCulture);
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
        	return value.ToString();
        }
    }
}
