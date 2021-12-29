using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace vlko.core.web.HtmlExtender
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
        public static string DescriptionFor<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            var expresionProvider = htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
            var modelExplorer = expresionProvider.CreateModelExpression<TModel, TValue>(htmlHelper.ViewData, expression);
            if (modelExplorer == null) throw new InvalidOperationException($"Failed to get model explorer for {expresionProvider.GetExpressionText(expression)}");

            return modelExplorer.Metadata?.Description;
        }

        /// <summary>
        /// Maximum the length specified by annotations StringLengthAttribute.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>Max length from StringLengthAttribute.</returns>
        public static int? MaxLength<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            var expresionProvider = htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
            var modelExplorer = expresionProvider.CreateModelExpression<TModel, TValue>(htmlHelper.ViewData, expression);
            if (modelExplorer == null) throw new InvalidOperationException($"Failed to get model explorer for {expresionProvider.GetExpressionText(expression)}");

            for (var i = 0; i < modelExplorer.Metadata.ValidatorMetadata.Count; i++)
            {
                if (modelExplorer.Metadata.ValidatorMetadata[i] is StringLengthAttribute stringLengthAttribute && stringLengthAttribute.MaximumLength > 0)
                {
                    return stringLengthAttribute.MaximumLength;
                }

                if (modelExplorer.Metadata.ValidatorMetadata[i] is MaxLengthAttribute maxLengthAttribute && maxLengthAttribute.Length > 0)
                {
                    return maxLengthAttribute.Length;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether the property value specified by expression is valid.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>
        ///   <c>true</c> if the property value specified by expression is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValid<TModel, TValue>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            if (htmlHelper == null) throw new ArgumentNullException(nameof(htmlHelper));
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            var expresionProvider = htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
            var expressionAsString = expresionProvider.GetExpressionText(expression);
            
            string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionAsString);
            if (!htmlHelper.ViewData.ModelState.ContainsKey(fullHtmlFieldName))
                return true;
            var modelState = htmlHelper.ViewData.ModelState[fullHtmlFieldName];
            ModelErrorCollection modelErrorCollection = modelState == null ? null : modelState.Errors;
            ModelError error = modelErrorCollection == null || modelErrorCollection.Count == 0
                                   ? null
                                   : modelErrorCollection.FirstOrDefault(m => !string.IsNullOrEmpty(m.ErrorMessage)) ??
                                     modelErrorCollection[0];
            if (error == null)
                return true;
            return false;
        }
    }
}
