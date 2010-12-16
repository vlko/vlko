using System;

namespace vlko.model.Roots
{
	public class RssItem : Content
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RssFeed"/> class.
		/// </summary>
		public RssItem()
		{
			ContentType = ContentType.RssItem;
		}

		/// <summary>
		/// Gets or sets the feed item id.
		/// </summary>
		/// <value>The feed item id.</value>
		public virtual string FeedItemId { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>The URL.</value>
		public virtual string Url { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public virtual string Title { get; set; }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		public virtual string Text { get; set; }

		/// <summary>
		/// Gets or sets the author.
		/// </summary>
		/// <value>The author.</value>
		public virtual string Author { get; set; }

		/// <summary>
		/// Gets or sets the RSS feed.
		/// </summary>
		/// <value>The RSS feed.</value>
		public virtual RssFeed RssFeed { get; set; }
	}
}


