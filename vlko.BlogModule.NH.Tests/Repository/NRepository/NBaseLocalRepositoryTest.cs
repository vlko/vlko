using System;
using System.Collections.Generic;
using ConfOrm;
using ConfOrm.NH;
using ConfOrm.Patterns;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using vlko.core.NH.Repository;
using vlko.core.NH.Testing;
using vlko.core.Repository;
using vlko.core.Repository.RepositoryAction;
using vlko.BlogModule.Tests.Repository.NRepository.Implementation;

namespace vlko.BlogModule.Tests.Repository.NRepository
{
	public class NBaseLocalRepositoryTest : InMemoryTest
	{
		protected IRepository<NTestObject> BaseRepository;

		private IUnitOfWork _session;

		public override void ConfigureMapping(Configuration configuration)
		{
			var orm = new ObjectRelationalMapper();
			var mapper = new Mapper(orm);

			mapper.AddPropertyPattern(mi => mi.GetPropertyOrFieldType() == typeof(string), pm => pm.Length(50));
			orm.Patterns.PoidStrategies.Add(new AssignedPoidPattern());
			// define the mapping shape

			// list all the entities we want to map.
			IEnumerable<Type> baseEntities = GetMappingTypes();

			// we map all classes as Table per class
			orm.TablePerClass(baseEntities);

			// compile the mapping for the specified entities
			HbmMapping mappingDocument = mapper.CompileMappingFor(baseEntities);

			// inject the mapping in NHibernate
			configuration.AddDeserializedMapping(mappingDocument, "Domain");

			SessionFactory.SessionFactoryInstance = configuration.BuildSessionFactory();

		}

		public override Type[] GetMappingTypes()
		{
			return new Type[] {typeof(NTestObject)};
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		public void Initialize()
		{
			SetUp();
			RepositoryFactory.IntitializeWith(new NLocalFactoryResolver(SessionFactory.SessionFactoryInstance));

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
			RepositoryFactory.IntitializeWith(null);
			base.TearDown();
		}
	}
}
