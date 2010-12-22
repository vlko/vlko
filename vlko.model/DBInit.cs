using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Patterns;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Tool.hbm2ddl;
using vlko.core.InversionOfControl;
using vlko.model.Roots;

namespace vlko.model
{
	public static class DBInit
	{
		/// <summary>
		/// Lists the of model types.
		/// </summary>
		/// <returns>List of model types.</returns>
		public static Type[] ListOfModelTypes()
		{
			return new[]
					   {
						   typeof(AppSetting),
						   typeof(SystemMessage),
						   typeof(Content),
						   typeof(User),
						   typeof(Comment),
						   typeof(CommentVersion),
						   typeof(StaticText),
						   typeof(StaticTextVersion),
						   typeof(RssFeed),
						   typeof(RssItem),
						   typeof(TwitterStatus)
					   };
		}

		/// <summary>
		/// Initializes the mappings.
		/// </summary>
		/// <param name="config">The configuration.</param>
		public static void InitMappings(Configuration config)
		{
			var orm = new ObjectRelationalMapper();

			var mapper = new Mapper(orm);

			mapper.AddPropertyPattern(mi => mi.GetPropertyOrFieldType() == typeof(string), pm => pm.Length(50));

			orm.Patterns.PoidStrategies.Add(new GuidOptimizedPoidPattern());


			// list all the entities we want to map.
			IEnumerable<Type> baseEntities = ListOfModelTypes();

			// we map non Content classes as normal
			orm.TablePerClass(baseEntities.Where(type => !(type.IsSubclassOf(typeof(Content)) || type == typeof(Content))));

			// defines the whole hierarchy coming up from Content
			orm.TablePerConcreteClass<Content>();

			orm.Poid<AppSetting>(item => item.Name);

			mapper.Customize<AppSetting>(mapping => mapping.Property(item => item.Value, pm => pm.Length(255)));

			mapper.Customize<Comment>(mapping =>
			{
				mapping.Property(item => item.Name, pm => pm.Length(255));
				mapping.Collection(item => item.CommentVersions, cm =>
				{
					cm.Inverse(false);
					cm.Key(km => km.OnDelete(OnDeleteAction.NoAction));
				});
			});

			mapper.Customize<CommentVersion>(mapping =>
			{
				mapping.Property(item => item.UserAgent, pm => pm.Length(255));
				mapping.Property(item => item.Text, pm => { pm.Type(NHibernateUtil.StringClob); pm.Length(int.MaxValue); });
			});
			mapper.Customize<Content>(mapping =>
			{
				mapping.Property(item => item.Description, pm => pm.Length(ModelConstants.DescriptionMaxLenghtConst));
				mapping.Property(item => item.Hidden, pm => pm.NotNullable(true));
				mapping.Property(item => item.AreCommentAllowed, pm => pm.NotNullable(true));
				mapping.Collection(item => item.Comments, cm =>
				{
					cm.Inverse(false);
					cm.Key(km => km.OnDelete(OnDeleteAction.NoAction));
				});
			});

			mapper.Customize<RssFeed>(mapping =>
			{
				mapping.Property(item => item.Url, pm => pm.Length(255));
				mapping.Property(item => item.AuthorRegex, pm => pm.Length(255));
				mapping.Property(item => item.ContentParseRegex, pm => pm.Length(255));
			});

			mapper.Customize<RssItem>(mapping =>
			{
				mapping.Property(item => item.FeedItemId, pm =>
				{
					pm.Length(255);
					pm.Unique(true);
				});
				mapping.Property(item => item.Url, pm => pm.Length(255));
				mapping.Property(item => item.Title, pm => pm.Length(255));
				mapping.Property(item => item.Text, pm => { pm.Type(NHibernateUtil.StringClob); pm.Length(int.MaxValue); });
			});

			mapper.Customize<StaticText>(mapping =>
			{
				mapping.Property(item => item.Title, pm => pm.Length(80));
				mapping.Property(item => item.FriendlyUrl, pm => pm.Unique(true));
				mapping.Collection(item => item.StaticTextVersions, cm =>
				{
					cm.Inverse(false);
					cm.Key(km => km.OnDelete(OnDeleteAction.NoAction));
				}
					);
			});
			mapper.Customize<StaticTextVersion>(mapping => mapping.Property(item => item.Text, pm => { pm.Type(NHibernateUtil.StringClob); pm.Length(int.MaxValue); }));

			mapper.Customize<SystemMessage>(mapping =>
			{
				mapping.Property(item => item.Sender, pm => pm.Length(255));
				mapping.Property(item => item.Text, pm => { pm.Type(NHibernateUtil.StringClob); pm.Length(int.MaxValue); });
			});

			mapper.Customize<TwitterStatus>(mapping =>
			{
				mapping.Property(item => item.TwitterId, pm => pm.Unique(true));
				mapping.Property(item => item.User, pm =>
				{
					pm.Length(255);
					pm.Column("UserName");
				});
			});
			orm.ExcludeProperty<TwitterStatus>(item => item.Text);

			mapper.Customize<User>(mapping =>
			{
				mapping.Property(item => item.Email, pm => pm.Unique(true));
				mapping.Property(item => item.Password, pm => pm.Length(64));
			});

			// compile the mapping for the specified entities
			HbmMapping mappingDocument = mapper.CompileMappingFor(baseEntities);

			// inject the mapping in NHibernate
			config.AddDeserializedMapping(mappingDocument, "Domain");
			// fix up the schema
			SchemaMetadataUpdater.QuoteTableAndColumns(config);
		}

		/// <summary>
		/// Registers the session factory.
		/// </summary>
		/// <param name="sessionFactory">The session factory.</param>
		public static void RegisterSessionFactory(ISessionFactory sessionFactory)
		{
			IoC.Container.Register(Component.For<ISessionFactory>().Instance(sessionFactory));
		}
	}
}
