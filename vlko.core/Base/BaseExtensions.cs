using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;

namespace vlko.core.Base
{
	public static class BaseExtensions
	{
		/// <summary>
		/// Adds the model error.
		/// </summary>
		/// <typeparam name="TModel">The type of the model.</typeparam>
		/// <param name="modelState">State of the model.</param>
		/// <param name="method">The method.</param>
		/// <param name="message">The message.</param>
		public static void AddModelError<TModel>(this ModelStateDictionary modelState, Expression<Func<TModel, object>> method, string message)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}

			var property = ExpressionHelper.GetExpressionText(method);

			modelState.AddModelError(property, message);
		}
	}
}
