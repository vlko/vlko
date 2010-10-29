using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;

namespace vlko.model
{
	[ActiveRecord]
	public class AppSetting
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[PrimaryKey(PrimaryKeyType.Assigned, Length = 50)]
		public virtual string Name { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		[Property(Length = 255)]
		public virtual string Value { get; set; }
	}
}
