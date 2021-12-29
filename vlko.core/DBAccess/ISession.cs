using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vlko.core.DBAccess.Querying;
using vlko.core.InversionOfControl;

namespace vlko.core.DBAccess
{
    public interface ISession : IDisposable
    {
        IoCScope Scope { get; }

        /// <summary>
        /// Initializes the specified session with scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        void Init(IoCScope scope);

        /// <summary>
        /// Starts the transaction and return instance maintaining it.
        /// </summary>
        /// <returns></returns>
        ITransaction StartTransaction();

        /// <summary>
        /// Resolve and initialize command.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        TCommand GetCommand<TCommand>() where TCommand : ICommands;

        /// <summary>
        /// Resolve and initialize QueryDef object using internaly session.
        /// </summary>
        IQueryDef<T> GetQueryDef<T>(IQueryable<T> query) where T : class;

        /// <summary>
        /// Loads instance with the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="throwOnNotFound">if set to <c>true</c> [throw on not found].</param>
        /// <returns>Object with specified id or exception if not found (in case throwOnNotFound is not specified).</returns>
        T Load<T>(object id, bool throwOnNotFound = true) where T : class;
        /// <summary>
        /// Loads instance with the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="throwOnNotFound">if set to <c>true</c> [throw on not found].</param>
        /// <returns>Object with specified id or exception if not found (in case throwOnNotFound is not specified).</returns>
        Task<T> LoadAsync<T>(object id, bool throwOnNotFound = true, CancellationToken token = default(CancellationToken)) where T: class;

        /// <summary>
        /// Loads instances with the specified ids.
        /// </summary>
        /// <param name="id">The array of ids.</param>
        /// <returns>Object with specified id (not found items are ignored in result).</returns>
        IDictionary<string, T> LoadMore<T>(params object[] ids) where T : class;
        /// <summary>
        /// Loads instances with the specified ids.
        /// </summary>
        /// <param name="id">The array of ids.</param>
        /// <returns>Object with specified id (not found items are ignored in result).</returns>
        Task<IDictionary<string, T>> LoadMoreAsync<T>(params object[] ids) where T: class;

        /// <summary>
        /// Loads instances with the specified ids.
        /// </summary>
        /// <param name="id">The array of ids.</param>
        /// <returns>Object with specified id (not found items are ignored in result).</returns>
        IDictionary<string, T> LoadMore<T>(IEnumerable<object> ids) where T : class;
        /// <summary>
        /// Loads instances with the specified ids.
        /// </summary>
        /// <param name="id">The array of ids.</param>
        /// <returns>Object with specified id (not found items are ignored in result).</returns>
        Task<IDictionary<string, T>> LoadMoreAsync<T>(IEnumerable<object> ids, CancellationToken token = default(CancellationToken)) where T: class;

        /// <summary>
        /// Deletes the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        void Delete<T>(T instance) where T : class;
        /// <summary>
        /// Deletes the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        Task DeleteAsync<T>(T instance, CancellationToken token = default(CancellationToken)) where T: class;

        /// <summary>
        /// Deletes the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        void Store<T>(T instance) where T : class;
        /// <summary>
        /// Deletes the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        Task StoreAsync<T>(T instance, CancellationToken token = default(CancellationToken)) where T: class;
    }
}
