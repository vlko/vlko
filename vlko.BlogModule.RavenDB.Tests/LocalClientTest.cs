using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Raven.Client;
using Raven.Client.Client;
using Raven.Client.Document;
using Raven.Database.Extensions;
using vlko.BlogModule.RavenDB.Repository;
using Newtonsoft.Json.Linq;
using vlko.core.Repository;

namespace vlko.BlogModule.RavenDB.Tests
{
	public abstract class LocalClientTest
	{
		private string path;
		protected IDocumentStore Store { get; private set; }

		public void SetUp()
		{
			Store = 
				NewEmbedableDocumentStore();
				//NewRemoteDocumentStore();
			CustomizeStore(Store);
			Store.Initialize();
		}

		protected virtual void CustomizeStore(IDocumentStore store)
		{
			
		}

		public void TearDown()
		{
			Store.Dispose();
		}

		public IDocumentStore NewRemoteDocumentStore()
		{
			var documentStore = new DocumentStore()
			{
				Url = "http://localhost:8080"
			};
			return documentStore;
		}

		public EmbeddableDocumentStore NewEmbedableDocumentStore()
		{
			path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(LocalClientTest)).CodeBase);
			path = Path.Combine(path, "TestDb").Substring(6);

			IOExtensions.DeleteDirectory(path);

			var documentStore = new EmbeddableDocumentStore()
			{
				Configuration =
				{
					DataDirectory = path,
					RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true
				}

			};
			return documentStore;
		}

		public void WaitForIndexing()
		{
			WaitForIndexing(Store);
		}

		public void WaitForIndexing(IDocumentStore store)
		{
			if (store is EmbeddableDocumentStore)
			{
				while (((EmbeddableDocumentStore)store).DocumentDatabase.Statistics.StaleIndexes.Length > 0)
				{
					Thread.Sleep(100);
				}
			}
			else
			{
				SessionFactory.WaitForStaleIndexes();
			}
		}
	}
}
