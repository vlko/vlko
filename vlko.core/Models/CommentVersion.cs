using System;
using Castle.ActiveRecord;

namespace vlko.core.Models
{
    [ActiveRecord]
    public class CommentVersion
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [PrimaryKey(PrimaryKeyType.GuidComb)]
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [Property]
        public virtual DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [Property]
        public virtual int Version { get; set; }

        /// <summary>
        /// Gets or sets the client ip.
        /// </summary>
        /// <value>The client ip.</value>
        [Property(Length = 50)]
        public virtual string ClientIp { get; set; }

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        /// <value>The user agent.</value>
        [Property(Length = 255)]
        public virtual string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [Property(ColumnType = "StringClob")]
        public virtual string Text { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>The comment.</value>
        [BelongsTo("CommentId")]
        public virtual Comment Comment { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>The created by.</value>
        [BelongsTo("CreatedById")]
        public virtual User CreatedBy { get; set; }
    }
}


