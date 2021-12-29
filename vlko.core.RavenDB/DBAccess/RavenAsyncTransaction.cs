using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using vlko.core.DBAccess;

namespace vlko.core.RavenDB.DBAccess
{
    public class RavenAsyncTransaction : ITransaction
    {
        public enum TransactionStateEnum
        {
            NoActiveTransaction,
            Active,
            Commit,
            Rollback
        }

        private readonly RavenAsyncSession _parentSession;
        private bool _commitRollbackPerformed = false;


        internal RavenAsyncTransaction(RavenAsyncSession parentSession)
        {
            _parentSession = parentSession;
        }
        //
        public async Task CommitAsync()
        {
            if (_commitRollbackPerformed)
            {
                _commitRollbackPerformed = true;
                throw new RavenDBAccessException($"There was already commit/rollback operation performed in this transaction assigned to session [{_parentSession.SessionIdent}]!");
            }
            await _parentSession.CommitTransaction();
            RavenAsyncDatabase.Logger.Trace($"Transaction for session [{_parentSession.SessionIdent}] commited");
            _commitRollbackPerformed = true;
        }

        public void Commit()
        {
            throw new NotImplementedException("Not awailable in async session, use CommitAsync.");
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
                    throw new RavenDBAccessException($"Transaction in session [{_parentSession.SessionIdent}] is open! Try commit or rollback first!");
                }
                _parentSession.DisposeTransaction();
            }
            _disposed = true;
        }

        public void Rollback()
        {
            if (_commitRollbackPerformed)
            {
                _commitRollbackPerformed = true;
                throw new RavenDBAccessException($"There was already commit/rollback operation performed in this transaction assigned to session [{_parentSession.SessionIdent}]!");
            }
            _parentSession.RollBackTransaction();
            RavenAsyncDatabase.Logger.Trace($"Transaction for session [{_parentSession.SessionIdent}] rollbacked");
            _commitRollbackPerformed = true;
        }


    }
}
