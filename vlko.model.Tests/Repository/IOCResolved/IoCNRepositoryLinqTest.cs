using System;
using Castle.ActiveRecord.Testing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core.InversionOfControl;
using vlko.model.Repository;
using vlko.model.Repository.NH;
using vlko.model.Repository.NH.RepositoryAction;
using vlko.model.Repository.RepositoryAction;
using vlko.model.Tests.Repository.IOCResolved.Model;
using vlko.model.Tests.Repository.IOCResolved.Queries;

namespace vlko.model.Tests.Repository.IOCResolved
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
                Component.For<ICreateAction<Hotel>>().ImplementedBy<CRUDActions<Hotel>>(),
                Component.For<ICreateAction<Room>>().ImplementedBy<CRUDActions<Room>>(),
                Component.For<ICreateAction<Reservation>>().ImplementedBy<CRUDActions<Reservation>>(),
                Component.For<IQueryActionAll<Hotel>>().ImplementedBy<QueryActionAllLinq<Hotel>>().LifeStyle.Transient,
                Component.For<IQueryActionHotelRooms>().ImplementedBy<QueryActionHotelRoomsLinq>().LifeStyle.Transient,
                Component.For<IQueryActionReservationForDay>().ImplementedBy<QueryActionReservationForDayLinq>().LifeStyle.Transient,
                Component.For<IQueryActionProjection>().ImplementedBy<QueryActionProjectionLinq>().LifeStyle.Transient
                );
            IoC.InitializeWith(container);
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
        public virtual void QueryProjection()
        {
            _Test.QueryProjection();
        }
    }
}
