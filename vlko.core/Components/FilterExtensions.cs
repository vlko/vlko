using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vlko.core.Components
{
    public static class FilterExtensions
    {
        public static T[] RemoveNullItems<T>(this T[] array)
        {
            if (array != null)
            {
                return array.Where(x => x != null).ToArray();
            }
            return array;
        }
    }
}
