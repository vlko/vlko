using System;

namespace vlko.core.Models.Action.ViewModel
{
	public class CommentSearchViewModel : CommentViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CommentSearchViewModel"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="name">The name.</param>
		/// <param name="createdDate">The created date.</param>
		/// <param name="text">The text.</param>
		/// <param name="version">The version.</param>
		/// <param name="owner">The owner.</param>
		/// <param name="anonymousName">Name of the anonymous.</param>
		/// <param name="clientIp">The client ip.</param>
		/// <param name="level">The level.</param>
		/// <param name="friendlyUrl">The friendly URL.</param>
		public CommentSearchViewModel(Guid id, string name, DateTime createdDate, string text, int version, User owner, string anonymousName, string clientIp, int level, Content content) 
			: base(id, name, createdDate, text, version, owner, anonymousName, clientIp, level)
		{
			Content = content;
		}

		/// <summary>
		/// Gets or sets the content.
		/// </summary>
		/// <value>The content.</value>
		public Content Content { get; set; }
	}
}