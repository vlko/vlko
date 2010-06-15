using System;

namespace vlko.core.Models.Action.ViewModel
{
	public class CommentForAdminViewModel : CommentViewModel
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="CommentForAdminViewModel"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="contentId">The content id.</param>
		/// <param name="contentType">Type of the content.</param>
		/// <param name="name">The name.</param>
		/// <param name="createdDate">The created date.</param>
		/// <param name="text">The text.</param>
		/// <param name="version">The version.</param>
		/// <param name="owner">The owner.</param>
		/// <param name="anonymousName">Name of the anonymous.</param>
		/// <param name="clientIp">The client ip.</param>
		/// <param name="level">The level.</param>
		public CommentForAdminViewModel(Guid id, Guid contentId, ContentType contentType, string name, DateTime createdDate, string text, int version, User owner, string anonymousName, string clientIp, int level) 
			: base(id, name, createdDate, text, version, owner, anonymousName, clientIp, level)
		{
			ContentId = contentId;
			ContentType = contentType;
		}

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