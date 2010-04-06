using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vlko.core.ValidationAtribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AntiXssHtmlTextAttribute : Attribute
    {
    }
}
