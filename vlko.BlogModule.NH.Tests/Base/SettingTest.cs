using System;
using System.Globalization;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.NH;
using vlko.core.Action;
using vlko.core.Base.Scheduler.Setting;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.BlogModule.NH.Testing;

namespace vlko.BlogModule.Tests.Base
{
	[TestClass]
	public class SettingTest : InMemoryTest
	{
		private IUnitOfWork _session;

		[TestInitialize]
		public void Init()
		{
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule.NH"));
			base.SetUp();
			_session = RepositoryFactory.StartUnitOfWork();
		}

		[TestCleanup]
		public void Cleanup()
		{
			_session.Dispose();
			TearDown();
		}

		public override void ConfigureMapping(NHibernate.Cfg.Configuration configuration)
		{
			DBInit.InitMappings(configuration);
		}

		[TestMethod]
		public void Get_setting_from_db_fallback_to_default()
		{
			var action = RepositoryFactory.Action<IAppSettingAction>();
			const int defaultInt = 245;
			const string defaultString = "defaultValue";

			var intSetting = new SettingValue<int>("intVal", defaultInt, new DatabaseSettingProvider());
			var stringSetting = new SettingValue<string>("stringVal", defaultString, new DatabaseSettingProvider());

			Assert.AreEqual(defaultInt, intSetting.Value);
			Assert.AreEqual(defaultString, stringSetting.Value);

			// check if db is really empty
			Assert.IsNull(action.Get(intSetting.Name));
			Assert.IsNull(action.Get(stringSetting.Name));
		}

		[TestMethod]
		public void Get_setting_from_config_fallback_to_default()
		{
			const int defaultInt = 245;
			const string defaultString = "defaultValue";

			var intSetting = new SettingValue<int>("intVal", defaultInt, new ConfigSettingProvider());
			var stringSetting = new SettingValue<string>("stringVal", defaultString, new ConfigSettingProvider());

			Assert.AreEqual(defaultInt, intSetting.Value);
			Assert.AreEqual(defaultString, stringSetting.Value);
		}

		[TestMethod]
		public void Save_setting_to_db()
		{
			var action = RepositoryFactory.Action<IAppSettingAction>();
			const int newValueInt = 2;
			const string newValueString = "newValue";

			var intSetting = new SettingValue<int>("newIntVal", 245, new DatabaseSettingProvider());
			var stringSetting = new SettingValue<string>("newStringVal", "defaultValue", new DatabaseSettingProvider());

			intSetting.SaveValue(newValueInt);
			stringSetting.SaveValue(newValueString);

			Assert.AreEqual(newValueInt, intSetting.Value);
			Assert.AreEqual(newValueString, stringSetting.Value);

			// check if db is contain values
			Assert.AreEqual(newValueInt.ToString(CultureInfo.InvariantCulture), action.Get(intSetting.Name).Value);
			Assert.AreEqual(newValueString, action.Get(stringSetting.Name).Value); ;
		}

		[TestMethod]
		[ExpectedException(typeof(NotImplementedException))]
		public void Save_setting_to_config___not_implemented()
		{
			var intSetting = new SettingValue<int>("newIntVal", 245, new ConfigSettingProvider());
			intSetting.SaveValue(1);
		}

		[TestMethod]
		public void Change_setting_to_db()
		{
			var action = RepositoryFactory.Action<IAppSettingAction>();
			const int newValueInt = 2;
			const string newValueString = "newValue";
			const int changeValueInt = 4;
			const string changeValueString = "changeValue";

			var intSetting = new SettingValue<int>("changeIntVal", 245, new DatabaseSettingProvider());
			var stringSetting = new SettingValue<string>("changeStringVal", "defaultValue", new DatabaseSettingProvider());

			// create 
			intSetting.SaveValue(newValueInt);
			stringSetting.SaveValue(newValueString);

			Assert.AreEqual(newValueInt, intSetting.Value);
			Assert.AreEqual(newValueString, stringSetting.Value);

			// check if db is really empty
			Assert.AreEqual(newValueInt.ToString(CultureInfo.InvariantCulture), action.Get(intSetting.Name).Value);
			Assert.AreEqual(newValueString, action.Get(stringSetting.Name).Value); 

			// change 
			intSetting.SaveValue(changeValueInt);
			stringSetting.SaveValue(changeValueString);

			Assert.AreEqual(changeValueInt, intSetting.Value);
			Assert.AreEqual(changeValueString, stringSetting.Value);

			// check if db is really empty
			Assert.AreEqual(changeValueInt.ToString(CultureInfo.InvariantCulture), action.Get(intSetting.Name).Value);
			Assert.AreEqual(changeValueString, action.Get(stringSetting.Name).Value);
		}

		[TestMethod]
		public void Change_setting_to_db_check_string_null()
		{
			var action = RepositoryFactory.Action<IAppSettingAction>();
			const string newValueString = "newValue";

			var stringSetting = new SettingValue<string>("changeStringVal", "defaultValue", new DatabaseSettingProvider());

			// create 
			stringSetting.SaveValue(newValueString);

			Assert.AreEqual(newValueString, stringSetting.Value);

			// check if db is really empty
			Assert.AreEqual(newValueString, action.Get(stringSetting.Name).Value);

			// change 
			stringSetting.SaveValue(null);

			Assert.AreEqual(null, stringSetting.Value);

			// check if db is really empty
			Assert.AreEqual(null, action.Get(stringSetting.Name).Value);
		}
	}
}
