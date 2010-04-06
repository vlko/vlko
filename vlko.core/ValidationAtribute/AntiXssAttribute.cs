using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Microsoft.Security.Application;

namespace vlko.core.ValidationAtribute
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AntiXssAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
            foreach (PropertyDescriptor property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    ProcessProperty(value, property);
                }
            }
            return true;
        }

        /// <summary>
        /// Processes the property.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="property">The property.</param>
        private static void ProcessProperty(object value, PropertyDescriptor property)
        {
            if (property.Attributes.Contains(new AntiXssHtmlTextAttribute()))
            {
                property.SetValue(value, AntiXss.GetSafeHtmlFragment((string) property.GetValue(value)));
            }
            else
            {
                property.SetValue(value, AntiXss.HtmlEncode((string)property.GetValue(value)));
            }
        }
    }
}
