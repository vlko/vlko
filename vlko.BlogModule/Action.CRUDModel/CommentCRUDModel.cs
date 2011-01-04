using System;
using System.ComponentModel.DataAnnotations;
using vlko.core.Roots;
using vlko.core.ValidationAtribute;

namespace vlko.BlogModule.Action.CRUDModel
{
	public class CommentCRUDModel
	{
		public readonly string ExpressionText = "23 + 6 =";
		public readonly string ExpressionCorrectValue = "29";
		/// <summary>
		/// Gets or sets the Id.
		/// </summary>
		/// <value>The Id.</value>
		[Key]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the content id.
		/// </summary>
		/// <value>The content id.</value>
		public Guid ContentId { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CommentNameRequireError")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		[DataType(DataType.Html)]
		[AntiXssHtmlText]
		[Required(ErrorMessageResourceType = typeof(ModelResources), ErrorMessageResourceName = "CommentTextRequireError")]
		public string Text { get; set; }

		/// <summary>
		/// Gets or sets the robo check value.
		/// </summary>
		/// <value>The robo check value.</value>
		public string RoboCheck { get; set; }

		/// <summary>
		/// Gets or sets the change date.
		/// </summary>
		/// <value>The change date.</value>
		public DateTime ChangeDate { get; set; }

		/// <summary>
		/// Gets or sets the parent id.
		/// </summary>
		/// <value>The parent id.</value>
		public Guid? ParentId { get; set; }

		/// <summary>
		/// Gets or sets the change user.
		/// </summary>
		/// <value>The change user.</value>
		public IUser ChangeUser { get; set; }

		/// <summary>
		/// Gets or sets the name of the anonymous.
		/// </summary>
		/// <value>The name of the anonymous.</value>
		public string AnonymousName { get; set; }

		/// <summary>
		/// Gets or sets the client ip.
		/// </summary>
		/// <value>The client ip.</value>
		public string ClientIp { get; set; }

		/// <summary>
		/// Gets or sets the user agent.
		/// </summary>
		/// <value>The user agent.</value>
		public string UserAgent { get; set; }
	}
}
