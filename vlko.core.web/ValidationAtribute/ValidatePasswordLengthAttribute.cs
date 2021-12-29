using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using vlko.core.Roots;

namespace vlko.core.web.ValidationAtribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute
    {


        private const string DefaultErrorMessage = "'{0}' musí mať minimálnu dĺžku {1} znakov.";
        private readonly int _minCharacters = WebSettings.MinRequiredPasswordLength;

        public ValidatePasswordLengthAttribute()
            : base(DefaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                                 name, _minCharacters);
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return valueAsString != null && valueAsString.Length >= _minCharacters;
        }
    }
}