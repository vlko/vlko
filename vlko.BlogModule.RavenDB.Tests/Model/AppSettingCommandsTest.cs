using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.RavenDB.Testing;
using vlko.BlogModule.Tests.Model;
using vlko.core.InversionOfControl;
using vlko.core.RavenDB.Repository;
using vlko.core.Repository;
using vlko.core.Roots;

namespace vlko.BlogModule.RavenDB.Tests.Model
{
	[TestClass]
	public class AppSettingCommandsTest : AppSettingCommandsBaseTest
	{
		public AppSettingCommandsTest()
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
		public override void Test_get()
		{
			base.Test_get();
		}

		[TestMethod]
		public override void Test_save()
		{
			base.Test_save();
		}
	}
}
