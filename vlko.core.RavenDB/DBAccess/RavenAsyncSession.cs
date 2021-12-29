using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Operations.Identities;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using vlko.core.DBAccess;
using vlko.core.DBAccess.Exceptions;
using vlko.core.DBAccess.Querying;
using vlko.core.InversionOfControl;
using vlko.core.RavenDB.DBAccess.Querying;
using vlko.core.Tools;

namespace vlko.core.RavenDB.DBAccess
{
    public class RavenAsyncSession : ISession
    {
        private readonly IAsyncDocumentSession _session;
        private RavenAsyncTransaction _currentTransaction;

        public IAsyncAdvancedSessionOperations Advanced { get => _session.Advanced; }

        internal RavenAsyncSession(IAsyncDocumentSession session, Guid sessionIdent)
        {
            _session = session;
            SessionIdent = sessionIdent;
        }

        /// <summary>
        /// Resolve scope for this session.
        /// </summary>
        public IoCScope Scope { get; private set; }
        /// <summary>
        /// Gets the session ident to identify session.
        /// </summary>
        public Guid SessionIdent { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has active transaction.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has active transaction; otherwise, <c>false</c>.
        /// </value>
        public bool HasActiveTransaction { get => _currentTransaction != null; }

        /// <summary>
        /// Initializes the specified session with scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        public void Init(IoCScope scope)
        {
            Scope = scope;
        }


        /// <summary>
        /// Resolve and initialize command.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        public TCommand GetCommand<TCommand>() where TCommand : ICommands
        {
            var command = Scope.Resolve<TCommand>();
            if (command == null)
            {
                new RavenDBAccessException($"Command of type '{typeof(TCommand).Name}' was not resolved by current session scope!!");
            }
            if (!(command is ICommands<RavenAsyncSession>))
            {
                new RavenDBAccessException($"Command of type '{typeof(TCommand).Name}' does not implement ICommand<RavenSession> interface!!");
            }
            (command as ICommands<RavenAsyncSession>).Init(this);
            return command;
        }

        /// <summary>
        /// Resolve and initialize QueryDef object using internaly session.
        /// </summary>
        public IQueryDef<T> GetQueryDef<T>(IQueryable<T> query) where T : class => new QueryDefAsync<T>(query, this);

        /// <summary>
        /// Starts the transaction and return instance maintaining it.
        /// </summary>
        /// <returns>Transaction object, the only way to store changes.</returns>
        public ITransaction StartTransaction()
        {
            if (_currentTransaction != null)
            {
                _currentTransaction = null;
                throw new RavenDBAccessException($"There is already active transaction in this session [{SessionIdent}]!");
            }
            _currentTransaction = new RavenAsyncTransaction(this);
            //rollback all previously queued changes
            RollBackTransaction();

            return _currentTransaction;
        }

        internal async Task CommitTransaction()
        {
            await _session.SaveChangesAsync();
        }

        internal void RollBackTransaction()
        {
            _session.Advanced.Clear();
        }

        internal void DisposeTransaction()
        {
            _currentTransaction = null;
        }

        /// <summary>
        /// Create query to query by specified document.
        /// </summary>
        /// <typeparam name="TIndexType">The type of the index.</typeparam>
        /// <typeparam name="T">Type of document.</typeparam>
        public IRavenQueryable<T> Query<TIndexType, T>() where TIndexType : AbstractIndexCreationTask, new() where T : class
        {
            return _session.Query<T, TIndexType>();
        }
        /// <summary>
        /// Create query to query by with projection result.
        /// </summary>
        /// <typeparam name="TIndexType">The type of the index.</typeparam>
        /// <typeparam name="TProjection">The type of the projection.</typeparam>
        public IRavenQueryable<TProjection> QueryWithProjection<TIndexType, TProjection>() where TIndexType : AbstractIndexCreationTask, new() where TProjection : class
        {
            return _session.Query<TProjection, TIndexType>()
                .ProjectInto<TProjection>();
        }

        /// <summary>
        /// Create query to query by with projection result only from index fields.
        /// </summary>
        /// <typeparam name="TIndexType">The type of the index.</typeparam>
        /// <typeparam name="TProjection">The type of the projection.</typeparam>
        public IRavenQueryable<TProjection> QueryFromIndexFields<TIndexType, TProjection>() where TIndexType : AbstractIndexCreationTask, new() where TProjection : class
        {
            return _session.Query<TProjection, TIndexType>()
#if FAILPROJECTION
                .Customize(x => x.Projection(ProjectionBehavior.FromIndexOrThrow))
#else
                .Customize(x => x.Projection(ProjectionBehavior.FromIndex))
#endif
                .ProjectInto<TProjection>();
        }

        /// <summary>
        /// Create document query (one using direct lucene syntax) to query by specified document.
        /// </summary>
        /// <typeparam name="TIndexType">The type of the index.</typeparam>
        /// <typeparam name="T">Type of document.</typeparam>
        public IAsyncDocumentQuery<T> DocumentQuery<TIndexType, T>() where TIndexType : AbstractIndexCreationTask, new() where T : class
        {
            return _session.Advanced.AsyncDocumentQuery<T, TIndexType>();
        }

        /// <summary>
        /// Create document query (one using direct lucene syntax)  to query by with projection result.
        /// </summary>
        /// <typeparam name="TIndexType">The type of the index.</typeparam>
        /// <typeparam name="TProjection">The type of the projection.</typeparam>
        public IAsyncDocumentQuery<TProjection> DocumentQueryWithProjection<TIndexType, TProjection>() where TIndexType : AbstractIndexCreationTask, new() where TProjection : class
        {
            return _session.Advanced.AsyncDocumentQuery<TProjection, TIndexType>().SelectFields<TProjection>(ProjectionBehavior.FromIndex);
        }

        /// <summary>
        /// Loads instance with the specified id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The id.</param>
        /// <param name="throwOnNotFound">if set to <c>true</c> [throw on not found].</param>
        /// <param name="token"></param>
        /// <returns>
        /// Object with specified id or exception if not found (in case throwOnNotFound is not specified).
        /// </returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task<T> LoadAsync<T>(object id, bool throwOnNotFound = true, CancellationToken token = default) where T : class
        {
            T loaded = await _session.LoadAsync<T>(ConvertToStringId<T>(id), token).ConfigureAwait(false);
            if (throwOnNotFound && loaded == null)
            {
                throw new NotFoundException(typeof(T), id, string.Empty);
            }
            return loaded;
        }

        /// <summary>
        /// Lazily loads instance with the specified id (will not throw error if not exists, but will return null).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">The id.</param>
        /// <param name="token"></param>
        /// <returns>
        /// Object with specified id or null if not found.
        /// </returns>
        public Lazy<Task<T>> LazilyLoadAsync<T>(object id, CancellationToken token = default)
        {
            return _session.Advanced.Lazily.LoadAsync<T>(ConvertToStringId<T>(id), token);
        }

        /// <summary>
        /// Loads instances with the specified ids.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns>
        /// Object with specified id (not found items are ignored in result).
        /// </returns>
        public async Task<IDictionary<string, T>> LoadMoreAsync<T>(params object[] ids) where T : class
        {
            return await _session.LoadAsync<T>(ids.Select(x => ConvertToStringId<T>(x))).ConfigureAwait(false);
        }


        public async Task<TProjection[]> LoadMoreWithTransformerAsync<T, TTransformer, TProjection>(params object[] ids) where TTransformer : AbstractTransformer<T, TProjection>, new() where T : class
        {
            var idents = ids.Select(x => ConvertToStringId<T>(x)).ToArray();
            var basicQuery = _session.Query<T>()
                .Customize(customization => customization.BeforeQueryExecuted(query =>
                {
                    // regex for query pattern
                    var match = Regex.Match(
                        query.Query,
                        @"^(?<1>.*?from\s*.[A-z0-9]*.\s*as\s*[A-z0-9]*)(?<2>.*)$|^(?<1>.*?from\s*'[A-z0-9]*'.\s*)(?<2>select.*)$",
                        RegexOptions.Singleline);
                    if (!match.Success)
                    {
                        throw new RavenDBAccessException($"Not able to detect query for transformer '{typeof(TTransformer).Name}'!");
                    }
                    // use id() to not create auto index
                    query.Query = match.Groups[1].Value + " where id() IN ($idents) " + match.Groups[2].Value;
                    query.QueryParameters.Add("idents", idents);
                }));
            var transformer = InstanceCreator<TTransformer>.Create();
            return await transformer.TransformResult(basicQuery).ToArrayAsync();
        }

        public async Task<TProjection> LoadWithTransformerAsync<T, TTransformer, TProjection>(object id) where TTransformer : AbstractTransformer<T, TProjection>, new() where T : class
        {
            var basicQuery = _session.Query<T>()
                .Customize(customization => customization.BeforeQueryExecuted(query =>
                {
                    // regex for query pattern
                    var match = Regex.Match(
                        query.Query,
                        @"^(?<1>.*?from\s*.[A-z0-9]*.\s*as\s*[A-z0-9]*)(?<2>.*)$|^(?<1>.*?from\s*'[A-z0-9]*'.\s*)(?<2>select.*)$",
                        RegexOptions.Singleline);
                    if (!match.Success)
                    {
                        throw new RavenDBAccessException($"Not able to detect query for transformer '{typeof(TTransformer).Name}'!");
                    }
                    // use id() to not create auto index
                    query.Query = match.Groups[1].Value + " where id() = $ident " + match.Groups[2].Value;
                    query.QueryParameters.Add("ident", ConvertToStringId<T>(id));
                }));
            var transformer = InstanceCreator<TTransformer>.Create();
            return await transformer.TransformResult(basicQuery).FirstOrDefaultAsync();
        }

        public Lazy<Task<TProjection>> LoadLazyWithTransformerAsync<T, TTransformer, TProjection>(object id) where TTransformer : AbstractTransformer<T, TProjection>, new() where T : class
        {
            var basicQuery = _session.Query<T>()
                .Customize(customization => customization.BeforeQueryExecuted(query =>
                {
                    // regex for query pattern
                    var match = Regex.Match(
                        query.Query,
                        @"^(?<1>.*?from\s*.[A-z0-9]*.\s*as\s*[A-z0-9]*)(?<2>.*)$|^(?<1>.*?from\s*'[A-z0-9]*'.\s*)(?<2>select.*)$",
                        RegexOptions.Singleline);
                    if (!match.Success)
                    {
                        throw new RavenDBAccessException($"Not able to detect query for transformer '{typeof(TTransformer).Name}'!");
                    }
                    // use id() to not create auto index
                    query.Query = match.Groups[1].Value + " where id() = $ident " + match.Groups[2].Value;
                    query.QueryParameters.Add("ident", ConvertToStringId<T>(id));
                }));
            var transformer = InstanceCreator<TTransformer>.Create();
            var lazyResult = transformer.TransformResult(basicQuery).LazilyAsync();
            return new Lazy<Task<TProjection>>(async () =>
            {
                return (await lazyResult.Value.ConfigureAwait(false)).FirstOrDefault();
            });
        }

        /// <summary>
        /// Loads instances with the specified ids.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <param name="token"></param>
        /// <returns>
        /// Object with specified id (not found items are ignored in result).
        /// </returns>
        public async Task<IDictionary<string, T>> LoadMoreAsync<T>(IEnumerable<object> ids, CancellationToken token = default) where T : class
        {
            return await _session.LoadAsync<T>(ids.Select(x => ConvertToStringId<T>(x)), token).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes by the specified id.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task DeleteByIdAsync<T>(string id, CancellationToken token = default) where T : class
        {
            await Task.Run(() => _session.Delete(id), token).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the specified instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task DeleteAsync<T>(T instance, CancellationToken token = default) where T : class
        {
            if (instance is Task)
            {
                throw new RavenDBAccessException("You should use await for instance parameter!!");
            }
            await Task.Run(() => _session.Delete(instance), token).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the specified instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task StoreAsync<T>(T instance, CancellationToken token = default) where T : class
        {
            if (instance is Task)
            {
                throw new RavenDBAccessException("You should use await for instance parameter!!");
            }
            await _session.StoreAsync(instance, token).ConfigureAwait(false);
        }

        /// <summary>Generates the id.</summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Generate id based on </returns>
        public async Task<string> GenerateId<T>(T entity)
        {
            var typeIdent = GetTypeIdent<T>();
            var seedIdentity = await _session.Advanced.DocumentStore.Maintenance.SendAsync(new NextIdentityForOperation(typeIdent));
            return typeIdent + "/" + seedIdentity;
        }

        /// <summary>
        /// Generate type ident.
        /// </summary>
        /// <returns>Type ident for this session factory type.</returns>
        public string GetTypeIdent<T>()
        {
            return _session.Advanced.DocumentStore.Conventions.FindCollectionName(typeof(T)).ToLower();
        }

        // ########################################
        // Raven async session specific functions section
        // ########################################


        public async IAsyncEnumerator<T> GetCollectionStreamAsync<T>() where T : class
        {
            var stream = await _session.Advanced.StreamAsync(_session.Query<T>());
            while (await stream.MoveNextAsync())
            {
                yield return stream.Current.Document;
            }
        }

        // ########################################
        // End of section for Raven async session specific functions section
        // ########################################


        private string ConvertToStringId<T>(object id)
        {
            if (id is string)
            {
                return (string)id;
            }
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", GetTypeIdent<T>(), id);
        }

        /// <summary>
        /// Disposes the resources.
        /// </summary>
        /// <param name="disposeManagedResources">if set to <c>true</c> [dispose managed resources].</param>


        /// <summary>
        /// Finalizes an instance of the DisposableBase class.
        /// </summary>
        ~RavenAsyncSession()
        {
            Dispose(false);
        }

        // To detect redundant calls
        private bool _disposed = false;
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                // dispose connection if exists
                if (_session != null)
                {
                    if (_currentTransaction != null)
                    {
                        _currentTransaction.Dispose();
                    }
                    RavenAsyncDatabase.Logger.Trace($"Session [{SessionIdent}] closed");
                    _session.Advanced.Clear();
                    _session.Dispose();
                }
            }
            _disposed = true;
        }

        public T Load<T>(object id, bool throwOnNotFound = true) where T : class
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, T> LoadMore<T>(params object[] ids) where T : class
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, T> LoadMore<T>(IEnumerable<object> ids) where T : class
        {
            throw new NotImplementedException("You are trying to use sync methd in a async session");
        }

        public void Delete<T>(T instance) where T : class
        {
            throw new NotImplementedException("You are trying to use sync methd in a async session");
        }

        public void Store<T>(T instance) where T : class
        {
            throw new NotImplementedException("You are trying to use sync methd in a async session");
        }
    }
}
