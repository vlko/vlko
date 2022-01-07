using Microsoft.Data.Sqlite;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace vlko.core.DBAccess.SQLite
{
    public class SQLiteTransaction : ITransaction
    {
        public enum TransactionStateEnum
        {
            NoActiveTransaction,
            Active,
            Commit,
            Rollback
        }

        private readonly SQLiteSession _parentSession;
        private bool _commitRollbackPerformed = false;

        internal SqliteTransaction InnerTransaction { get; private set; }


        internal SQLiteTransaction(SQLiteSession parentSession)
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
                throw new SQLiteAccessException($"There was already commit/rollback operation performed in this transaction assigned to session [{_parentSession.SessionIdent}]!");
            }
            await InnerTransaction.CommitAsync();
            SQLiteDatabase.Logger.Trace($"Transaction for session [{_parentSession.SessionIdent}] commited");
            _commitRollbackPerformed = true;
        }

        public void Commit()
        {
            if (_commitRollbackPerformed)
            {
                _commitRollbackPerformed = true;
                throw new SQLiteAccessException($"There was already commit/rollback operation performed in this transaction assigned to session [{_parentSession.SessionIdent}]!");
            }
            InnerTransaction.Commit();
            SQLiteDatabase.Logger.Trace($"Transaction for session [{_parentSession.SessionIdent}] commited");
            _commitRollbackPerformed = true;
        }


        public void Rollback()
        {
            if (_commitRollbackPerformed)
            {
                _commitRollbackPerformed = true;
                throw new SQLiteAccessException($"There was already commit/rollback operation performed in this transaction assigned to session [{_parentSession.SessionIdent}]!");
            }
            InnerTransaction.Rollback();
            SQLiteDatabase.Logger.Trace($"Transaction for session [{_parentSession.SessionIdent}] rollbacked");
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
                    throw new SQLiteAccessException($"Transaction in session [{_parentSession.SessionIdent}] is open! Try commit or rollback first!");
                }
                InnerTransaction.Dispose();
            }
            _disposed = true;
        }
    }
}
