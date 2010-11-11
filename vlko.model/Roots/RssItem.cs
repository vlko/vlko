using System;
using Castle.ActiveRecord;

namespace vlko.model.Roots
{
	[ActiveRecord]
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
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		[JoinedKey]
		public virtual Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the feed item id.
		/// </summary>
		/// <value>The feed item id.</value>
		[Property(Length = 255, Unique = true)]
		public virtual string FeedItemId { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>The URL.</value>
		[Property(Length = 255)]
		public virtual string Url { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		[Property(Length = 255)]
		public virtual string Title { get; set; }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		[Property(ColumnType = "StringClob")]
		public virtual string Text { get; set; }

		/// <summary>
		/// Gets or sets the author.
		/// </summary>
		/// <value>The author.</value>
		[Property(Length = 50)]
		public virtual string Author { get; set; }

		/// <summary>
		/// Gets or sets the RSS feed.
		/// </summary>
		/// <value>The RSS feed.</value>
		[BelongsTo("RssFeedId")]
		public virtual RssFeed RssFeed { get; set; }
	}
}


