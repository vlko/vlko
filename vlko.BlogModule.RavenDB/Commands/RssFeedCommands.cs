using System;
using System.Linq;
using vlko.BlogModule.Commands;
using vlko.BlogModule.Commands.CRUDModel;
using vlko.BlogModule.Commands.ViewModel;
using vlko.BlogModule.RavenDB.Indexes;
using vlko.BlogModule.RavenDB.Indexes.ReduceModelView;
using vlko.BlogModule.Roots;
using vlko.core.RavenDB.Repository;
using vlko.core.Repository;
using vlko.core.Repository.Exceptions;

namespace vlko.BlogModule.RavenDB.Commands
{
	public class RssFeedCommands : CommandGroup<RssFeed>, IRssFeedCommands
	{
		/// <summary>
		/// Creates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Created item.</returns>
		public RssFeedCRUDModel Create(RssFeedCRUDModel item)
		{

			var feed = new RssFeed
			           	{
							Id = Guid.NewGuid(),
			           		Name = item.Name,
			           		AuthorRegex = item.AuthorRegex,
			           		GetDirectContent = item.GetDirectContent,
			           		DisplayFullContent = item.DisplayFullContent,
			           		ContentParseRegex = item.ContentParseRegex,
			           		Url = item.Url
			           	};

			SessionFactory<RssFeed>.Store(feed);

			item.Id = feed.Id;

			return item;
		}

		/// <summary>
		/// Finds the by primary key.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>
		/// Item matching id or exception if not exists.
		/// </returns>
		/// <exception cref="NotFoundException">If matching id was not found.</exception>
		public RssFeedCRUDModel FindByPk(Guid id)
		{
			return FindByPk(id, true);
		}

		/// <summary>
		/// Finds item by PK.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="throwOnNotFound">if set to <c>true</c> [throw exception on not found].</param>
		/// <returns>
		/// Item matching id or null/exception if not exists.
		/// </returns>
		public RssFeedCRUDModel FindByPk(Guid id, bool throwOnNotFound)
		{
			RssFeedCRUDModel result = null;

			var item = SessionFactory<RssFeed>.Load(id);

			if (throwOnNotFound && item == null)
			{
				throw new NotFoundException(typeof(RssFeed), id,
											"with relation to StaticTextVersion via Version number");
			}
			return new RssFeedCRUDModel
				{
					Id = item.Id,
					Name = item.Name,
					AuthorRegex = item.AuthorRegex,
					GetDirectContent = item.GetDirectContent,
					DisplayFullContent = item.DisplayFullContent,
					ContentParseRegex = item.ContentParseRegex,
					Url = item.Url
				};
		}

		/// <summary>
		/// Updates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Updated item.</returns>
		public RssFeedCRUDModel Update(RssFeedCRUDModel item)
		{
			var feed = SessionFactory<RssFeed>.Load(item.Id);
			feed.Name = item.Name;
			feed.AuthorRegex = item.AuthorRegex;
			feed.GetDirectContent = item.GetDirectContent;
			feed.DisplayFullContent = item.DisplayFullContent;
			feed.ContentParseRegex = item.ContentParseRegex;
			feed.Url = item.Url;

			SessionFactory<RssFeed>.Store(feed);

			return item;
		}

		/// <summary>
		/// Deletes the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Delete(RssFeedCRUDModel item)
		{
			var feed = SessionFactory<RssFeed>.Load(item.Id);
			SessionFactory<RssFeed>.Delete(feed);
		}

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns>Query result with all rss feeds.</returns>
		public IQueryResult<RssFeedViewModel> GetAll()
		{
			return new ResultProjectionQueryResult<RssFeed, RssFeedViewModel>(
				SessionFactory<RssFeed>.IndexQuery<RssFeedSortIndex>(),
				items =>
					{
						// get feed items counts
						var feedCounts = SessionFactory<RssItem>.IndexQuery<RssFeedsWithItemsCount, RssFeedCount>()
							.WhereContainsAsOr(items, rssFeed => rssFeed.Id, id => item => item.FeedId == id)
							.ToDictionary(item => item.FeedId);
						// merge results
						return items
							.Select(item => new RssFeedViewModel
							                	{
							                		Id = item.Id,
							                		Name = item.Name,
							                		DisplayFullContent = item.DisplayFullContent,
							                		FeedUrl = item.Url,
							                		FeedItemCount = feedCounts.ContainsKey(item.Id) ? feedCounts[item.Id].Count : 0
							                	})
							.ToArray();
					})
					.AddSortMapping(feed => feed.Name, feed => feed.Name);
		}

		/// <summary>
		/// Gets the feed to process.
		/// </summary>
		/// <returns>Feeds to process.</returns>
		public RssFeedCRUDModel[] GetFeedToProcess()
		{
			return
				new ProjectionQueryResult<RssFeed, RssFeedCRUDModel>(
					SessionFactory<RssFeed>.IndexQuery<RssFeedSortIndex>(),
					item => new RssFeedCRUDModel
					        	{
					        		Id = item.Id,
					        		Name = item.Name,
					        		AuthorRegex = item.AuthorRegex,
					        		GetDirectContent = item.GetDirectContent,
					        		DisplayFullContent = item.DisplayFullContent,
					        		ContentParseRegex = item.ContentParseRegex,
					        		Url = item.Url
					        	})
					.ToArray();
		}
	}
}