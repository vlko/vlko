using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.Tests.Model;
using vlko.core.InversionOfControl;
using vlko.core.NH.Repository;
using vlko.core.NH.Testing;
using vlko.core.Repository;
using vlko.BlogModule.Roots;
using vlko.core.Roots;

namespace vlko.BlogModule.NH.Tests.Model
{
	[TestClass]
	public class TwitterStatusActionsTest : TwitterStatusActionsBaseTest
	{
		public TwitterStatusActionsTest()
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
		public override void Test_create()
		{
			base.Test_create();
		}

		[TestMethod]
		public override void Get_by_ids()
		{
			base.Get_by_ids();
		}

		[TestMethod]
		public override void Get_by_twitter_ids()
		{
			base.Get_by_twitter_ids();
		}

		[TestMethod]
		public override void Get_all()
		{
			base.Get_all();
		}
	}
}
