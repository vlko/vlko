using System;
using System.ComponentModel.DataAnnotations;
using vlko.core.Roots;
using vlko.core.ValidationAtribute;

namespace vlko.BlogModule.Action.CRUDModel
{
	public class StaticTextCRUDModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StaticTextCRUDModel"/> class.
		/// </summary>
		public StaticTextCRUDModel()
		{
		}

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		[Key]
		[Editable(false)]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the friendly URL (if new then auto-generated).
		/// </summary>
		/// <value>The friendly URL.</value>
		[Display(ResourceType = typeof(ModelResources), Name = "FriendlyUrl")]
		[StringLength(80, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "FriendlyURLError")]
		public string FriendlyUrl { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		[Display(ResourceType = typeof(ModelResources), Name = "Title")]
		[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TitleRequireError")]
		[StringLength(80, ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TitleLengthError")]
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		[Display(ResourceType = typeof(ModelResources), Name = "Text")]
		[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "TextRequireError")]
		[DataType(DataType.Html)]
		[AntiXssHtmlText]
		public string Text { get; set; }

		[Display(ResourceType = typeof(ModelResources), Name = "Description")]
		[Editable(false)]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the creator.
		/// </summary>
		/// <value>The creator.</value>
		[Editable(false)]
		public IUser Creator { get; set; }

		/// <summary>
		/// Gets or sets the current date.
		/// </summary>
		/// <value>The current date.</value>
		[Editable(false)]
		[Display(ResourceType = typeof(ModelResources), Name = "ChangeDate")]
		public DateTime ChangeDate { get; set; }

		/// <summary>
		/// Gets or sets the publish date.
		/// </summary>
		/// <value>The publish date.</value>
		[Display(ResourceType = typeof(ModelResources), Name = "PublishDate")]
		[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "PublishDateRequireError")]
		[DataType(DataType.Date)]
		public DateTime PublishDate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [allow comments].
		/// </summary>
		/// <value><c>true</c> if [allow comments]; otherwise, <c>false</c>.</value>
		[Display(ResourceType = typeof(ModelResources), Name = "AllowComments")]
		public bool AllowComments { get; set; }
	}
}
