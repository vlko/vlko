using System;
using Newtonsoft.Json;
using Raven.Client;
using Raven.Client.Document;
using vlko.BlogModule.RavenDB.Indexes;
using vlko.BlogModule.RavenDB.Repository;
using vlko.BlogModule.RavenDB.Repository.ReferenceProxy;
using vlko.BlogModule.Roots;

namespace vlko.BlogModule.RavenDB
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
		public static void RegisterIndexes(IDocumentStore documentStore)
		{
			new UsersByNameSortIndex().Execute(documentStore);
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
		/// Registers the document store.
		/// </summary>
		/// <param name="documentStore">The document store.</param>
		public static void RegisterDocumentStore(IDocumentStore documentStore)
		{
			SessionFactory.DocumentStoreInstance = documentStore;

			documentStore.Conventions.CustomizeJsonSerializer += json => json.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;

			// register custom contract resolver to handle relations
			documentStore.Conventions.JsonContractResolver = new RelationContractResolver(
				(DefaultRavenContractResolver)documentStore.Conventions.JsonContractResolver,
				documentStore.Conventions,
				ListOfModelTypes());

			// use same type tag for all contents hierarchy
			documentStore.Conventions.FindTypeTagName = type => typeof (Content).IsAssignableFrom(type) ? "Contents" : null;
		}
	}
}
