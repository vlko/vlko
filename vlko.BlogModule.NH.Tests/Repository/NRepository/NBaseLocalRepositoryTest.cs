using System;
using System.Collections.Generic;
using NHibernate.Bytecode.CodeDom;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using vlko.BlogModule.NH.Tests.Repository.NRepository.Implementation;
using vlko.core.NH.Repository;
using vlko.core.NH.Testing;
using vlko.core.Repository;
using vlko.core.Repository.RepositoryAction;

namespace vlko.BlogModule.NH.Tests.Repository.NRepository
{
	public class NBaseLocalRepositoryTest : InMemoryTest
	{
		protected IRepository<NTestObject> BaseRepository;

		private IUnitOfWork _session;

		public override void ConfigureMapping(Configuration configuration)
		{
            var mapper = new ConventionModelMapper();

		    mapper.BeforeMapProperty += (inspector, member, customizer) =>
		                                    {
		                                        if (member.LocalMember.GetPropertyOrFieldType() == typeof (string))
		                                        {
		                                            customizer.Length(50);
		                                        }
		                                    };
		    mapper.BeforeMapClass += (inspector, type, customizer) => customizer.Id(im => im.Generator(Generators.Assigned));
			// define the mapping shape

			// list all the entities we want to map.
			IEnumerable<Type> baseEntities = GetMappingTypes();

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
				BaseRepository.GetCommand<ICreateCommand<NTestObject>>().Create(new NTestObject
				{
					ID = 1,
					Text = "testFirst",
					Type = TypeEnum.SomeFirstType
				});
				BaseRepository.GetCommand<ICreateCommand<NTestObject>>().Create(new NTestObject
				{
					ID = 2,
					Text = "testSecond",
					Type = TypeEnum.SomeOtherType
				});
				BaseRepository.GetCommand<ICreateCommand<NTestObject>>().Create(new NTestObject
				{
					ID = 3,
					Text = "testThird",
					Type = TypeEnum.SomeOtherType
				});
				BaseRepository.GetCommand<ICreateCommand<NTestObject>>().Create(new NTestObject
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
