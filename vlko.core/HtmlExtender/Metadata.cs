using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using Microsoft.Web.Mvc.AspNet4;

namespace vlko.core.HtmlExtender
{
	public static class Metadata
	{
		/// <summary>
		/// Descriptions for expression.
		/// </summary>
		/// <typeparam name="TModel">The type of the model.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="htmlHelper">The HTML helper.</param>
		/// <param name="expression">The expression.</param>
		/// <returns>Description based on model metadata.</returns>
		public static string DescriptionFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
		{
			return ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Description;
		}

		/// <summary>
		/// Maximum the length specified by annotations StringLengthAttribute.
		/// </summary>
		/// <typeparam name="TModel">The type of the model.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="htmlHelper">The HTML helper.</param>
		/// <param name="expression">The expression.</param>
		/// <returns>Max length from StringLengthAttribute.</returns>
		public static int? MaxLength<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
		{
			var modelMetaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			var lengthValidator = modelMetaData.ContainerType.GetProperty(modelMetaData.PropertyName)
				.GetCustomAttributes(typeof(StringLengthAttribute), false)
				.FirstOrDefault() as StringLengthAttribute;
			if (lengthValidator != null)
			{
				return lengthValidator.MaximumLength;
			}
			return null;
		}
	}
}
