using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using vlko.core.Roots;

namespace vlko.core.ValidationAtribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ValidatePasswordLengthAttribute : ValidationAttribute
	{


		private const string DefaultErrorMessage = "'{0}' must be at least {1} characters long.";
		private readonly int _minCharacters = Settings.MinRequiredPasswordLength;

		public ValidatePasswordLengthAttribute()
			: base(DefaultErrorMessage)
		{
		}

		public override string FormatErrorMessage(string name)
		{
			return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
								 name, _minCharacters);
		}

		public override bool IsValid(object value)
		{
			string valueAsString = value as string;
			return (valueAsString != null && valueAsString.Length >= _minCharacters);
		}
	}
}