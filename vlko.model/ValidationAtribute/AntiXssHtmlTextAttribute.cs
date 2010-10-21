using System;

namespace vlko.model.ValidationAtribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AntiXssHtmlTextAttribute : Attribute
    {
    }
}
