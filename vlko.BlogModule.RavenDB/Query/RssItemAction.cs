using System;
using System.Collections.Generic;
using System.Linq;
using vlko.BlogModule.Action;
using vlko.BlogModule.Action.CRUDModel;
using vlko.BlogModule.Action.ViewModel;
using vlko.BlogModule.RavenDB.Indexes;
using vlko.BlogModule.Roots;
using vlko.core.RavenDB.Repository;
using vlko.core.Repository;

namespace vlko.BlogModule.RavenDB.Action
{
	public class RssItemAction : BaseAction<RssItem>, IRssItemAction
	{
		/// <summary>
		/// Gets by the feed ids.
		/// </summary>
		/// <param name="feedIds">The feed ids.</param>
		/// <returns>Feed items with matching id.</returns>
		public RssItemCRUDModel[] GetByFeedIds(IEnumerable<string> feedIds)
		{
			var itemIds = feedIds.ToArray();

			if (itemIds.Length == 0)
			{
				return new RssItemCRUDModel[] { };
			}

			return SessionFactory<RssItem>.IndexQuery<RssItemSortIndex>()
				.WhereContainsAsOr(itemIds, id => id, id => item => item.FeedItemId == id)
				.ToArray()
				.Select(item => new RssItemCRUDModel
				                	{
				                		FeedItemId = item.FeedItemId,
				                		Url = item.Url,
				                		FeedId = item.RssFeed.Id,
				                		Author = item.Author,
				                		Published = item.PublishDate,
				                		Title = item.Title,
				                		Description = item.Description,
				                		Text = item.Text
				                	}).ToArray();
		}

		/// <summary>
		/// Gets the by ids.
		/// </summary>
		/// <param name="feedIds">The feed ids.</param>
		/// <returns>Only Feed items matching specified ids.</returns>
		public IQueryResult<RssItemViewModel> GetByIds(IEnumerable<string> feedIds)
		{
			var itemIds = feedIds.ToArray();

			if (itemIds.Length == 0)
			{
				return new EmptyQueryResult<RssItemViewModel>();
			}

			return new ProjectionQueryResult<RssItem, RssItemViewModel>(
				SessionFactory<RssItem>.IndexQuery<RssItemSortIndex>()
					.WhereContainsAsOr(itemIds, id => id, id => item => item.FeedItemId == id),
				item => new RssItemViewModel
				        	{
				        		FeedItemId = item.FeedItemId,
				        		Url = item.Url,
				        		Author = item.Author,
				        		Published = item.PublishDate,
				        		Title = item.Title,
				        		Description = item.Description
				        	});
		}

		/// <summary>
		/// Gets the by ids.
		/// </summary>
		/// <param name="feedIds">The feed ids.</param>
		/// <returns>Only Feed items matching specified ids.</returns>
		public IQueryResult<RssItemViewModelWithId> GetByIds(IEnumerable<Guid> feedIds)
		{
			var ids = feedIds.ToArray();

			if (ids.Length == 0)
			{
				return new EmptyQueryResult<RssItemViewModelWithId>();
			}

			return new ProjectionQueryResult<RssItem, RssItemViewModelWithId>(
				SessionFactory<RssItem>.LoadMore(feedIds).AsQueryable(),
					item => new RssItemViewModelWithId
					                	{
					                		Id = item.Id,
					                		FeedItemId = item.FeedItemId,
					                		Url = item.Url,
					                		Author = item.Author,
					                		Published = item.PublishDate,
					                		Title = item.Title,
					                		Description = item.Description
					                	});
		}

		/// <summary>
		/// Saves the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Saved item.</returns>
		public RssItemCRUDModel Save(RssItemCRUDModel item)
		{
			var dbItem = SessionFactory<RssItem>.IndexQuery<RssItemSortIndex>()
				.Where(rssItem => rssItem.FeedItemId == item.FeedItemId).FirstOrDefault();
			

			if (dbItem == null)
			{
				var feed = SessionFactory<RssFeed>.Load(item.FeedId);
				dbItem = new RssItem
				         	{
				         		Id = Guid.NewGuid(),
				         		AreCommentAllowed = false,
				         		RssFeed = feed,

				         	};
			}

			dbItem.CreatedDate = item.Published;
			dbItem.PublishDate = item.Published;
			dbItem.Modified = item.Published;
			dbItem.Text = item.Text;
			dbItem.Description = item.Description;
			dbItem.Author = item.Author;
			dbItem.Title = item.Title;
			dbItem.Url = item.Url;
			dbItem.FeedItemId = item.FeedItemId;

			if (dbItem.Id == Guid.Empty)
			{
				SessionFactory<RssItem>.Store(dbItem);
			}
			else
			{
				SessionFactory<RssItem>.Store(dbItem);
			}
			return item;
		}

		/// <summary>
		/// Deletes the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Delete(RssItemCRUDModel item)
		{
			var dbItem = SessionFactory<RssItem>.IndexQuery<RssItemSortIndex>()
											.Where(rssItem => rssItem.FeedItemId == item.FeedItemId)
											.FirstOrDefault();

			if (dbItem != null)
			{
				dbItem.Hidden = true;
				SessionFactory<RssItem>.Store(dbItem);
			}
		}

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns>Query result with all rss feeds.</returns>
		public IQueryResult<RssItemViewModel> GetAll()
		{
			return new ProjectionQueryResult<RssItem, RssItemViewModel>(
				SessionFactory<RssItem>.IndexQuery<RssItemSortIndex>()
					.Where(item => !item.Hidden),
				item => new RssItemViewModel
				        	{
				        		FeedItemId = item.FeedItemId,
				        		Url = item.Url,
				        		Author = item.Author,
				        		Published = item.PublishDate,
				        		Title = item.Title,
				        		Description = item.Description
				        	})
				.AddSortMapping(rssItem => rssItem.PublishDate, rssItem => rssItem.Published);
		}
	}
}