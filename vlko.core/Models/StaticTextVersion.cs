using System;
using System.Collections.Generic;
using Castle.ActiveRecord;

namespace vlko.core.Models
{
    [ActiveRecord]
    public class StaticTextVersion
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [PrimaryKey(PrimaryKeyType.GuidComb)]
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [Property]
        public virtual int Version { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [Property]
        public virtual string Text { get; set; }

        /// <summary>
        /// Gets or sets the creator.
        /// </summary>
        /// <value>The creator.</value>
        [BelongsTo("CreatorId")]
        public virtual User CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        [Property]
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the static text.
        /// </summary>
        /// <value>The static text.</value>
        [BelongsTo("StaticTextId")]
        public virtual StaticText StaticText { get; set; }
    }
}


