using System;
using Castle.ActiveRecord.Testing;
using vlko.model.Repository;
using vlko.model.Repository.RepositoryAction;
using vlko.model.Tests.Repository.NRepository.Implementation;

namespace vlko.model.Tests.Repository.NRepository
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
            RepositoryFactory.IntitializeWith(new NLocalFactoryResolver());

            _session = RepositoryFactory.StartUnitOfWork();

            BaseRepository = RepositoryFactory.GetRepository<NTestObject>();
            using (var tran = RepositoryFactory.StartTransaction())
            {
                BaseRepository.GetAction<ICreateAction<NTestObject>>().Create(new NTestObject
                {
                    ID = 1,
                    Text = "testFirst",
                    Type = TypeEnum.SomeFirstType
                });
                BaseRepository.GetAction<ICreateAction<NTestObject>>().Create(new NTestObject
                {
                    ID = 2,
                    Text = "testSecond",
                    Type = TypeEnum.SomeOtherType
                });
                BaseRepository.GetAction<ICreateAction<NTestObject>>().Create(new NTestObject
                {
                    ID = 3,
                    Text = "testThird",
                    Type = TypeEnum.SomeOtherType
                });
                BaseRepository.GetAction<ICreateAction<NTestObject>>().Create(new NTestObject
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
