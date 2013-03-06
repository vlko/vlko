using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Database.Extensions;
using vlko.core.InversionOfControl;
using vlko.core.RavenDB;
using vlko.core.RavenDB.Repository;
using vlko.core.Repository;
using vlko.core.Testing;

namespace vlko.BlogModule.RavenDB.Testing
{
	public class RavenDBTestProvider : ITestProvider
	{
		private string path;
		protected IDocumentStore Store { get; private set; }

		public void SetUp()
		{
			IoC.AddCatalogAssembly(Assembly.Load("vlko.core.RavenDB"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule.RavenDB"));
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

		private void WaitForIndexing(IDocumentStore store)
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

		public void TearDown()
		{
			Store.Dispose();
		}

		public void Create<T>(T model) where T : class
		{
			SessionFactory<T>.Store(model);
		}


		public T GetById<T>(object id) where T : class
		{
			return SessionFactory<T>.Load(id);
		}

		public IEnumerable<T> FindAll<T>() where T : class
		{
			return SessionFactory<T>.FindAll();
		}

		public int Count<T>() where T : class
		{
			return SessionFactory<T>.Count();
		}

		public IQueryable<T> AsQueryable<T>() where T : class
		{
			return SessionFactory<T>.Queryable;
		}
	}
}
