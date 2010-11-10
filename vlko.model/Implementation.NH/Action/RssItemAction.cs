using System;
using System.Collections.Generic;
using System.Linq;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using vlko.model.Action;
using vlko.model.Action.CRUDModel;
using vlko.model.Action.ViewModel;
using vlko.model.Repository;

namespace vlko.model.Implementation.NH.Action
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

			return ActiveRecordLinqBase<RssItem>.Queryable
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
				                	})
				.ToArray();
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
				ActiveRecordLinqBase<RssItem>.Queryable
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
		/// Saves the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Saved item.</returns>
		public RssItemCRUDModel Save(RssItemCRUDModel item)
		{
			var dbItem = ActiveRecordLinqBase<RssItem>.Queryable
				                         	.Where(rssItem => rssItem.FeedItemId == item.FeedItemId)
											.FirstOrDefault();
			

			if (dbItem == null)
			{
				var feed = ActiveRecordMediator<RssFeed>.FindByPrimaryKey(item.FeedId);
				dbItem = new RssItem
				         	{
				         		Id = Guid.Empty,
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
				ActiveRecordMediator<RssFeed>.Create(dbItem);
			}
			else
			{
				ActiveRecordMediator<RssFeed>.Save(dbItem);
			}
			return item;
		}

		/// <summary>
		/// Deletes the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Delete(RssItemCRUDModel item)
		{
			var dbItem = ActiveRecordLinqBase<RssItem>.Queryable
											.Where(rssItem => rssItem.FeedItemId == item.FeedItemId)
											.FirstOrDefault();

			if (dbItem != null)
			{
				dbItem.Hidden = true;
				ActiveRecordMediator<RssFeed>.Save(dbItem);
			}
		}

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns>Query result with all rss feeds.</returns>
		public IQueryResult<RssItemViewModel> GetAll()
		{
			return new QueryLinqResult<RssItemViewModel>(
				ActiveRecordLinqBase<RssItem>.Queryable
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