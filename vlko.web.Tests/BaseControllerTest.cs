using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.NH;
using vlko.core.InversionOfControl;
using vlko.core.NH.Testing;
using vlko.core.Repository;
using vlko.BlogModule.Search;

namespace vlko.web.Tests
{
	public abstract class BaseControllerTest : InMemoryTest
	{
		private IUnitOfWork _session;

		[TestInitialize]
		public void Init()
		{
			IoC.AddCatalogAssembly(Assembly.Load("vlko.core.NH"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule.NH"));
			IoC.Resolve<ISearchProvider>().Initialize(Directory.GetCurrentDirectory());

			base.SetUp();

			FillDbWithData();
			_session = RepositoryFactory.StartUnitOfWork();
		}

		[TestCleanup]
		public void Cleanup()
		{
			_session.Dispose();
			TearDown();
		}

		/// <summary>
		/// Fills the db with data.
		/// </summary>
		protected abstract void FillDbWithData();
	}
}
