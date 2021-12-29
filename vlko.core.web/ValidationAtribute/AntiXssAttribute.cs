using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using vlko.core.Tools;

namespace vlko.core.web.ValidationAtribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AntiXssAttribute : ActionFilterAttribute
    {
        private static readonly AntiXssHtmlTextAttribute AntiXssHtmlText = new AntiXssHtmlTextAttribute();
        private static readonly AntiXssIgnoreAttribute AntiXssIgnore = new AntiXssIgnoreAttribute();
        /// <summary>
        /// Processes the property.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="property">The property.</param>
        private static void ProcessProperty(object value, PropertyDescriptor property)
        {
            if (property.Attributes.Contains(AntiXssHtmlText))
            {
                property.SetValue(value, HtmlSanitizer.Sanitize((string)property.GetValue(value)));
            }
            else if (property.Attributes.Contains(AntiXssIgnore))
            {
                // do nothing this contains special text
            }
            else
            {
                property.SetValue(value, HttpUtility.HtmlDecode(HttpUtility.HtmlEncode((string)property.GetValue(value))));
            }
        }

        /// <summary>
        /// Called by the MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            foreach (KeyValuePair<string, object> actionParameter in filterContext.ActionArguments.ToArray())
            {
                var value = actionParameter.Value;
                if (value is string)
                {
                    filterContext.ActionArguments[actionParameter.Key] = HttpUtility.HtmlDecode(HttpUtility.HtmlEncode((string)value));
                }
                else
                {
                    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
                    foreach (PropertyDescriptor property in properties)
                    {
                        if (property.PropertyType == typeof(string))
                        {
                            ProcessProperty(value, property);
                        }
                    }
                }
            }
        }

    }
}
