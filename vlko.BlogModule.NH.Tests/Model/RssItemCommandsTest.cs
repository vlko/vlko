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

namespace vlko.BlogModule.NH.Tests.Model
{
	[TestClass]
	public class RssItemCommandsTest : RssItemCommandsBaseTest
	{
		public RssItemCommandsTest()
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
		public override void Test_save()
		{
			base.Test_save();
		}

		[TestMethod]
		public override void Test_delete()
		{
			base.Test_delete();
		}

		[TestMethod]
		public override void Get_by_ids()
		{
			base.Get_by_ids();
		}

		[TestMethod]
		public override void Get_by_feed_ids()
		{
			base.Get_by_feed_ids();
		}

		[TestMethod]
		public override void Get_all()
		{
			base.Get_all();
		}
	}

}
