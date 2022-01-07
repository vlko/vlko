using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace vlko.core.DBAccess.PostgreSQL
{
    public class PostgreSQLTransaction : ITransaction
    {
        public enum TransactionStateEnum
        {
            NoActiveTransaction,
            Active,
            Commit,
            Rollback
        }

        private readonly PostgreSQLSession _parentSession;
        private bool _commitRollbackPerformed = false;

        internal NpgsqlTransaction InnerTransaction { get; private set; }


        internal PostgreSQLTransaction(PostgreSQLSession parentSession)
        {
            _parentSession = parentSession;
        }

        internal void Init()
        {
            InnerTransaction = _parentSession.Connection.BeginTransaction();
        }
        //
        public async Task CommitAsync()
        {
            if (_commitRollbackPerformed)
            {
                _commitRollbackPerformed = true;
                throw new PostgreSQLAccessException($"There was already commit/rollback operation performed in this transaction assigned to session [{_parentSession.SessionIdent}]!");
            }
            await InnerTransaction.CommitAsync();
            PostgreSQLDatabase.Logger.Trace($"Transaction for session [{_parentSession.SessionIdent}] commited");
            _commitRollbackPerformed = true;
        }

        public void Commit()
        {
            if (_commitRollbackPerformed)
            {
                _commitRollbackPerformed = true;
                throw new PostgreSQLAccessException($"There was already commit/rollback operation performed in this transaction assigned to session [{_parentSession.SessionIdent}]!");
            }
            InnerTransaction.Commit();
            PostgreSQLDatabase.Logger.Trace($"Transaction for session [{_parentSession.SessionIdent}] commited");
            _commitRollbackPerformed = true;
        }


        public void Rollback()
        {
            if (_commitRollbackPerformed)
            {
                _commitRollbackPerformed = true;
                throw new PostgreSQLAccessException($"There was already commit/rollback operation performed in this transaction assigned to session [{_parentSession.SessionIdent}]!");
            }
            InnerTransaction.Rollback();
            PostgreSQLDatabase.Logger.Trace($"Transaction for session [{_parentSession.SessionIdent}] rollbacked");
            _commitRollbackPerformed = true;
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
                if (!_commitRollbackPerformed && _parentSession.HasActiveTransaction)
                {
                    _commitRollbackPerformed = true;
                    // if is in exception do not throw error
#pragma warning disable CS0618 // Type or member is obsolete
                    if (Marshal.GetExceptionCode() != 0)
#pragma warning restore CS0618 // Type or member is obsolete
                    {
                        return;
                    }
                    throw new PostgreSQLAccessException($"Transaction in session [{_parentSession.SessionIdent}] is open! Try commit or rollback first!");
                }
                InnerTransaction.Dispose();
            }
            _disposed = true;
        }
    }
}
