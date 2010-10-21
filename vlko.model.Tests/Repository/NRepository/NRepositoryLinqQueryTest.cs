using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.model.Tests.Repository.NRepository.Implementation;

namespace vlko.model.Tests.Repository.NRepository
{
    [TestClass]
    public class NRepositoryQueryLinqTest : NBaseLocalRepositoryTest
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
        public void Test_Query_1_count_and_toArray()
        {
            var items = BaseRepository.GetQuery<NFilterLinqQueryAction>()
                .WhereType(TypeEnum.SomeFirstType);

            Assert.AreEqual(1, items.Count());
            Assert.AreEqual(1, items.ToArray()[0].ID);
        }

        [TestMethod]
        public void Test_Query_2_count_and_toArray()
        {
            var items = BaseRepository.GetQuery<NFilterLinqQueryAction>()
                .WhereType(TypeEnum.SomeOtherType);

            Assert.AreEqual(3, items.Count());

            var result = items.ToArray();
            Assert.AreEqual(2, result[0].ID);
            Assert.AreEqual(3, result[1].ID);
            Assert.AreEqual(4, result[2].ID);
        }

        [TestMethod]
        public void Test_Query_2_count_and_toArray_ordered()
        {
            var items = BaseRepository.GetQuery<NFilterLinqQueryAction>()
                .WhereType(TypeEnum.SomeOtherType);

            var result = items.OrderByDescending(test => test.ID).ToArray();
            Assert.AreEqual(4, result[0].ID);
            Assert.AreEqual(3, result[1].ID);
            Assert.AreEqual(2, result[2].ID);

            result = items.OrderBy(test => test.ID).ToArray();
            Assert.AreEqual(2, result[0].ID);
            Assert.AreEqual(3, result[1].ID);
            Assert.AreEqual(4, result[2].ID);
        }

        [TestMethod]
        public void Test_Query_3_paged_result()
        {
            var items = BaseRepository.GetQuery<NFilterLinqQueryAction>()
                .WhereType(TypeEnum.SomeOtherType);

            var result = items.ToPage(1, 1);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(3, result[0].ID);

            result = items.ToPage(3, 10);
            Assert.AreEqual(0, result.Length);

            result = items.ToPage(1, 2);
            Assert.AreEqual(1, result.Length);

            result = items.ToPage(5, 10);
            Assert.AreEqual(0, result.Length);
        }
    }
}
