using System;
using System.Collections.Generic;
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
	public class StaticTextCommandsTest : StaticTextCommandsBaseTest
	{
		public StaticTextCommandsTest()
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
		public override void Test_find_by_primary_key()
		{
			base.Test_find_by_primary_key();
		}

		[TestMethod]
		public override void Test_create()
		{
			base.Test_create();
		}

		[TestMethod]
		public override void Test_update()
		{
			base.Test_update();
		}

		[TestMethod]
		public override void Test_delete()
		{
			base.Test_delete();
		}

		[TestMethod]
		public override void Data_get_by_id()
		{
			base.Data_get_by_id();
		}

		[TestMethod]
		public override void Data_get_by_friendly_url()
		{
			base.Data_get_by_friendly_url();
		}

		[TestMethod]
		public override void Data_get_all()
		{
			base.Data_get_all();
		}

		[TestMethod]
		public override void Data_get_by_ids()
		{
			base.Data_get_by_ids();
		}

		[TestMethod]
		public override void Data_test_pivot()
		{
			base.Data_test_pivot();
		}
	}
}
