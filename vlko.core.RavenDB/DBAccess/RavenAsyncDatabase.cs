using NLog;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.Documents.Subscriptions;
using System;
using vlko.core.DBAccess;

namespace vlko.core.RavenDB.DBAccess
{
    public class RavenAsyncDatabase : IDatabase<RavenAsyncSession>
    {
        internal static Logger Logger = LogManager.GetLogger("RavenAsyncDatabase");
        private IDocumentStore _documentStore;

        public RavenAsyncDatabase(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public RavenAsyncSession CreateSession(ISessionOptions sessionOptions)
        {
            if (sessionOptions != null && !(sessionOptions is RavenSessionOptions))
            {
                throw new RavenDBAccessException($"Session options parameter should by RavenSessionOptions type but is {sessionOptions.GetType().Name}!");
            }

            var sessionIdent = Guid.NewGuid();
            Logger.Trace($"Creating session [{sessionIdent}] ...");

            if (sessionOptions is RavenSessionOptions ravenSessionOptions)
            {
                var optionsForSession = new SessionOptions
                {
                    TransactionMode = ravenSessionOptions.ClusterWide ? TransactionMode.ClusterWide : TransactionMode.SingleNode,
                    DisableAtomicDocumentWritesInClusterWideTransaction = ravenSessionOptions.ClusterWide,
                    NoTracking = ravenSessionOptions.NoTracking,
                    NoCaching = ravenSessionOptions.NoCaching
                };
                return new RavenAsyncSession(_documentStore.OpenAsyncSession(optionsForSession), sessionIdent);
            }
            else
            {
                return new RavenAsyncSession(_documentStore.OpenAsyncSession(), sessionIdent);
            }

        }
    }
}
