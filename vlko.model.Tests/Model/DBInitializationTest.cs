using System;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core;

namespace vlko.model.Tests.Model
{
    [TestClass]
    public class DbInitializationTest : InMemoryTest
    {
        [TestInitialize]
        public void Init()
        {
            base.SetUp();
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
        public void Test_querying_all_model_types()
        {
            foreach (Type modelType in ApplicationInit.ListOfModelTypes())
            {
                var modelItems = ActiveRecordMediator.FindAll(modelType);
                Assert.AreEqual(0, modelItems.Length);
            }
        }
    }
}
