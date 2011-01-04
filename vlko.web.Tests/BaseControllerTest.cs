using System.IO;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.NH;
using vlko.BlogModule.NH.Testing;
using vlko.core.InversionOfControl;
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
			IoC.InitializeWith(new WindsorContainer());
			ApplicationInit.InitializeRepositories();
			ApplicationInit.InitializeServices();
			IoC.Resolve<ISearchProvider>().Initialize(Directory.GetCurrentDirectory());
			base.SetUp();

			DBInit.RegisterSessionFactory(SessionFactoryInstance);

			FillDbWithData();
			_session = RepositoryFactory.StartUnitOfWork();
		}

		[TestCleanup]
		public void Cleanup()
		{
			_session.Dispose();
			TearDown();
		}

		public override void ConfigureMapping(NHibernate.Cfg.Configuration configuration)
		{
			DBInit.InitMappings(configuration);
		}

		/// <summary>
		/// Fills the db with data.
		/// </summary>
		protected abstract void FillDbWithData();
	}
}
