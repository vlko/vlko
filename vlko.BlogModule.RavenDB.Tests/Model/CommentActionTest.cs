using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.Tests;
using vlko.BlogModule.Tests.Model;
using vlko.core.RavenDB.Testing;

namespace vlko.BlogModule.RavenDB.Tests.Model
{
	[TestClass]
	public class CommentCommandsTest : CommentCommandsBaseTest
	{
		public CommentCommandsTest() : base(new RavenDBTestProvider())
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
		public override void Get_by_ids()
		{
			base.Get_by_ids();
		}

		[TestMethod]
		public override void GetAllFlat()
		{
			base.GetAllFlat();
		}

		[TestMethod]
		public override void GetAllFlatDesc()
		{
			base.GetAllFlatDesc();
		}

		[TestMethod]
		public override void GetAllTree()
		{
			base.GetAllTree();
		}

		[TestMethod]
		public override void GetAllAdmin()
		{
			base.GetAllAdmin();
		}
	}
}
