using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.RavenDB.Testing;
using vlko.BlogModule.Tests.Model;
using vlko.core.InversionOfControl;
using vlko.core.RavenDB.Repository;
using vlko.core.Repository;
using vlko.BlogModule.Roots;
using vlko.core.Roots;

namespace vlko.BlogModule.RavenDB.Tests.Model
{
	[TestClass]
	public class TwitterStatusCommandsTest : TwitterStatusCommandsBaseTest
	{
		public TwitterStatusCommandsTest()
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
