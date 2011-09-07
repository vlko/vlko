using System;

namespace vlko.BlogModule.Commands.CRUDModel
{
	public class RssItemCRUDModel
	{
		/// <summary>
		/// Gets or sets the feed item id.
		/// </summary>
		/// <value>The feed item id.</value>
		public string FeedItemId { get; set; }

		/// <summary>
		/// Gets or sets the published.
		/// </summary>
		/// <value>The published.</value>
		public DateTime Published { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>The URL.</value>
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		public string Text { get; set; }

		/// <summary>
		/// Gets or sets the author.
		/// </summary>
		/// <value>The author.</value>
		public string Author { get; set; }

		/// <summary>
		/// Gets or sets the feed id.
		/// </summary>
		/// <value>The feed id.</value>
		public Guid FeedId { get; set; }
	}
}
