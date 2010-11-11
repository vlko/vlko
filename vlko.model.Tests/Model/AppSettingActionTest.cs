using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Testing;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core;
using vlko.core.Action;
using vlko.core.Action.Model;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.model.Action;
using vlko.model.Roots;

namespace vlko.model.Tests.Model
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
			IoC.InitializeWith(new WindsorContainer());
			ApplicationInit.InitializeRepositories();
			base.SetUp();
			using (var tran = new TransactionScope())
			{
				_setting1 = new AppSetting
				            	{
				            		Name = "setting1",
									Value = "val1"
				            	};
				_setting2 = new AppSetting
				{
					Name = "setting2",
					Value = "val2"
				};
				_emptySetting = new AppSetting
				                	{
				                		Name = "empty_setting",
				                		Value = null
				                	};

				ActiveRecordMediator<AppSetting>.Create(_setting1);
				ActiveRecordMediator<AppSetting>.Create(_setting2);
				ActiveRecordMediator<AppSetting>.Create(_emptySetting);
			}
		}

		[TestCleanup]
		public void Cleanup()
		{
			TearDown();
		}

		public override Type[] GetTypes()
		{
			return ApplicationInit.ListOfModelTypes();
		}

		[TestMethod]
		public void Test_get()
		{
			using (new SessionScope())
			{
				var action = RepositoryFactory.Action<IAppSettingAction>();

				var item = action.Get(_setting1.Name);

				Assert.AreEqual(_setting1.Name, item.Name);
				Assert.AreEqual(_setting1.Value, item.Value);

				item = action.Get(_setting2.Name);

				Assert.AreEqual(_setting2.Name, item.Name);
				Assert.AreEqual(_setting2.Value, item.Value);

				var empty = action.Get(_emptySetting.Name);

				Assert.AreEqual(_emptySetting.Name, empty.Name);
				Assert.AreEqual(null, empty.Value);

				var notExisted = action.Get("not_existed");

				Assert.AreEqual(null, notExisted);
			}
		}

		[TestMethod]
		public void Test_save()
		{
			using (new SessionScope())
			{
				var action = RepositoryFactory.Action<IAppSettingAction>();

				var item = action.Get(_setting1.Name);
				item.Value = "changed_value";
				// try to update value
				using (var tran = RepositoryFactory.StartTransaction())
				{
					action.Save(item);
				}

				var dbItem = action.Get(_setting1.Name);

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
				}

				dbItem = action.Get(newItem.Name);

				Assert.AreEqual(newItem.Name, dbItem.Name);
				Assert.AreEqual(newItem.Value, dbItem.Value);
			}
		}
	}
}
