using Npgsql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vlko.core.DBAccess;
using vlko.core.InversionOfControl;
using Dapper;
using Dapper.Contrib.Extensions;
using vlko.core.DBAccess.Querying;
using vlko.core.DBAccess.Exceptions;

namespace vlko.core.DBAccess.PostgreSQL
{
    public class PostgreSQLSession : ISession
    {
        private readonly NpgsqlConnection _connection;
        private int _disposableState;
        private PostgreSQLTransaction _currentTransaction;

        public NpgsqlConnection Connection { get => _connection; }

        internal PostgreSQLSession(NpgsqlConnection connection, Guid sessionIdent)
        {
            _connection = connection;
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

        public NpgsqlTransaction NativeTransaction { get => _currentTransaction?.InnerTransaction; }

        /// <summary>
        /// Initializes the specified session with scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        public void Init(IoCScope scope)
        {
            Scope = scope;
            _connection.Open();
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
                new PostgreSQLAccessException($"Command of type '{typeof(TCommand).Name}' was not resolved by current session scope!!");
            }
            if (!(command is ICommands<PostgreSQLSession>))
            {
                new PostgreSQLAccessException($"Command of type '{typeof(TCommand).Name}' does not implement ICommand<RavenSession> interface!!");
            }
            (command as ICommands<PostgreSQLSession>).Init(this);
            return command;
        }


        /// <summary>
        /// Starts the transaction and return instance maintaining it.
        /// </summary>
        /// <returns>Transaction object, the only way to store changes.</returns>
        public ITransaction StartTransaction()
        {
            if (_currentTransaction != null)
            {
                _currentTransaction = null;
                throw new PostgreSQLAccessException($"There is already active transaction in this session [{SessionIdent}]!");
            }
            _currentTransaction = new PostgreSQLTransaction(this);
            _currentTransaction.Init();
            return _currentTransaction;
        }

        internal void DisposeTransaction()
        {
            _currentTransaction = null;
        }

        public T Load<T>(object id, bool throwOnNotFound = true) where T : class
        {
            T loaded = _connection.Get<T>(id, _currentTransaction.InnerTransaction);
            if (throwOnNotFound && loaded == null)
            {
                throw new NotFoundException(typeof(T), id, string.Empty);
            }
            return loaded;
        }

        public async Task<T> LoadAsync<T>(object id, bool throwOnNotFound = true, CancellationToken token = default(CancellationToken)) where T: class
        {
            T loaded = await _connection.GetAsync<T>(id, _currentTransaction.InnerTransaction).ConfigureAwait(false);
            if (throwOnNotFound && loaded == null)
            {
                throw new NotFoundException(typeof(T), id, string.Empty);
            }
            return loaded;
        }

        public T[] LoadMoreToArray<T>(params object[] ids) where T : class
        {
            var result = _connection.GetMultiple<T>(ids, _currentTransaction.InnerTransaction);
            return result?.ToArray();
        }

        public async Task<T[]> LoadMoreAsyncToArray<T>(params object[] ids) where T: class
        {
            var result = await _connection.GetMultipleAsync<T>(ids, _currentTransaction.InnerTransaction).ConfigureAwait(false);
            return result?.ToArray();
        }

        public T[] LoadMoreToArray<T>(IEnumerable<object> ids) where T : class
        {
            var result = _connection.GetMultiple<T>(ids, _currentTransaction.InnerTransaction);
            return result?.ToArray();
        }

        public async Task<T[]> LoadMoreAsyncToArray<T>(IEnumerable<object> ids, CancellationToken token = default(CancellationToken)) where T: class
        {
            var result = await _connection.GetMultipleAsync<T>(ids, _currentTransaction.InnerTransaction).ConfigureAwait(false);
            return result?.ToArray();
        }

        public void Delete<T>(T instance) where T : class
        {
            _connection.Delete(instance, _currentTransaction.InnerTransaction);
        }

        public async Task DeleteAsync<T>(T instance, CancellationToken token = default(CancellationToken)) where T: class
        {
            await _connection.DeleteAsync(instance, _currentTransaction.InnerTransaction).ConfigureAwait(false);
        }

        public void Store<T>(T instance) where T : class
        {
            if (SqlMapperExtensions. IsKeySpecified(instance))
            {
                _connection.Update<T>(instance, _currentTransaction.InnerTransaction);
            }
            else
            {
                _connection.Insert<T>(instance, _currentTransaction.InnerTransaction);
            }
        }

        public async Task StoreAsync<T>(T instance, CancellationToken token = default(CancellationToken)) where T: class
        {
            if (SqlMapperExtensions.IsKeySpecified(instance))
            {
                await _connection.UpdateAsync<T>(instance, _currentTransaction.InnerTransaction).ConfigureAwait(false);
            }
            else
            {
                await _connection.InsertAsync<T>(instance, _currentTransaction.InnerTransaction).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Disposes the resources.
        /// </summary>
        /// <param name="disposeManagedResources">if set to <c>true</c> [dispose managed resources].</param>
        private void DisposeResources(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                // dispose connection if exists
                if (_connection != null)
                {
                    if (_currentTransaction != null)
                    {
                        _currentTransaction.Dispose();
                    }
                    if (_connection.State != System.Data.ConnectionState.Closed || _connection.State != System.Data.ConnectionState.Broken)
                    {
                        _connection.Close();
                    }
                    PostgreSQLDatabase.Logger.Trace($"Session [{SessionIdent}] closed");
                    _connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Finalizes an instance of the DisposableBase class.
        /// </summary>
        ~PostgreSQLSession()
        {
            this.DisposeResources(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with disposing of resources.
        /// </summary>
        public void Dispose()
        {
            // Attempt to move the disposable state from 0 to 1. If successful, we can be assured that
            // this thread is the first thread to do so, and can safely dispose of the object.
            if (Interlocked.CompareExchange(ref this._disposableState, 1, 0) == 0)
            {
                // Call the DisposeResources method with the disposeManagedResources flag set to true, indicating
                // that derived classes may release unmanaged resources and dispose of managed resources.
                this.DisposeResources(true);

                // Suppress finalization of this object (remove it from the finalization queue and
                // prevent the destructor from being called).
                GC.SuppressFinalize(this);
            }
        }

        public IQueryDef<T> GetQueryDef<T>(IQueryable<T> query) where T : class
        {
            throw new NotImplementedException();
        }

        IDictionary<string, T> ISession.LoadMore<T>(params object[] ids)
        {
            throw new NotImplementedException();
        }

        Task<IDictionary<string, T>> ISession.LoadMoreAsync<T>(params object[] ids)
        {
            throw new NotImplementedException();
        }

        IDictionary<string, T> ISession.LoadMore<T>(IEnumerable<object> ids)
        {
            throw new NotImplementedException();
        }

        Task<IDictionary<string, T>> ISession.LoadMoreAsync<T>(IEnumerable<object> ids, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
