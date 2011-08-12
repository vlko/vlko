using System;
using Newtonsoft.Json;
using Raven.Client;
using Raven.Client.Document;
using vlko.BlogModule.RavenDB.Indexes;
using vlko.BlogModule.Roots;
using vlko.core.RavenDB;
using vlko.core.RavenDB.Indexes;
using vlko.core.RavenDB.Repository;
using vlko.core.RavenDB.Repository.ReferenceProxy;
using vlko.core.Roots;

namespace vlko.BlogModule.RavenDB
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
						   typeof(StaticText),
						   typeof(RssFeed),
						   typeof(RssItem),
						   typeof(TwitterStatus)
					   };
		}

		/// <summary>
		/// Registers the indexes.
		/// </summary>
		/// <param name="documentStore">The document store.</param>
		public void RegisterIndexes(IDocumentStore documentStore)
		{
			new RssFeedsWithItemsCount().Execute(documentStore);
			new RssFeedSortIndex().Execute(documentStore);
			new RssItemSortIndex().Execute(documentStore);
			new CommentSortIndex().Execute(documentStore);
			new TwitterStatusSortIndex().Execute(documentStore);
			new StaticTextSortIndex().Execute(documentStore);
			new ContentWithCommentsCount().Execute(documentStore);
			new TimelineIndex().Execute(documentStore);
		}

		/// <summary>
		/// Customizes the document store.
		/// </summary>
		/// <param name="documentStore">The document store.</param>
		public void CustomizeDocumentStore(IDocumentStore documentStore)
		{
			// use same type tag for all contents hierarchy
			documentStore.Conventions.FindTypeTagName = type => typeof (Content).IsAssignableFrom(type) ? "Contents" : null;
		}
	}
}
