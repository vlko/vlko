using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Castle.ActiveRecord.Testing;
using GeneralRepositoryTest.IOCResolved.Model;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using GenericRepository;
using GeneralRepositoryTest.IOCResolved.Queries;
using System.Xml;
using vlko.core.ActiveRecord;
using vlko.model.IoC;

namespace GeneralRepositoryTest.IOCResolved
{
    [TestClass]
    public class IoCNRepositoryCriterionTest : InMemoryTest
    {
        private BaseTest _Test;

        [TestInitialize]
        public void Init()
        {
            //var doc = new XmlDocument();
            //doc.Load("log4net.config");
            //log4net.Config.XmlConfigurator.Configure(doc.DocumentElement);
            IWindsorContainer container = new WindsorContainer();
            container.Register(
                Component.For<IUnitOfWork>().ImplementedBy<UnitOfWork>().LifeStyle.Transient,
                Component.For<ITransaction>().ImplementedBy<Transaction>().LifeStyle.Transient,
                Component.For<BaseRepository<Hotel>>().ImplementedBy<Repository<Hotel>>(),
                Component.For<BaseRepository<Room>>().ImplementedBy<Repository<Room>>(),
                Component.For<BaseRepository<Reservation>>().ImplementedBy<Repository<Reservation>>(),
                Component.For<IQueryAll<Hotel>>().ImplementedBy<QueryAllCriterion<Hotel>>().LifeStyle.Transient,
                Component.For<IQueryHotelRooms>().ImplementedBy<QueryHotelRoomsCriterion>().LifeStyle.Transient,
                Component.For<IQueryReservationForDay>().ImplementedBy<QueryReservationForDayCriterion>().LifeStyle.Transient,
                Component.For<IQueryProjection>().ImplementedBy<QueryProjectionCriterion>().LifeStyle.Transient
                );
            log4net.LogManager.GetLogger("test").Debug("test");
            IoC.IntitializeWith(container);
            base.SetUp();
      
            _Test = new BaseTest();
            _Test.Intialize();
        }

        public override IDictionary<string, string> GetProperties()
        {
            return new Dictionary<string, string>() { { "show_sql", "true" } };
        }

        [TestCleanup]
        public void Cleanup()
        {
            TearDown();
        }

        public override Type[] GetTypes()
        {
            return new Type[] { typeof(Hotel), typeof(Room), typeof(Reservation), typeof(RoomWithHotelProjection) };
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
