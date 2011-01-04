using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core.Repository.Exceptions;
using vlko.core.Repository.RepositoryAction;
using vlko.BlogModule.Tests.Repository.NRepository.Implementation;

namespace vlko.BlogModule.Tests.Repository.NRepository
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

            BaseRepository.GetAction<ICreateAction<NTestObject>>().Create(testObj);

            var check = BaseRepository.GetAction<IFindByPkAction<NTestObject>>().FindByPk(100);

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

            BaseRepository.GetAction<ICreateAction<NTestObject>>().Create(testObj);

            var check = BaseRepository.GetAction<IFindByPkAction<NTestObject>>().FindByPk(200);

            Assert.AreEqual(testObj, check);

        	var updateObj = check;
        	updateObj.ID = 200;
        	updateObj.Text = "updateSave";
        	updateObj.Type = TypeEnum.Ignore;

            BaseRepository.GetAction<IUpdateAction<NTestObject>>().Update(updateObj);

            check = BaseRepository.GetAction<IFindByPkAction<NTestObject>>().FindByPk(200);

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

            BaseRepository.GetAction<ICreateAction<NTestObject>>().Create(testObj);

            var check = BaseRepository.GetAction<IFindByPkAction<NTestObject>>().FindByPk(300);

            Assert.AreEqual(testObj, check);

            BaseRepository.GetAction<IDeleteAction<NTestObject>>().Delete(testObj);

            check = BaseRepository.GetAction<IFindByPkAction<NTestObject>>().FindByPk(300);
        }
    }
}
