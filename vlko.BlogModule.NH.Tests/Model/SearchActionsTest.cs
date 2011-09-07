using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.Tests.Model;
using vlko.core.InversionOfControl;
using vlko.core.NH.Testing;
using vlko.core.Repository;
using vlko.core.Roots;
using vlko.BlogModule.Roots;
using vlko.BlogModule.Search;

namespace vlko.BlogModule.NH.Tests.Model
{
	[TestClass]
	public class SearchActionsTest : SearchActionsBaseTest
	{

		public SearchActionsTest()
			: base(new NHTestProvider())
		{
		}

		private User _user;

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
		public override void Test_index_static_text()
		{
			base.Test_index_static_text();
		}

		[TestMethod]
		public override void Test_index_comment()
		{
			base.Test_index_comment();
		}

		[TestMethod]
		public override void Test_index_twitter_status()
		{
			base.Test_index_twitter_status();
		}

		[TestMethod]
		public override void Test_index_rss_item()
		{
			base.Test_index_rss_item();
		}

		[TestMethod]
		public override void Multithreading_test()
		{
			base.Multithreading_test();
		}

		[TestMethod]
		public override void Test_delete()
		{
			base.Test_delete();
		}

		[TestMethod]
		public override void Test_Update()
		{
			base.Test_Update();
		}

		[TestMethod]
		public override void Test_search()
		{
			base.Test_search();
		}

		[TestMethod]
		public override void Test_search_by_date()
		{
			base.Test_search_by_date();
		}
	}
}
