using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vlko.core.Components
{
    public class FilterRoute : Dictionary<string, object>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public FilterRoute()
        {

        }
        /// <summary>
        /// Init from dictionary
        /// </summary>
        public FilterRoute(IDictionary<string, object> dictionary) : base(dictionary)
        {
        }

        /// <summary>
        /// Init from object
        /// </summary>
        /// <param name="source"></param>
        public FilterRoute(object source)
        {
            InitFromObject(source);
        }

        public void AddMultiple<T>( string ident, T[] values)
        {
            if (values != null)
            {
                if (values.Length == 1)
                {
                    Add(ident, values[0]);
                }
                else
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        var k = string.Format("{0}[{1}]", ident, i);
                        Add(k, values[i]);
                    }
                }
            }
        }

        private void InitFromObject(object source)
        {
            if (source == null)
                return;

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                AddPropertyToDictionary<object>(property, source);
        }

        private void AddPropertyToDictionary<T>(PropertyDescriptor property, object source)
        {
            object value = property.GetValue(source);
            if (IsOfType<T>(value))
                Add(property.Name, (T)value);
        }

        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }
    }
}
