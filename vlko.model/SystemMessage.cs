using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;

namespace vlko.model
{
	public enum SystemMessageTypeEnum
	{
		Urgent,
		Error,
		Warning
	}

 	[ActiveRecord]
	public class SystemMessage
	{
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		[PrimaryKey(PrimaryKeyType.GuidComb)]
		public virtual Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the created date.
		/// </summary>
		/// <value>The created date.</value>
		[Property]
		public virtual DateTime CreatedDate { get; set; }

		/// <summary>
		/// Gets or sets the type of the system message.
		/// </summary>
		/// <value>The type of the system message.</value>
		[Property]
		public virtual SystemMessageTypeEnum SystemMessageType { get; set; }

		/// <summary>
		/// Gets or sets the sender.
		/// </summary>
		/// <value>The sender.</value>
		[Property(Length = 255)]
		public virtual string Sender { get; set; }

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		[Property(ColumnType = "StringClob")]
		public virtual string Text { get; set; }
	}
}
