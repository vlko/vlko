using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.Tests.Model;
using vlko.core.NH.Testing;

namespace vlko.BlogModule.NH.Tests.Model
{
	[TestClass]
	public class AppSettingCommandsTest : AppSettingCommandsBaseTest
	{
		public AppSettingCommandsTest()
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
