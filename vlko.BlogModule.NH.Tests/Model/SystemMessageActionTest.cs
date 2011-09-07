using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core.InversionOfControl;
using vlko.core.NH.Repository;
using vlko.core.NH.Testing;
using vlko.core.Repository;
using vlko.BlogModule.Roots;
using vlko.BlogModule.Tests.Model;

namespace vlko.BlogModule.NH.Tests.Model
{
	[TestClass]
	public class SystemMessageActionTest : SystemMessageActionBaseTest
	{
		public SystemMessageActionTest()
			: base(new NHTestProvider())
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
