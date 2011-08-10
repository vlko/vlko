using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConfOrm;
using ConfOrm.NH;
using ConfOrm.Patterns;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Tool.hbm2ddl;
using vlko.core.InversionOfControl;
using vlko.core.NH.Repository;

namespace vlko.core.NH
{
	public static class DbInit
	{
		/// <summary>
		/// Lists the of model types.
		/// </summary>
		/// <returns>List of model types.</returns>
		public static Type[] ListOfModelTypes()
		{
			List<Type> result = new List<Type>();
			foreach (var componentDbInit in IoC.ResolveAllInstances<IComponentDbInit>())
			{
				result.AddRange(componentDbInit.ListOfModelTypes());
			}
			return result.ToArray();
		}

		/// <summary>
		/// Initializes the mappings.
		/// </summary>
		/// <param name="config">The configuration.</param>
		public static void InitMappings(Configuration config)
		{
			var orm = new ObjectRelationalMapper();

			var mapper = new Mapper(orm);

			mapper.AddPropertyPattern(mi => mi.GetPropertyOrFieldType() == typeof(string) && !mi.Name.EndsWith("Text"), pm => pm.Length(50));
			mapper.AddPropertyPattern(mi => mi.GetPropertyOrFieldType() == typeof(string) && mi.Name.EndsWith("Text"), pm => pm.Type(NHibernateUtil.StringClob));

			orm.Patterns.PoidStrategies.Add(new GuidOptimizedPoidPattern());

			foreach (var componentDbInit in IoC.ResolveAllInstances<IComponentDbInit>())
			{
				componentDbInit.InitMappings(orm, mapper);
			}

			// compile the mapping for the specified entities
			HbmMapping mappingDocument = mapper.CompileMappingFor(ListOfModelTypes());

			// inject the mapping in NHibernate
			config.AddDeserializedMapping(mappingDocument, "Domain");
			// fix up the schema
			SchemaMetadataUpdater.QuoteTableAndColumns(config);

			SessionFactory.SessionFactoryInstance = config.BuildSessionFactory();
		}
	}
}
