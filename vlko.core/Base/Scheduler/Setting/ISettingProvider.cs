namespace vlko.core.Base.Setting
{
	public interface ISettingProvider
	{

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <returns>True if there is value, otherwise false;</returns>
		bool GetValue(string name, ref string value);
		/// <summary>
		/// Saves the value for specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		void SaveValue(string name, string value);
	}
}
