using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.RavenDB.Testing;
using vlko.BlogModule.Tests.Model;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.core.Roots;
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
