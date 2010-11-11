using System;
using System.ComponentModel.DataAnnotations;
using vlko.core.ValidationAtribute;

namespace vlko.model.Action.CRUDModel
{
	public class RssFeedCRUDModel
	{
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		[Key]
		[Editable(false)]
		public Guid Id { get; set; }

		[Display(ResourceType = typeof(ModelResources), Name = "Name")]
		[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequireError")]
		[StringLength(50, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthError")]
		public string Name { get; set; }

		[Display(ResourceType = typeof(ModelResources), Name = "FeedUrl")]
		[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "RequireError")]
		[StringLength(255, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthError")]
		[AntiXssIgnore]
		public string Url { get; set; }

		[Display(ResourceType = typeof(ModelResources), Name = "AuthorRegex")]
		[StringLength(255, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthError")]
		[AntiXssIgnoreAttribute]
		public string AuthorRegex { get; set; }

		[Display(ResourceType = typeof(ModelResources), Name = "GetDirectContent")]
		public bool GetDirectContent { get; set; }

		[Display(ResourceType = typeof(ModelResources), Name = "DisplayFullContent")]
		public bool DisplayFullContent { get; set; }

		[Display(ResourceType = typeof(ModelResources), Name = "ContentParseRegex")]
		[StringLength(255, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "LengthError")]
		[AntiXssIgnoreAttribute]
		public string ContentParseRegex { get; set; }
	}
}