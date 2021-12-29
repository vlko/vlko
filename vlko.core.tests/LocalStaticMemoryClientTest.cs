using Raven.Client.Documents;
using Raven.TestDriver;
using System;
using System.Collections.Generic;
using vlko.core.DBAccess;
using vlko.core.InversionOfControl;
using vlko.core.RavenDB.DBAccess;

namespace finstat.BLL.tests
{
    public delegate void InitStoreData(string storeIdent);
    public abstract class LocalStaticMemoryClientTest : RavenTestDriver
    {
        static LocalStaticMemoryClientTest()
        {
            RavenTestDriver.ConfigureServer(new TestServerOptions
            {
                GracefulShutdownTimeout = TimeSpan.FromSeconds(10),
                CommandLineArgs = new List<string>()
                {
                    $"--Features.Availability=\"Experimental\""
                }
            });
        }
        protected static IDictionary<string, IDocumentStore> Stores { get; set; } = new Dictionary<string, IDocumentStore>();

        private static int _usageCounter = 0;

        public void SetUp(InitStoreData initDataFunction, string storeIdent = null, IoCScope scope = null)
        {
            var _lockIdent = storeIdent ?? "##";
            lock (typeof(LocalStaticMemoryClientTest))
            {
                if (!Stores.ContainsKey(_lockIdent))
                {
                    var store = GetDocumentStore(new GetDocumentStoreOptions {
                        WaitForIndexingTimeout = TimeSpan.FromMinutes(3)
                    });

                    DB.RegisterIdent(storeIdent, scope)
                        .RegisterRavenSessionProvider(store, initializedStore: true);

                    initDataFunction(storeIdent);
                }
                ++_usageCounter;
            }
        }

        protected override void PreInitialize(IDocumentStore documentStore)
        {
            DBInit.SetGenericsConvention(documentStore);
            base.PreInitialize(documentStore);
        }


        public void TearDown()
        {
            lock (typeof(LocalStaticMemoryClientTest))
            {
                --_usageCounter;
                if (_usageCounter == 0)
                {
                    foreach (var store in Stores)
                    {
                        store.Value.Dispose();
                    }
                }
            }
        }

        protected void WaitForIndexing(ISession session)
        {
            IDocumentStore documentStore = null;
            if (session is RavenSession syncSession)
            {
                documentStore = syncSession.Advanced.DocumentStore;
            }
            if (session is RavenAsyncSession asyncSession)
            {
                documentStore = asyncSession.Advanced.DocumentStore;
            }
            if (documentStore != null)
            {
                WaitForIndexing(documentStore);
            }
        }
    }
}
