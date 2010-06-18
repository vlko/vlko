using System;

namespace vlko.core.Models.Action.ViewModel
{
	public class StaticTextWithFullTextViewModel : StaticTextViewModel
	{
		public string Text { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StaticTextWithFullTextViewModel"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="friendlyUrl">The friendly URL.</param>
		/// <param name="title">The title.</param>
		/// <param name="description">The description.</param>
		/// <param name="text">The text.</param>
		/// <param name="creator">The creator.</param>
		/// <param name="changeDate">The change date.</param>
		/// <param name="publishDate">The publish date.</param>
		/// <param name="allowComments">if set to <c>true</c> [allow comments].</param>
		/// <param name="commentCounts">The comment counts.</param>
		public StaticTextWithFullTextViewModel(Guid id, string friendlyUrl, string title, string description, string text, User creator, DateTime changeDate, DateTime publishDate, bool allowComments, int commentCounts) : base(id, friendlyUrl, title, description, creator, changeDate, publishDate, allowComments, commentCounts)
		{
			Text = text;
		}
	}
}