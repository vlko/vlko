using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace vlko.core.web.HtmlExtender
{
    public static class ModelStateExtensions
    {
        public static bool IsValidField(this ModelStateDictionary modelState, string fieldName)
        {
            if (modelState != null && modelState.ContainsKey(fieldName))
            {
                return modelState[fieldName].ValidationState != ModelValidationState.Invalid;
            }
            return true;
        }
    }
}
