using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Operations.Identities;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
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
    public class RavenSession : ISession
    {
        private readonly IDocumentSession _session;
        private RavenTransaction _currentTransaction;

        public IAdvancedSessionOperations Advanced { get => _session.Advanced; }

        internal RavenSession(IDocumentSession session, Guid sessionIdent)
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
            if (!(command is ICommands<RavenSession>))
            {
                new RavenDBAccessException($"Command of type '{typeof(TCommand).Name}' does not implement ICommand<RavenSession> interface!!");
            }
            (command as ICommands<RavenSession>).Init(this);
            return command;
        }

        /// <summary>
        /// Resolve and initialize QueryDef object using internaly session.
        /// </summary>
        public IQueryDef<T> GetQueryDef<T>(IQueryable<T> query) where T : class => new QueryDef<T>(query, this);


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
            _currentTransaction = new RavenTransaction(this);
            //rollback all previously queued changes
            RollBackTransaction();

            return _currentTransaction;
        }

        internal void CommitTransaction()
        {
            _session.SaveChanges();
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
        public IDocumentQuery<T> DocumentQuery<TIndexType, T>() where TIndexType : AbstractIndexCreationTask, new() where T : class
        {
            return _session.Advanced.DocumentQuery<T, TIndexType>();
        }

        /// <summary>
        /// Create document query (one using direct lucene syntax)  to query by with projection result.
        /// </summary>
        /// <typeparam name="TIndexType">The type of the index.</typeparam>
        /// <typeparam name="TProjection">The type of the projection.</typeparam>
        public IDocumentQuery<TProjection> DocumentQueryWithProjection<TIndexType, TProjection>() where TIndexType : AbstractIndexCreationTask, new() where TProjection : class
        {
            return _session.Advanced.DocumentQuery<TProjection, TIndexType>().SelectFields<TProjection>();
        }

        /// <summary>
        /// Loads instance with the specified id.
        /// </summary>
        public T Load<T>(object id, bool throwOnNotFound = true) where T : class
        {
            T loaded = _session.Load<T>(ConvertToStringId<T>(id));
            if (throwOnNotFound && loaded == null)
            {
                throw new NotFoundException(typeof(T), id, string.Empty);
            }
            return loaded;
        }

        /// <summary>
        /// Lazily loads instance with the specified id (will not throw error if not exists, but will return null).
        /// </summary>
        /// <returns>
        /// Object with specified id or null if not found.
        /// </returns>
        public Lazy<T> LazilyLoad<T>(object id)
        {
            return _session.Advanced.Lazily.Load<T>(ConvertToStringId<T>(id));
        }

        public IDictionary<string, T> LoadMore<T>(params object[] ids) where T : class
        {
            return _session.Load<T>(ids.Select(x => ConvertToStringId<T>(x)));
        }

        public IDictionary<string, T> LoadMore<T>(IEnumerable<object> ids) where T : class
        {
            return _session.Load<T>(ids.Select(x => ConvertToStringId<T>(x)));

        }

        public TProjection[] LoadMoreWithTransformer<T, TTransformer, TProjection>(params object[] ids) where TTransformer : AbstractTransformer<T, TProjection>, new() where T : class
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
            return transformer.TransformResult(basicQuery).ToArray();
        }

        public TProjection LoadWithTransformer<T, TTransformer, TProjection>(object id) where TTransformer : AbstractTransformer<T, TProjection>, new() where T : class
        {
            var basicQuery = _session.Query<T>()
                .Customize(customization => customization.BeforeQueryExecuted(query =>
                {
                    // regex for query output pattern
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
            return transformer.TransformResult(basicQuery).FirstOrDefault();
        }


        /// <summary>
        /// Deletes the specified instance.
        /// </summary>
        public void Delete<T>(T instance) where T : class
        {
            _session.Delete(instance);
        }

        /// <summary>
        /// Stores the specified instance.
        /// </summary>
        public void Store<T>(T instance) where T : class
        {
            _session.Store(instance);
        }

        // ########################################
        // Raven sync session specific functions section
        // ########################################

        /// <summary>
        /// Deletes db item by id.
        /// </summary>
        public void DeleteById(string id)
        {
            _session.Delete(id);
        }

        /// <summary>
        /// Stores the specified instance.
        /// </summary>
        public void StoreWithId<T>(T instance, string id) where T : class
        {
            _session.Store(instance, id);
        }

        /// <summary>
        /// Loads the specified id and evicts it from session tracking.
        /// </summary>
        public T LoadEvict<T>(object id, bool throwOnNotFound) where T : class
        {
            var record = Load<T>(id, throwOnNotFound);
            if (record != null)
            {
                _session.Advanced.Evict(record);
            }
            return record;
        }

        /// <summary>
        /// Loads the specified ids and evicts them from session tracking.
        /// </summary>
        public IDictionary<string, T> LoadMoreEvict<T>(IEnumerable<string> ids) where T : class
        {
            var records = LoadMore<T>(ids);
            foreach (var item in records.Where(x => x.Value != null))
            {
                _session.Advanced.Evict(item.Value);
            }
            return records;
        }

        /// <summary>
        /// Evict instance form session tracking.
        /// </summary>
        public void Evict<T>(T instance) where T : class
        {
            _session.Advanced.Evict(instance);
        }

        public IEnumerator<T> GetCollectionStream<T>() where T : class
        {
            using var stream = _session.Advanced.Stream(_session.Query<T>());
            while (stream.MoveNext())
            {
                yield return stream.Current.Document;
            }
        }

        [Obsolete("Only for temporary fix if stream not works")]
        public IEnumerator<T> GetPagedCollectionStream<T>() where T : class
        {
            _session.Advanced.MaxNumberOfRequestsPerSession = 100000;
            var ids = new List<string>();
            var collectionName = _session.Advanced.DocumentStore.Conventions.FindCollectionName(typeof(T));
            var query = _session.Advanced.RawQuery<IDProjection>($"from {collectionName} select id() as ID");
            using var stream = _session.Advanced.Stream(query);
            while (stream.MoveNext())
            {
                ids.Add(stream.Current.Document.ID);
            }
            foreach (var batch in ids.Batch(256))
            {
                foreach (var item in _session.Load<T>(batch))
                {
                    _session.Advanced.Evict(item.Value);
                    yield return item.Value;
                }
            }
        }
        internal class IDProjection
        {
            public string ID { get; set; }
        }
            // ########################################
            // End of section for Raven sync session specific functions section
            // ########################################


            /// <summary>Generates the id.</summary>
            /// <param name="entity">The entity.</param>
            /// <returns>Generate id based on </returns>
            public string GenerateId<T>(T entity)
        {
            var typeIdent = GetTypeIdent<T>();
            var seedIdentity = _session.Advanced.DocumentStore.Maintenance.Send(new NextIdentityForOperation(typeIdent));
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

        private string ConvertToStringId<T>(object id)
        {
            if (id is string)
            {
                return (string)id;
            }
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", GetTypeIdent<T>(), id);
        }



        /// <summary>
        /// Finalizes an instance of the DisposableBase class.
        /// </summary>
        ~RavenSession()
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
                    RavenDatabase.Logger.Trace($"Session [{SessionIdent}] closed");
                    _session.Advanced.Clear();
                    _session.Dispose();
                }
            }
            _disposed = true;
        }
#pragma warning disable 1998
        public async Task<T> LoadAsync<T>(object id, bool throwOnNotFound = true, CancellationToken token = default) where T : class
        {
            throw new NotImplementedException();
        }
        public async Task<IDictionary<string, T>> LoadMoreAsync<T>(params object[] ids) where T : class
        {
            throw new NotImplementedException();
        }

        public async Task<IDictionary<string, T>> LoadMoreAsync<T>(IEnumerable<object> ids, CancellationToken token = default) where T : class
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync<T>(T instance, CancellationToken token = default) where T : class
        {
            throw new NotImplementedException();
        }

        public async Task StoreAsync<T>(T instance, CancellationToken token = default) where T : class
        {
            throw new NotImplementedException();
        }
#pragma warning restore 1998
    }
}
