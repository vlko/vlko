using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

public static class ModelMetadataExtensions
{
	public static ModelMetadata GetMetadata<TModel>(this TModel model)
	{
		return ModelMetadataProviders.Current.GetMetadataForType(() => model, typeof(TModel));
	}

	public static ModelMetadata GetMetadata<TModel, TPropertyType>(this TModel model, Expression<Func<TModel, TPropertyType>> propertySelector)
	{
		var propertyName = ((MemberExpression)propertySelector.Body).Member.Name;
		return model.GetMetadata().Properties.Where(p => p.PropertyName == propertyName).First();
	}

	public static ModelMetadata GetMetadataForType<TModel>(this ModelMetadata model)
	{
		return ModelMetadataProviders.Current.GetMetadataForType(null, typeof(TModel));
	}

	public static ModelMetadata GetMetadataForType<TModel, TPropertyType>(this ModelMetadata model, Expression<Func<TModel, TPropertyType>> propertySelector)
	{
		var propertyName = ((MemberExpression)propertySelector.Body).Member.Name;
		return model.GetMetadataForType<TModel>().Properties.Where(p => p.PropertyName == propertyName).First();
	}

	public static bool IsValid(this ModelMetadata model, ControllerContext controllerContext)
	{

		foreach (ModelMetadata propertyMetadata in model.Properties)
		{
			foreach (ModelValidator propertyValidator in propertyMetadata.GetValidators(controllerContext))
			{
				foreach (ModelValidationResult propertyResult in propertyValidator.Validate(model.Model))
				{
					return false;
				}
			}
		}

		foreach (ModelValidator typeValidator in model.GetValidators(controllerContext))
			foreach (ModelValidationResult typeResult in typeValidator.Validate(model.Model))
				return false;

		return true;
	}
}