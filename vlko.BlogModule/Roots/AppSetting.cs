using vlko.core.Roots;

namespace vlko.BlogModule.Roots
{
	public class AppSetting : IAppSetting
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public virtual string Id { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public virtual string Value { get; set; }
	}
}
