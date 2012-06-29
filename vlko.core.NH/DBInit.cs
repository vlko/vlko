using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
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
            var mapper = new ConventionModelMapper();

		    mapper.BeforeMapProperty += (inspector, member, customizer) =>
		                                    {
		                                        if (member.LocalMember.GetPropertyOrFieldType() == typeof (string) &&
		                                            !member.LocalMember.Name.EndsWith("Text"))
		                                        {
		                                            customizer.Length(50);
		                                        }
		                                        if (member.LocalMember.GetPropertyOrFieldType() == typeof (string) &&
		                                            member.LocalMember.Name.EndsWith("Text"))
		                                        {
                                                    customizer.Type(NHibernateUtil.StringClob);
		                                        }
		                                    };

		    mapper.BeforeMapBag += (inspector, member, customizer) =>
		                               {
		                                   customizer.Cascade(Cascade.All | Cascade.DeleteOrphans);
		                               };

            foreach (var componentDbInit in IoC.ResolveAllInstances<IComponentDbInit>())
            {
                componentDbInit.InitMappings(mapper);
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
