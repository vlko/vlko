using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Castle.ActiveRecord.Testing;
using GeneralRepositoryTest.IOCResolved.Model;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using GenericRepository;
using GeneralRepositoryTest.IOCResolved.Queries;
using vlko.core.ActiveRecord;
using vlko.model.IoC;

namespace GeneralRepositoryTest.IOCResolved
{
    [TestClass]
    public class IoCNRepositoryLinqTest : InMemoryTest
    {
        private BaseTest _Test;

        [TestInitialize]
        public void Init()
        {
            IWindsorContainer container = new WindsorContainer();
            container.Register(
                Component.For<IUnitOfWork>().ImplementedBy<UnitOfWork>().LifeStyle.Transient,
                Component.For<ITransaction>().ImplementedBy<Transaction>().LifeStyle.Transient,
                Component.For<BaseRepository<Hotel>>().ImplementedBy<Repository<Hotel>>(),
                Component.For<BaseRepository<Room>>().ImplementedBy<Repository<Room>>(),
                Component.For<BaseRepository<Reservation>>().ImplementedBy<Repository<Reservation>>(),
                Component.For<IQueryAll<Hotel>>().ImplementedBy<QueryAllLinq<Hotel>>().LifeStyle.Transient,
                Component.For<IQueryHotelRooms>().ImplementedBy<QueryHotelRoomsLinq>().LifeStyle.Transient,
                Component.For<IQueryReservationForDay>().ImplementedBy<QueryReservationForDayLinq>().LifeStyle.Transient,
                Component.For<IQueryProjection>().ImplementedBy<QueryProjectionLinq>().LifeStyle.Transient
                );
            IoC.IntitializeWith(container);
            base.SetUp();
            _Test = new BaseTest();
            _Test.Intialize();
        }

        [TestCleanup]
        public void Cleanup()
        {
            TearDown();
        }

        public override Type[] GetTypes()
        {
            return new Type[] { typeof(Hotel), typeof(Room), typeof(Reservation) };
        }

        [TestMethod]
        public virtual void QueryAllHotels()
        {
            _Test.QueryAllHotels();
        }

        [TestMethod]
        public virtual void QueryHotelRoomsByName()
        {
            _Test.QueryHotelRoomsByName();
        }

        [TestMethod]
        public virtual void QueryReservationsForDate()
        {
            _Test.QueryReservationsForDate();
        }


        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public virtual void QueryProjection()
        {
            _Test.QueryProjection();
        }
    }
}
