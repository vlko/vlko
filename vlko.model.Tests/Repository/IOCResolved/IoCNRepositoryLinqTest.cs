using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Patterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Tool.hbm2ddl;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.core.Repository.RepositoryAction;
using vlko.model.Implementation.NH.Repository;
using vlko.model.Implementation.NH.Repository.RepositoryAction;
using vlko.model.Implementation.NH.Testing;
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

			container.Register(Component.For<NHibernate.ISessionFactory>().Instance(SessionFactoryInstance));
	  
			_Test = new BaseTest();
			_Test.Intialize();
		}

		[TestCleanup]
		public void Cleanup()
		{
			TearDown();
		}

		public override Type[] GetMappingTypes()
		{
			return new Type[] { typeof(Hotel), typeof(Room), typeof(Reservation) };
		}

		public override void ConfigureMapping(NHibernate.Cfg.Configuration configuration)
		{
			var orm = new ObjectRelationalMapper();
			var mapper = new Mapper(orm);

			mapper.AddPropertyPattern(mi => mi.GetPropertyOrFieldType() == typeof(string), pm => pm.Length(50));
			orm.Patterns.PoidStrategies.Add(new NativePoidPattern());
			orm.Patterns.PoidStrategies.Add(new GuidOptimizedPoidPattern());
			// define the mapping shape

			// list all the entities we want to map.
			IEnumerable<Type> baseEntities = GetMappingTypes();

			// we map all classes as Table per class
			orm.TablePerClass(baseEntities);

			mapper.Customize<Hotel>(ca => ca.Collection(item => item.Reservations, cm => cm.Key(km => km.Column("HotelId"))));
			mapper.Customize<Room>(ca => ca.Collection(item => item.Reservations, cm => cm.Key(km =>
			                                                                                   	{
			                                                                                   		km.Column("RoomId");
			                                                                                   		km.OnDelete(OnDeleteAction.NoAction);
			                                                                                   	})));
			mapper.Customize<Reservation>(ca =>
			                              	{
												ca.ManyToOne(item => item.Hotel, m => { m.Column("HotelId"); m.Insert(false); m.Update(false); m.Lazy(LazyRelation.Proxy);} );
												ca.ManyToOne(item => item.Room, m => { m.Column("RoomId"); m.Insert(false); m.Update(false); m.Lazy(LazyRelation.Proxy); });
			                              	});

			// compile the mapping for the specified entities
			HbmMapping mappingDocument = mapper.CompileMappingFor(baseEntities);

			// inject the mapping in NHibernate
			configuration.AddDeserializedMapping(mappingDocument, "Domain");
			// fix up the schema
			SchemaMetadataUpdater.QuoteTableAndColumns(configuration);
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
