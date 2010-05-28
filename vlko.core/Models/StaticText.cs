using System;
using System.Collections.Generic;
using Castle.ActiveRecord;

namespace vlko.core.Models
{
    [ActiveRecord]
    public class StaticText : Content
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticText"/> class.
        /// </summary>
        public StaticText()
        {
            ContentType = ContentType.StaticText;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [JoinedKey]
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [Property(Length = 80)]
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the friendly URL.
        /// </summary>
        /// <value>The friendly URL.</value>
        [Property(Unique = true)]
        public virtual string FriendlyUrl { get; set; }

        /// <summary>
        /// Gets or sets the actual version.
        /// </summary>
        /// <value>The actual version.</value>
        [Property]
        public virtual int ActualVersion { get; set; }

        /// <summary>
        /// Gets or sets all static text versions.
        /// </summary>
        /// <value>All static text versions.</value>
        [HasMany(typeof(StaticTextVersion), ColumnKey = "StaticTextId", Cascade = ManyRelationCascadeEnum.AllDeleteOrphan, Lazy = true)]
        public IList<StaticTextVersion> StaticTextVersions { get; set; }
    }
}


