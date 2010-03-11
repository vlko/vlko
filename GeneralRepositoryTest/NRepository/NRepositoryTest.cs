using GenericRepository.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneralRepositoryTest.NRepository
{
    [TestClass]
    public class NRepositoryTest : NBaseLocalRepositoryTest
    {
        [TestInitialize]
        public void Init()
        {
           base.Initialize();
        }

        [TestCleanup]
        public void Cleanup()
        {
            TearDown();
        }

        [TestMethod]
        public void Test_create()
        {
            var testObj = new NTestObject() 
            {
                ID = 100,
                Text = "create",
                Type = TypeEnum.Ignore
            };

            BaseRepository.Create(testObj);

            var check = BaseRepository.FindByPk(100);

            Assert.AreEqual(testObj, check);
        }

        [TestMethod]
        public void Test_save()
        {
            var testObj = new NTestObject()
            {
                ID = 200,
                Text = "updateCreate",
                Type = TypeEnum.Ignore
            };

            BaseRepository.Create(testObj);

            var check = BaseRepository.FindByPk(200);

            Assert.AreEqual(testObj, check);

            var updateObj = new NTestObject()
            {
                ID = 200,
                Text = "updateSave",
                Type = TypeEnum.Ignore
            };

            BaseRepository.Save(updateObj);

            check = BaseRepository.FindByPk(200);

            Assert.AreEqual(updateObj, check);

        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void Test_delete()
        {
            var testObj = new NTestObject()
            {
                ID = 300,
                Text = "delete",
                Type = TypeEnum.Ignore
            };

            BaseRepository.Create(testObj);

            var check = BaseRepository.FindByPk(300);

            Assert.AreEqual(testObj, check);

            BaseRepository.Delete(testObj);

            check = BaseRepository.FindByPk(300);
        }
    }
}
