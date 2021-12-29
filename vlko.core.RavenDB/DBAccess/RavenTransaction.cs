using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using vlko.core.DBAccess;

namespace vlko.core.RavenDB.DBAccess
{
    public class RavenTransaction : ITransaction
    {
        public enum TransactionStateEnum
        {
            NoActiveTransaction,
            Active,
            Commit,
            Rollback
        }

        private readonly RavenSession _parentSession;
        private bool _commitRollbackPerformed = false;


        internal RavenTransaction(RavenSession parentSession)
        {
            _parentSession = parentSession;
        }
        //
        public Task CommitAsync()
        {
            throw new NotImplementedException("Not awailable in async session, use CommitAsync.");
        }

        public void Commit()
        {
            if (_commitRollbackPerformed)
            {
                _commitRollbackPerformed = true;
                throw new RavenDBAccessException($"There was already commit/rollback operation performed in this transaction assigned to session [{_parentSession.SessionIdent}]!");
            }
            _parentSession.CommitTransaction();
            RavenDatabase.Logger.Trace($"Transaction for session [{_parentSession.SessionIdent}] commited");
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
            RavenDatabase.Logger.Trace($"Transaction for session [{_parentSession.SessionIdent}] rollbacked");
            _commitRollbackPerformed = true;
        }


    }
}
