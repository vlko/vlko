using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Globalization;

namespace vlko.core.web.HtmlExtender
{
    public static class Localization
    {
        public static string GetCurrentJsDatePattern(this HtmlHelper htmlHelper)
        {
            switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
            {
                case "sk":
                    return "d. m. yy";
                default:
                    return "yy-mm-dd";
            }
        }
        public static string FormatDateValue(this HtmlHelper htmlHelper, object value)
        {
            DateTime dateVal = (DateTime)value;
            switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
            {
                case "sk":
                    return dateVal.ToString("d. M. yyyy");
                default:
                    return dateVal.ToString("yyyy-MM-dd");
            }
        }
    }
}
