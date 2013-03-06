using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.RavenDB.Testing;
using vlko.BlogModule.Tests.Model;
using vlko.core.InversionOfControl;
using vlko.core.RavenDB.Repository;
using vlko.core.Repository;
using vlko.BlogModule.Roots;

namespace vlko.BlogModule.RavenDB.Tests.Model
{
	[TestClass]
	public class SystemMessageCommandsTest : SystemMessageCommandsBaseTest
	{
		public SystemMessageCommandsTest()
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
		public override void Test_get_all_sort_by_date_desc()
		{
			base.Test_get_all_sort_by_date_desc();
		}

		[TestMethod]
		public override void Test_add_new()
		{
			base.Test_add_new();
		}
	}
}
