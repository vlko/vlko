using System;

namespace vlko.model.ViewModel
{
	public class CommentForAdminViewModel : CommentViewModel
	{
		/// <summary>
		/// Gets or sets the content id.
		/// </summary>
		/// <value>The content id.</value>
		public Guid ContentId { get; set; }

		/// <summary>
		/// Gets or sets the type of the content.
		/// </summary>
		/// <value>The type of the content.</value>
		public ContentType ContentType { get; set; }
	}
}