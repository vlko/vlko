using System;

namespace vlko.core.web.ValidationAtribute
{
    /// <summary>
    /// Use this attribute to prevent any changes to text.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AntiXssIgnoreAttribute : Attribute
    {
    }
}