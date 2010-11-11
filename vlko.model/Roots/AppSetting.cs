using Castle.ActiveRecord;
using vlko.core.Roots;

namespace vlko.model.Roots
{
	[ActiveRecord]
	public class AppSetting : IAppSetting
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
