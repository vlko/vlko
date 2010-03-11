using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace vlko.core.Tests.Model
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
            return ModelInitializer.ListOfModelTypes();
        }

        [TestMethod]
        public void Test_querying_all_model_types()
        {
            foreach (Type modelType in ModelInitializer.ListOfModelTypes())
            {
                var modelItems = ActiveRecordMediator.FindAll(modelType);
                Assert.AreEqual(0, modelItems.Length);
            }
        }
    }
}
