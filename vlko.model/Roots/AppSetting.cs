using vlko.core.Roots;

namespace vlko.model.Roots
{
	public class AppSetting : IAppSetting
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public virtual string Name { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public virtual string Value { get; set; }
	}
}
