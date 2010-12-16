using System;
using System.Collections.Generic;

namespace vlko.model.Roots
{
	public class Content
	{
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		public virtual Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the type of the content.
		/// </summary>
		/// <value>The type of the content.</value>
		public virtual ContentType ContentType { get; set; }

		/// <summary>
		/// Gets or sets the created.
		/// </summary>
		/// <value>The created.</value>
		public virtual DateTime CreatedDate { get; set; }

		/// <summary>
		/// Gets or sets the modified.
		/// </summary>
		/// <value>The modified.</value>
		public virtual DateTime Modified { get; set; }

		/// <summary>
		/// Gets or sets the publish date.
		/// </summary>
		/// <value>The publish date.</value>
		public virtual DateTime PublishDate { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [are comment allowed].
		/// </summary>
		/// <value><c>true</c> if [are comment allowed]; otherwise, <c>false</c>.</value>
		public virtual bool AreCommentAllowed { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Content"/> is deleted.
		/// </summary>
		/// <value><c>true</c> if hidden; otherwise, <c>false</c>.</value>
		public virtual bool Hidden { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public virtual string Description { get; set; }

		/// <summary>
		/// Gets or sets the created by.
		/// </summary>
		/// <value>The created by.</value>
		public virtual User CreatedBy { get; set; }

		/// <summary>
		/// Gets or sets the comments.
		/// </summary>
		/// <value>The comments.</value>
		public virtual IList<Comment> Comments { get; set; }


	}
}