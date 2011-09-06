using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.RavenDB.Action;
using vlko.BlogModule.Tests.Model;
using vlko.core.Action;
using vlko.core.InversionOfControl;
using vlko.core.RavenDB.Action;
using vlko.core.RavenDB.Testing;
using vlko.core.Repository;
using vlko.core.Roots;
using vlko.BlogModule.Action;
using vlko.BlogModule.Action.CRUDModel;
using vlko.BlogModule.Action.ViewModel;
using vlko.BlogModule.Roots;

namespace vlko.BlogModule.RavenDB.Tests.Model
{
	[TestClass]
	public class TimelineTest : TimelineBaseTest
	{
		public TimelineTest()
			: base(new RavenDBTestProvider())
		{
		}

		[TestInitialize]
		public override void Init()
		{
			base.Init();
		}

		[TestCleanup]
		public override void Cleanup()
		{
			base.Cleanup();
		}

		[TestMethod]
		public override void Get_timeline_all()
		{
			base.Get_timeline_all();
		}

		[TestMethod]
		public override void Get_timeline_page()
		{
			base.Get_timeline_page();
		}

		[TestMethod]
		public override void Get_hierarchical_content()
		{
			base.Get_hierarchical_content();
		}
	}
}
