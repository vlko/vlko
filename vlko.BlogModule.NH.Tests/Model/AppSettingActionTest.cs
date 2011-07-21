using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Cfg;
using vlko.BlogModule.NH;
using vlko.core.Action;
using vlko.core.Action.Model;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.BlogModule.NH.Repository;
using vlko.BlogModule.NH.Testing;
using vlko.BlogModule.Roots;

namespace vlko.BlogModule.Tests.Model
{
	[TestClass]
	public class AppSettingActionTest : InMemoryTest
	{
		private AppSetting _setting1;
		private AppSetting _setting2;
		private AppSetting _emptySetting;

		[TestInitialize]
		public void Init()
		{
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule.NH"));
			base.SetUp();
			using (var tran = RepositoryFactory.StartTransaction())
			{
				_setting1 = new AppSetting
				            	{
				            		Id = "setting1",
									Value = "val1"
				            	};
				_setting2 = new AppSetting
				{
					Id = "setting2",
					Value = "val2"
				};
				_emptySetting = new AppSetting
				                	{
				                		Id = "empty_setting",
				                		Value = null
				                	};

				SessionFactory<AppSetting>.Create(_setting1);
				SessionFactory<AppSetting>.Create(_setting2);
				SessionFactory<AppSetting>.Create(_emptySetting);
				tran.Commit();
			}
		}

		[TestCleanup]
		public void Cleanup()
		{
			TearDown();
		}

		public override void ConfigureMapping(NHibernate.Cfg.Configuration configuration)
		{
			DBInit.InitMappings(configuration);
		}

		[TestMethod]
		public void Test_get()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var action = RepositoryFactory.Action<IAppSettingAction>();

				var item = action.Get(_setting1.Id);

				Assert.AreEqual(_setting1.Id, item.Name);
				Assert.AreEqual(_setting1.Value, item.Value);

				item = action.Get(_setting2.Id);

				Assert.AreEqual(_setting2.Id, item.Name);
				Assert.AreEqual(_setting2.Value, item.Value);

				var empty = action.Get(_emptySetting.Id);

				Assert.AreEqual(_emptySetting.Id, empty.Name);
				Assert.AreEqual(null, empty.Value);

				var notExisted = action.Get("not_existed");

				Assert.AreEqual(null, notExisted);
			}
		}

		[TestMethod]
		public void Test_save()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var action = RepositoryFactory.Action<IAppSettingAction>();

				var item = action.Get(_setting1.Id);
				item.Value = "changed_value";
				// try to update value
				using (var tran = RepositoryFactory.StartTransaction())
				{
					action.Save(item);
					tran.Commit();
				}

				var dbItem = action.Get(_setting1.Id);

				Assert.AreEqual(item.Name, dbItem.Name);
				Assert.AreEqual(item.Value, dbItem.Value);

				var newItem = new AppSettingModel
				              	{
				              		Name = "new_Value",
				              		Value = "changed_value"
				              	};
				// try to update value
				using (var tran = RepositoryFactory.StartTransaction())
				{
					action.Save(newItem);
					tran.Commit();
				}

				dbItem = action.Get(newItem.Name);

				Assert.AreEqual(newItem.Name, dbItem.Name);
				Assert.AreEqual(newItem.Value, dbItem.Value);
			}
		}
	}
}
