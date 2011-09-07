using System;
using System.Collections.Generic;
using System.Linq;
using vlko.BlogModule.Action;
using vlko.BlogModule.Action.CRUDModel;
using vlko.BlogModule.Action.ViewModel;
using vlko.BlogModule.Roots;
using vlko.core.NH.Repository;
using vlko.core.Repository;

namespace vlko.BlogModule.NH.Action
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
			var feedIts = feedIds.ToArray();

			if (feedIts.Length == 0)
			{
				return new RssItemCRUDModel[] { };
			}

			return SessionFactory<RssItem>.Queryable
				.Where(item => feedIts.Contains(item.FeedItemId))
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
			var feedIts = feedIds.ToArray();

			if (feedIts.Length == 0)
			{
				return new EmptyQueryResult<RssItemViewModel>();
			}

			return new QueryLinqResult<RssItemViewModel>(
				SessionFactory<RssItem>.Queryable
					.Where(item => feedIts.Contains(item.FeedItemId))
					.Select(item => new RssItemViewModel
					                	{
					                		FeedItemId = item.FeedItemId,
					                		Url = item.Url,
					                		Author = item.Author,
					                		Published = item.PublishDate,
					                		Title = item.Title,
					                		Description = item.Description
					                	}));
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

			return new QueryLinqResult<RssItemViewModelWithId>(
				SessionFactory<RssItem>.Queryable
					.Where(item => ids.Contains(item.Id))
					.Select(item => new RssItemViewModelWithId
					                	{
					                		Id = item.Id,
					                		FeedItemId = item.FeedItemId,
					                		Url = item.Url,
					                		Author = item.Author,
					                		Published = item.PublishDate,
					                		Title = item.Title,
					                		Description = item.Description
					                	}));
		}

		/// <summary>
		/// Saves the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Saved item.</returns>
		public RssItemCRUDModel Save(RssItemCRUDModel item)
		{
			var dbItem = SessionFactory<RssItem>.Queryable
				.Where(rssItem => rssItem.FeedItemId == item.FeedItemId).FirstOrDefault();

			var exists = dbItem != null;

			if (!exists)
			{
				var feed = SessionFactory<RssFeed>.FindByPrimaryKey(item.FeedId);
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

			if (!exists)
			{
				SessionFactory<RssItem>.Create(dbItem);
			}
			else
			{
				SessionFactory<RssItem>.Update(dbItem);
			}
			return item;
		}

		/// <summary>
		/// Deletes the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Delete(RssItemCRUDModel item)
		{
			var dbItem = SessionFactory<RssItem>.Queryable
											.Where(rssItem => rssItem.FeedItemId == item.FeedItemId)
											.FirstOrDefault();

			if (dbItem != null)
			{
				dbItem.Hidden = true;
				SessionFactory<RssItem>.Update(dbItem);
			}
		}

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns>Query result with all rss feeds.</returns>
		public IQueryResult<RssItemViewModel> GetAll()
		{
			return new QueryLinqResult<RssItemViewModel>(
				SessionFactory<RssItem>.Queryable
					.Where(item => !item.Hidden)
					.Select(item => new RssItemViewModel
					                	{
					                		FeedItemId = item.FeedItemId,
					                		Url = item.Url,
					                		Author = item.Author,
					                		Published = item.PublishDate,
					                		Title = item.Title,
					                		Description = item.Description
					                	}));
		}
	}
}