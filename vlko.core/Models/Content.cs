using System;
using System.Collections.Generic;
using Castle.ActiveRecord;

namespace vlko.core.Models
{
	[ActiveRecord, JoinedBase]
	public class Content
	{
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		[PrimaryKey(PrimaryKeyType.GuidComb)]
		public virtual Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the type of the content.
		/// </summary>
		/// <value>The type of the content.</value>
		[Property]
		public virtual ContentType ContentType { get; set; }

		/// <summary>
		/// Gets or sets the created.
		/// </summary>
		/// <value>The created.</value>
		[Property]
		public virtual DateTime CreatedDate { get; set; }

		/// <summary>
		/// Gets or sets the modified.
		/// </summary>
		/// <value>The modified.</value>
		[Property]
		public virtual DateTime Modified { get; set; }

		/// <summary>
		/// Gets or sets the publish date.
		/// </summary>
		/// <value>The publish date.</value>
		[Property]
		public virtual DateTime PublishDate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [are comment allowed].
		/// </summary>
		/// <value><c>true</c> if [are comment allowed]; otherwise, <c>false</c>.</value>
		[Property]
		public virtual bool AreCommentAllowed { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Content"/> is deleted.
		/// </summary>
		/// <value><c>true</c> if deleted; otherwise, <c>false</c>.</value>
		[Property]
		public virtual bool Deleted { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		[Property(Length = ModelConstants.DescriptionMaxLenghtConst)]
		public virtual string Description { get; set; }

		/// <summary>
		/// Gets or sets the created by.
		/// </summary>
		/// <value>The created by.</value>
		[BelongsTo("CreatedById")]
		public virtual User CreatedBy { get; set; }



		/// <summary>
		/// Gets or sets the comments.
		/// </summary>
		/// <value>The comments.</value>
		[HasMany(typeof(Comment), ColumnKey = "ContentId", Cascade = ManyRelationCascadeEnum.AllDeleteOrphan, Lazy = true, Inverse = true)]
		public IList<Comment> Comments { get; set; }


	}
}