using System;
using System.Collections;
using Castle.Windsor;
using vlko.model.Repository;

namespace vlko.core.InversionOfControl
{
	/// <summary>
	/// The Inversion of Control factory.
	/// </summary>
	public static class IoC
	{
		private static IWindsorContainer _container;
		/// <summary>
		/// The IoC container.
		/// </summary>
		/// <value>The container.</value>
		public static IWindsorContainer Container
		{
			get
			{
				if (_container == null)
				{
					Exception ex = new ApplicationException("IoC container not initialized, please call IoC.InitializeWith(IWindsorContainer) before using this library!");
					NLog.LogManager.GetCurrentClassLogger().FatalException(ex.Message, ex);
					throw ex;
				}
				return _container;
			}
			set
			{
				_container = value;
			}
		}

		/// <summary>
		/// Initialize this static instance the with specified container.
		/// </summary>
		/// <param name="container">The container.</param>
		public static void InitializeWith(IWindsorContainer container)
		{
			NLog.LogManager.GetCurrentClassLogger().Info("Initializing IoC...");
			_container = container;
			// Initialize repository IoC resolver
			RepositoryFactory.IntitializeWith(new RepositoryFactoryResolver());
		}

		/// <summary>
		/// Returns a component instance by the service
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>Returns a component instance by the service</returns>
		public static T Resolve<T>()
		{
			return Container.Resolve<T>();
		}

		/// <summary>
		/// Returns a component instance by the service.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="arguments">The arguments.</param>
		/// <returns>Returns a component instance by the service.</returns>
		public static T Resolve<T>(IDictionary arguments)
		{
			return Container.Resolve<T>(arguments);
		}

		/// <summary>
		/// Resolves the specified arguments as anonymous type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <returns>Returns a component instance by the service.</returns>
		public static T Resolve<T>(object argumentsAsAnonymousType)
		{
			return Container.Resolve<T>(argumentsAsAnonymousType);
		}

		/// <summary>
		/// Returns a component instance by the key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <returns>Returns a component instance by the key.</returns>
		public static T Resolve<T>(string key)
		{
			return Container.Resolve<T>(key);
		}

		/// <summary>
		/// Returns a component instance by the key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="arguments">The arguments.</param>
		/// <returns>Returns a component instance by the key.</returns>
		public static T Resolve<T>(string key, IDictionary arguments)
		{
			return Container.Resolve<T>(key, arguments);
		}

		/// <summary>
		/// Resolves the specified arguments as anonymous type for key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <returns>Returns a component instance by the key.</returns>
		public static T Resolve<T>(string key, object argumentsAsAnonymousType)
		{
			return Container.Resolve<T>(key, argumentsAsAnonymousType);
		}
	}
}


