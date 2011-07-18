using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Linq;
using vlko.core.Repository;

namespace vlko.core.InversionOfControl
{
	/// <summary>
	/// The Inversion of Control factory.
	/// </summary>
	public static class IoC
	{
		private static readonly List<Assembly> CatalogAssemblies = new List<Assembly>
		                                                     	{
		                                                     		Assembly.GetAssembly(typeof(IoC))
		                                                     	};

		private static readonly IDictionary<Type, Lazy<object>> Reroutings = new ConcurrentDictionary<Type, Lazy<object>>();

		/// <summary>
		/// Adds the rerouting.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The value.</param>
		public static void AddRerouting<T>(Lazy<object> value)
		{
			Reroutings.Add(typeof(T), value);
		}

		/// <summary>
		/// Adds the rerouting.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		public static void AddRerouting(Type type, Lazy<object> value)
		{
			Reroutings.Add(type, value);
		}

		/// <summary>
		/// Clears the reroutings.
		/// </summary>
		public static void ClearReroutings()
		{
			Reroutings.Clear();
		}

		/// <summary>
		/// Adds the catalog assembly.
		/// </summary>
		/// <param name="catalogAssembly">The catalog assembly.</param>
		public static void AddCatalogAssembly(Assembly catalogAssembly)
		{
			if (!CatalogAssemblies.Contains(catalogAssembly))
			{
				CatalogAssemblies.Add(catalogAssembly);
				_container = null;
			}
		}

		private static CompositionContainer _container;
		/// <summary>
		/// The IoC container.
		/// </summary>
		/// <value>The container.</value>
		public static CompositionContainer Container
		{
			get
			{
				lock (typeof(IoC))
				{
					if (_container == null)
					{
						Initialize();
					}
					return _container;
				}
			}
			set
			{
				_container = value;
			}
		}

		/// <summary>
		/// Initialize this static instance.
		/// </summary>
		public static void Initialize()
		{
			NLog.LogManager.GetCurrentClassLogger().Info("Initializing IoC...");
			var catalog = new AggregateCatalog(
					CatalogAssemblies.Select(assembly => new AssemblyCatalog(assembly))
					);
			_container = new CompositionContainer(catalog, true, null);
		}

		/// <summary>
		/// Returns a component instance by the service
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>Returns a component instance by the service</returns>
		public static T Resolve<T>()
		{
			if (Reroutings.ContainsKey(typeof(T)))
			{
				return (T) Reroutings[typeof (T)].Value;
			}
			return Container.GetExportedValue<T>();
		}

		/// <summary>
		/// Returns a component instance by the key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <returns>Returns a component instance by the key.</returns>
		public static T Resolve<T>(string key)
		{
			return Container.GetExportedValue<T>(key);
		}
	}
}


