using System;
using System.Collections.Generic;
using System.Reflection;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Patterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Tool.hbm2ddl;
using vlko.BlogModule.NH.Tests.Repository.IOCResolved.Model;
using vlko.BlogModule.NH.Tests.Repository.IOCResolved.Queries;
using vlko.core.InversionOfControl;
using vlko.core.NH.Repository;
using vlko.core.NH.Repository.RepositoryAction;
using vlko.core.NH.Testing;
using vlko.core.Repository.RepositoryAction;

namespace vlko.BlogModule.NH.Tests.Repository.IOCResolved
{
	[TestClass]
	public class IoCNRepositoryLinqTest : InMemoryTest
	{
		private BaseTest _Test;

		[TestInitialize]
		public void Init()
		{
			IoC.AddCatalogAssembly(Assembly.Load("vlko.Core.NH"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule.NH"));

			IoC.AddRerouting<ICreateAction<Hotel>>(() => new CRUDActions<Hotel>());
			IoC.AddRerouting<ICreateAction<Room>>(() => new CRUDActions<Room>());
			IoC.AddRerouting<ICreateAction<Reservation>>(() => new CRUDActions<Reservation>());
			IoC.AddRerouting<IQueryActionAll<Hotel>>(() => new QueryActionAllLinq<Hotel>());
			IoC.AddRerouting<IQueryActionHotelRooms>(() => new QueryActionHotelRoomsLinq());
			IoC.AddRerouting<IQueryActionReservationForDay>(() => new QueryActionReservationForDayLinq());
			IoC.AddRerouting<IQueryActionProjection>(() => new QueryActionProjectionLinq());

			base.SetUp();
	  
			_Test = new BaseTest();
			_Test.Intialize();
		}

		[TestCleanup]
		public void Cleanup()
		{
			IoC.ClearReroutings();
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

			SessionFactory.SessionFactoryInstance = configuration.BuildSessionFactory();
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
