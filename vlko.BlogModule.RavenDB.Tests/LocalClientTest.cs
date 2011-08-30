using System.IO;
using System.Reflection;
using System.Threading;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Database.Extensions;
using vlko.core.RavenDB;
using vlko.core.RavenDB.Repository;
using vlko.core.Repository;

namespace vlko.BlogModule.RavenDB.Tests
{
	public abstract class LocalClientTest
	{
		private string path;
		protected IDocumentStore Store { get; private set; }

		public void SetUp()
		{
			RepositoryFactory.IntitializeWith(null);
			Store = 
				NewEmbedableDocumentStore();
				//NewRemoteDocumentStore();
			CustomizeStore(Store);
			Store.Initialize();
			DBInit.RegisterDocumentStore(Store);
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
			path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(LocalClientTest)).CodeBase).Substring(6);
			foreach (var folder in Directory.EnumerateDirectories(path))
			{
				if (!folder.EndsWith("TestResults") && !folder.EndsWith("Index"))
				{
					IOExtensions.DeleteDirectory(folder);
				}
			}


			var documentStore = new EmbeddableDocumentStore()
			{
				Configuration =
				{
					DataDirectory = path,
					
					RunInMemory = true,
					RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true
				},
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
