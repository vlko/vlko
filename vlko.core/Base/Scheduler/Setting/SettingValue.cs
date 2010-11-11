using System.Collections.Generic;
using System.Threading;

namespace vlko.core.Base.Scheduler.Setting
{
	public class SettingValue<T>
	{
		private static readonly ReaderWriterLockSlim CacheLock = new ReaderWriterLockSlim();
		private static readonly IDictionary<string, T> Cache = new Dictionary<string, T>();


		private readonly string _name;
		private readonly T _defaultValue;
		private readonly ISettingProvider _settingProvider;

		/// <summary>
		/// Gets the Name.
		/// </summary>
		/// <value>The Name.</value>
		public string Name
		{
			get 
			{
				return _name;
			}
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public T Value
		{
			get
			{
				return LoadValue();
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SettingValue&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="name">The Name.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <param name="settingProvider">The setting provider.</param>
		public SettingValue(string name, T defaultValue, ISettingProvider settingProvider )
		{
			_name = name;
			_defaultValue = defaultValue;
			_settingProvider = settingProvider;
		}

		/// <summary>
		/// Loads the value.
		/// </summary>
		private T LoadValue()
		{
			T value = _defaultValue;
			bool loadedFromCache = false;

			using (CacheLock.ReadLock())
			{
				if (Cache.ContainsKey(Name))
				{
					value = Cache[Name];
					loadedFromCache = true;
				}
			}

			if (!loadedFromCache)
			{
				using (CacheLock.WriteLock())
				{
					if (!Cache.ContainsKey(Name))
					{
						string rawValue = null;
						if (_settingProvider.GetValue(Name, ref rawValue))
						{
							value = SettingValueConverter.ConvertToValue<T>(rawValue);
						}
						else
						{
							Cache[Name] = _defaultValue;
						}
					}
					else
					{
						value = Cache[Name];
					}
				}
			}

			return value;
		}

		/// <summary>
		/// Saves the value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void SaveValue(T value)
		{
			using (CacheLock.WriteLock())
			{
				Cache[Name] = value;
				string rawValue = SettingValueConverter.ConvertToString(value);
				_settingProvider.SaveValue(Name, rawValue);
			}
		}
	}
}
