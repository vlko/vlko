using System;

namespace vlko.BlogModule.Roots
{
	public enum SystemMessageTypeEnum
	{
		Urgent,
		Error,
		Warning
	}

	public class SystemMessage
	{
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		public virtual Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the created date.
		/// </summary>
		/// <value>The created date.</value>
		public virtual DateTime CreatedDate { get; set; }

		/// <summary>
		/// Gets or sets the type of the system message.
		/// </summary>
		/// <value>The type of the system message.</value>
		public virtual SystemMessageTypeEnum SystemMessageType { get; set; }

		/// <summary>
		/// Gets or sets the sender.
		/// </summary>
		/// <value>The sender.</value>
		public virtual string Sender { get; set; }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		public virtual string Text { get; set; }
	}
}
