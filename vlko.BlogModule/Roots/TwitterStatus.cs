namespace vlko.BlogModule.Roots
{
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
		/// Gets or sets the twitter id.
		/// </summary>
		/// <value>The twitter id.</value>
		public virtual long TwitterId { get; set; }

		/// <summary>
		/// Gets or sets the user.
		/// </summary>
		/// <value>The user.</value>
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
		public virtual bool Reply { get; set; }

		/// <summary>
		/// Gets or sets the retweet user.
		/// </summary>
		/// <value>The retweet user.</value>
		public virtual string RetweetUser { get; set; }
	}
}
