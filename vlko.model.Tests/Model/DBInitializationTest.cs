using System;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.model.Implementation.NH.Repository;
using vlko.model.Implementation.NH.Testing;

namespace vlko.model.Tests.Model
{
	[TestClass]
	public class DbInitializationTest : InMemoryTest
	{
		[TestInitialize]
		public void Init()
		{
			IoC.InitializeWith(new WindsorContainer());
			ApplicationInit.InitializeRepositories();
			base.SetUp();
			DBInit.RegisterSessionFactory(SessionFactoryInstance);
		}

		[TestCleanup]
		public void Cleanup()
		{
			TearDown();
		}

		public override void ConfigureMapping(NHibernate.Cfg.Configuration configuration)
		{
			DBInit.InitMappings(configuration);
		}

		[TestMethod]
		public void Test_querying_all_model_types()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				foreach (Type modelType in DBInit.ListOfModelTypes())
				{
					var modelItems = SessionFactory.Current.CreateCriteria(modelType).List();
					Assert.AreEqual(0, modelItems.Count);
				}
			}
		}
	}
}
