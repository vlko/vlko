using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;

namespace vlko.model
{
	[ActiveRecord]
	public class TwitterStatus : Content
	{
		/// <summary>
		/// Statics the text.
		/// </summary>
		public TwitterStatus()
		{
			ContentType = ContentType.TwitterStatus;
		}

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		[JoinedKey]
		public virtual Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the twitter id.
		/// </summary>
		/// <value>The twitter id.</value>
		[Property(Unique = true)]
		public virtual long TwitterId { get; set; }

		/// <summary>
		/// Gets or sets the user.
		/// </summary>
		/// <value>The user.</value>
		[Property(Length = 255)]
		public virtual string User { get; set; }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		public virtual string Text
		{
			get { return Description; }
			set { Description = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="TwitterStatus"/> is reply.
		/// </summary>
		/// <value><c>true</c> if reply; otherwise, <c>false</c>.</value>
		[Property]
		public virtual bool Reply { get; set; }

		/// <summary>
		/// Gets or sets the retweet user.
		/// </summary>
		/// <value>The retweet user.</value>
		[Property]
		public virtual string RetweetUser { get; set; }
	}
}
