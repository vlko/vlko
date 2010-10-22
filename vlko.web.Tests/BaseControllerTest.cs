using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Castle.ActiveRecord.Testing;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core;
using vlko.core.InversionOfControl;
using vlko.model.Search;

namespace vlko.web.Tests
{
	public abstract class BaseControllerTest : InMemoryTest
	{
		[TestInitialize]
		public void Init()
		{
			IoC.InitializeWith(new WindsorContainer());
			ApplicationInit.InitializeRepositories();
			ApplicationInit.InitializeServices();
			IoC.Resolve<ISearchProvider>().Initialize(Directory.GetCurrentDirectory());
			base.SetUp();
			FillDbWithData();
		}

		[TestCleanup]
		public void Cleanup()
		{
			TearDown();
		}

		public override Type[] GetTypes()
		{
			return ApplicationInit.ListOfModelTypes();
		}

		/// <summary>
		/// Fills the db with data.
		/// </summary>
		protected abstract void FillDbWithData();
	}
}
