using System;
using System.Collections.Generic;
using Castle.ActiveRecord;

namespace vlko.core.Models
{
    [ActiveRecord]
    public class RssFeed : Content
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RssFeed"/> class.
        /// </summary>
        public RssFeed()
        {
            ContentType = ContentType.RssFeed;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [JoinedKey]
        public virtual Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the RSS items.
        /// </summary>
        /// <value>The RSS items.</value>
        [HasMany(typeof(RssItem), ColumnKey = "RssFeedId", Cascade = ManyRelationCascadeEnum.AllDeleteOrphan, Lazy = true, Inverse = true)]
        public virtual IList<RssItem> RssItems { get; set; }

    }
}


