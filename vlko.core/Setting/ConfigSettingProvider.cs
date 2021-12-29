using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using vlko.core.InversionOfControl;

namespace vlko.core.Setting
{
    public class ConfigSettingProvider : ISettingProvider
    {

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Setting Value for specified name.</returns>
        public bool GetValue(string name, ref string value)
        {
            var configuration = IoC.Scope.Resolve<IConfiguration>();
            if (configuration != null)
            {
                var appSettings = configuration.GetSection("AppSettings");
                if (appSettings.Exists())
                {
                    var valueSection = appSettings.GetSection(name);
                    if (valueSection.Exists())
                    {
                        value = valueSection.Value;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Saves the value for specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void SaveValue(string name, string value)
        {
            // we are not allowed to save values to config file
            throw new NotImplementedException();
        }

        public void SaveValueWithoutTransaction(string name, string value)
        {
            // we are not allowed to save values to config file
            throw new NotImplementedException();
        }
    }
}