using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lamar;
using Microsoft.Extensions.DependencyInjection;

namespace vlko.core.InversionOfControl
{
    public class IoCScope
    {
        public readonly string _ident;
        public IoCScope(string ident)
        {
            _ident = ident;
        }

        private readonly List<Assembly> _catalogAssemblies = new List<Assembly> { Assembly.GetAssembly(typeof(IoC)) };
        private readonly IDictionary<Type, Lazy<object>> _reroutings = new ConcurrentDictionary<Type, Lazy<object>>();
        private Container _container;

        /// <summary>
        /// The IoC container.
        /// </summary>
        /// <value>The container.</value>
        public Container Container
        {
            get
            {
                if (_container == null)
                {
                    lock (this)
                    {
                        if (_container == null)
                        {
                            Initialize();
                        }
                    }
                }
                return _container;
            }
            set
            {
                _container = value;
            }
        }

        /// <summary>
        /// Adds the catalog assembly.
        /// </summary>
        /// <param name="catalogAssembly">The catalog assembly.</param>
        public void AddCatalogAssembly(Assembly catalogAssembly)
        {
            if (!_catalogAssemblies.Contains(catalogAssembly))
            {
                _catalogAssemblies.Add(catalogAssembly);
                _container = null;
            }
        }



        /// <summary>
        /// Initialize this static instance.
        /// </summary>
        public void Initialize()
        {
            NLog.LogManager.GetCurrentClassLogger().Info($"Initializing IoC [{_ident ?? "default"}]...");
            _container = new Container(registry =>
            {
                registry.Scan(x =>
                    {
                        foreach (var assembly in _catalogAssemblies.Select(asmbl => asmbl).Reverse())
                        {
                            x.Assembly(assembly);
                        }
                        x.WithDefaultConventions();
                    });
                foreach (var rerouting in _reroutings)
                {
                    registry.AddSingleton(rerouting.Key, rerouting.Value.Value);
                }
            });
        }

        public void InitializeFromServices(ServiceRegistry services)
        {
            NLog.LogManager.GetCurrentClassLogger().Info($"Initializing IoC [{_ident ?? "default"}] from services...");
            services.Scan(x =>
            {
                x.TheCallingAssembly();
                foreach (var assembly in _catalogAssemblies.Select(asmbl => asmbl).Reverse())
                {
                    x.Assembly(assembly);
                }
                x.WithDefaultConventions();
            });
            _container = new Container(services);
        }

        /// <summary>
        /// Adds the rerouting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        public void AddRerouting<T>(Func<T> value)
        {
            _reroutings[typeof(T)] = new Lazy<object>(() => value());
        }

        /// <summary>
        /// Clears the reroutings.
        /// </summary>
        public void ClearReroutings()
        {
            _reroutings.Clear();
        }

        /// <summary>
        /// Returns a component instance by the service
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Returns a component instance by the service</returns>
        public T Resolve<T>()
        {
            if (_reroutings.ContainsKey(typeof(T)))
            {
                return (T)_reroutings[typeof(T)].Value;
            }
            return Container.GetInstance<T>();
        }

        /// <summary>
        /// Returns a component instance by the key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns>Returns a component instance by the key.</returns>
        public T Resolve<T>(string key)
        {
            return Container.GetInstance<T>(key);
        }

        /// <summary>
        /// Resolves all implementations.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>All instances registered for specified type.</returns>
        public IEnumerable<T> ResolveAllInstances<T>()
        {
            return Container.GetAllInstances<T>();
        }
    }
}
