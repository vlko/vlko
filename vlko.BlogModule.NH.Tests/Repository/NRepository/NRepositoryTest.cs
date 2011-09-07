using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.NH.Tests.Repository.NRepository.Implementation;
using vlko.core.Repository.Exceptions;
using vlko.core.Repository.RepositoryAction;

namespace vlko.BlogModule.NH.Tests.Repository.NRepository
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

            BaseRepository.GetCommand<ICreateCommand<NTestObject>>().Create(testObj);

            var check = BaseRepository.GetCommand<IFindByPkCommand<NTestObject>>().FindByPk(100);

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

            BaseRepository.GetCommand<ICreateCommand<NTestObject>>().Create(testObj);

            var check = BaseRepository.GetCommand<IFindByPkCommand<NTestObject>>().FindByPk(200);

            Assert.AreEqual(testObj, check);

        	var updateObj = check;
        	updateObj.ID = 200;
        	updateObj.Text = "updateSave";
        	updateObj.Type = TypeEnum.Ignore;

            BaseRepository.GetCommand<IUpdateCommand<NTestObject>>().Update(updateObj);

            check = BaseRepository.GetCommand<IFindByPkCommand<NTestObject>>().FindByPk(200);

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

            BaseRepository.GetCommand<ICreateCommand<NTestObject>>().Create(testObj);

            var check = BaseRepository.GetCommand<IFindByPkCommand<NTestObject>>().FindByPk(300);

            Assert.AreEqual(testObj, check);

            BaseRepository.GetCommand<IDeleteCommand<NTestObject>>().Delete(testObj);

            check = BaseRepository.GetCommand<IFindByPkCommand<NTestObject>>().FindByPk(300);
        }
    }
}
