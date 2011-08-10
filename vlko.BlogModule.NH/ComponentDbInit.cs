using System;
using System.Collections.Generic;
using System.Linq;
using ConfOrm;
using ConfOrm.Mappers;
using ConfOrm.NH;
using ConfOrm.Patterns;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Tool.hbm2ddl;
using vlko.BlogModule.Roots;
using vlko.core.NH;
using vlko.core.NH.Repository;

namespace vlko.BlogModule.NH
{
	public class ComponentDbInit : IComponentDbInit
	{
		/// <summary>
		/// Lists the of model types.
		/// </summary>
		/// <returns>List of model types.</returns>
		public Type[] ListOfModelTypes()
		{
			return new[]
					   {
						   typeof(SystemMessage),
						   typeof(Content),
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
		/// <param name="orm">The orm.</param>
		/// <param name="mapper">The mapper.</param>
		public void InitMappings(ObjectRelationalMapper orm, Mapper mapper)
		{

			// list all the entities we want to map.
			IEnumerable<Type> baseEntities = ListOfModelTypes();

			// we map non Content classes as normal
			orm.TablePerClass(baseEntities.Where(type => !(type.IsSubclassOf(typeof(Content)) || type == typeof(Content))));

			// defines the whole hierarchy coming up from Content
			orm.TablePerConcreteClass<Content>();

			mapper.Customize<Comment>(mapping =>
			{
				mapping.Property(item => item.Name, pm => pm.Length(255));
				mapping.Collection(item => item.CommentVersions, cm =>
				{
					cm.Inverse(false);
					cm.Key(km => km.OnDelete(OnDeleteAction.NoAction));
				});
			});

			mapper.Customize<CommentVersion>(mapping => mapping.Property(item => item.UserAgent, pm => pm.Length(255)));
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

			mapper.Customize<SystemMessage>(mapping => mapping.Property(item => item.Sender, pm => pm.Length(255)));

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
		}

	}
}
