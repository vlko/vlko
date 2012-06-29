using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
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
        /// <param name="mapper">The mapper.</param>
        public void InitMappings(ConventionModelMapper mapper)
        {
            mapper.Class<Content>(mapping =>
            {
                mapping.Property(item => item.Description, pm => pm.Length(ModelConstants.DescriptionMaxLenghtConst));
                mapping.Property(item => item.Hidden, pm => pm.NotNullable(true));
                mapping.Property(item => item.AreCommentAllowed, pm => pm.NotNullable(true));
                mapping.Bag(item => item.Comments, cm =>
                {
                    cm.Inverse(false);
                    cm.Key(km => km.OnDelete(OnDeleteAction.NoAction));
                });
            });

            mapper.Class<Comment>(mapping =>
                                               {
                                                   mapping.Property(item => item.Name, pm => pm.Length(255));
                                                   mapping.Bag(item => item.CommentVersions, cm =>
                                                        {
                                                            cm.Inverse(false);
                                                            cm.Key(km => km.OnDelete(OnDeleteAction.NoAction));
                                                        });
                                               });
            mapper.Class<CommentVersion>(mapping => mapping.Property(item => item.UserAgent, pm => pm.Length(255)));

            mapper.UnionSubclass<RssItem>(mapping =>
                                               {
                                                   mapping.Property(item => item.FeedItemId, pm =>
                                                                                                 {
                                                                                                     pm.Length(255);
                                                                                                     pm.Unique(true);
                                                                                                 });
                                                   mapping.Property(item => item.Url, pm => pm.Length(255));
                                                   mapping.Property(item => item.Title, pm => pm.Length(255));
                                               });
            mapper.Class<RssFeed>(mapping =>
                                      {
                                          mapping.Property(item => item.Url, pm => pm.Length(255));
                                          mapping.Property(item => item.AuthorRegex, pm => pm.Length(255));
                                          mapping.Property(item => item.ContentParseRegex, pm => pm.Length(255));
                                      });

            mapper.UnionSubclass<StaticText>(mapping =>
                    {
                        mapping.Property(item => item.Title, pm => pm.Length(80));
                        mapping.Property(item => item.FriendlyUrl, pm => pm.Unique(true));
                        mapping.Bag(item => item.StaticTextVersions, cm =>
                                                                                {
                                                                                    cm.Inverse(false);
                                                                                    cm.Key(km =>km.OnDelete(OnDeleteAction.NoAction));
                                                                                });
                    });
            mapper.UnionSubclass<TwitterStatus>(mapping =>
                                                     {
                                                         mapping.Property(item => item.TwitterId, pm => pm.Unique(true));
                                                         mapping.Property(item => item.User, pm =>
                                                                                                 {
                                                                                                     pm.Length(255);
                                                                                                     pm.Column("UserName");
                                                                                                 });
                                                     });

            mapper.Class<SystemMessage>(mapping => mapping.Property(item => item.Sender, pm => pm.Length(255)));
        }

	}
}
