using System;
using Castle.ActiveRecord;

namespace vlko.core.Models
{
    [ActiveRecord]
    public class RssItem
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
		[PrimaryKey(PrimaryKeyType.GuidComb)]
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        [Property(Length = 255)]
        public virtual string Url { get; set; }

        /// <summary>
        /// Gets or sets the published at.
        /// </summary>
        /// <value>The published at.</value>
        [Property]
        public virtual DateTime PublishedAt { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [Property(Length = 255)]
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the preview.
        /// </summary>
        /// <value>The preview.</value>
        [Property(Length = 500)]
        public virtual string Preview { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [Property(ColumnType = "StringClob")]
        public virtual string Text { get; set; }

        /// <summary>
        /// Gets or sets the RSS feed.
        /// </summary>
        /// <value>The RSS feed.</value>
        [BelongsTo("RssFeedId")]
        public virtual RssFeed RssFeed { get; set; }
    }
}


