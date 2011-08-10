using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.NH;
using vlko.core.InversionOfControl;
using vlko.core.NH;
using vlko.core.NH.Repository;
using vlko.core.NH.Testing;
using vlko.core.Repository;

namespace vlko.BlogModule.Tests.Model
{
	[TestClass]
	public class DbInitializationTest : InMemoryTest

	{
		[TestInitialize]
		public void Init()
		{
			IoC.AddCatalogAssembly(Assembly.Load("vlko.Core.NH"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule.NH"));
			base.SetUp();
		}

		[TestCleanup]
		public void Cleanup()
		{
			TearDown();
		}

		[TestMethod]
		public void Test_querying_all_model_types()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				foreach (Type modelType in DbInit.ListOfModelTypes())
				{
					var modelItems = SessionFactory.Current.CreateCriteria(modelType).List();
					Assert.AreEqual(0, modelItems.Count);
				}
			}
		}
	}
}
