using System;
using System.Collections.Generic;
using Castle.ActiveRecord;

namespace vlko.model
{
    [ActiveRecord]
    public class Comment
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [PrimaryKey(PrimaryKeyType.GuidComb)]
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Property(Length = 255)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the actual version.
        /// </summary>
        /// <value>The actual version.</value>
        [Property]
        public virtual int ActualVersion { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        [Property]
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>The level.</value>
        [Property]
        public virtual int Level { get; set; }

        /// <summary>
        /// Gets or sets the parent version.
        /// </summary>
        /// <value>The parent version.</value>
        [Property]
        public virtual int ParentVersion { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        [BelongsTo("ContentId", Lazy = FetchWhen.OnInvoke)]
        public virtual Content Content { get; set; }

        /// <summary>
        /// Gets or sets the parent comment.
        /// </summary>
        /// <value>The parent comment.</value>
        [BelongsTo("ParentCommentId", Lazy = FetchWhen.OnInvoke)]
        public virtual Comment ParentComment { get; set; }

        /// <summary>
        /// Gets or sets the topmost comment in thread (can be self).
        /// </summary>
        /// <value>The topmost comment in thread (can be self).</value>
        [BelongsTo("TopCommentId", Lazy = FetchWhen.OnInvoke)]
        public virtual Comment TopComment { get; set; }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
        [BelongsTo("OwnerId")]
        public virtual User Owner { get; set; }

        /// <summary>
        /// Gets or sets the name of the anonymous user.
        /// </summary>
        /// <value>The name of the anonymous user.</value>
        [Property]
        public virtual string AnonymousName { get; set; }

        /// <summary>
        /// Gets or sets the comment versions.
        /// </summary>
        /// <value>The comment versions.</value>
        [HasMany(typeof(CommentVersion), ColumnKey = "CommentId", Cascade = ManyRelationCascadeEnum.AllDeleteOrphan, Lazy = true)]
        public IList<CommentVersion> CommentVersions { get; set; }

    }
}


