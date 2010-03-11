using System;
using GenericRepository;
using Castle.ActiveRecord.Testing;

namespace GeneralRepositoryTest.NRepository
{
    public class NBaseLocalRepositoryTest : InMemoryTest
    {
        protected BaseRepository<NTestObject> BaseRepository;

        private IUnitOfWork _session;

        public override Type[] GetTypes()
        {
            return new Type[] {typeof(NTestObject)};
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            SetUp();
            RepositoryIoC.IntitializeWith(new NLocalIoCResolver());

            _session = RepositoryIoC.StartUnitOfWork();

            BaseRepository = RepositoryIoC.GetRepository<NTestObject>();
            using (var tran = RepositoryIoC.StartTransaction())
            {
                BaseRepository.Create(new NTestObject
                {
                    ID = 1,
                    Text = "testFirst",
                    Type = TypeEnum.SomeFirstType
                });
                BaseRepository.Create(new NTestObject
                {
                    ID = 2,
                    Text = "testSecond",
                    Type = TypeEnum.SomeOtherType
                });
                BaseRepository.Create(new NTestObject
                {
                    ID = 3,
                    Text = "testThird",
                    Type = TypeEnum.SomeOtherType
                });
                BaseRepository.Create(new NTestObject
                {
                    ID = 4,
                    Text = "Four",
                    Type = TypeEnum.SomeOtherType
                });
                tran.Commit();
            }
        }

        public override void TearDown()
        {
            _session.Dispose();
            base.TearDown();
        }
    }
}
