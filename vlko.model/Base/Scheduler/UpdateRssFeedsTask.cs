using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Security.Application;
using NLog;
using vlko.core.Base.Scheduler;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.core.Tools;
using vlko.model.Action;
using vlko.model.Action.CRUDModel;
using vlko.model.Search;

namespace vlko.model.Base.Scheduler
{
	public class UpdateRssFeedsTask : SchedulerTask
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UpdateRssFeedsTask"/> class.
		/// </summary>
		/// <param name="callIntervalInMinutes">The call interval in minutes.</param>
		/// <param name="startImmediately">if set to <c>true</c> [start immediately].</param>
		public UpdateRssFeedsTask(int callIntervalInMinutes, bool startImmediately) 
			: base(callIntervalInMinutes, startImmediately)
		{
		}

		/// <summary>
		/// Does the job.
		/// </summary>
		protected override void DoJob()
		{
			var feedItemAction = RepositoryFactory.Action<IRssItemAction>();
			var searchAction = RepositoryFactory.Action<ISearchAction>();

			RssFeedCRUDModel[] rssFeeds = null;

			// get feeds
			using (RepositoryFactory.StartUnitOfWork())
			{
				rssFeeds = RepositoryFactory.Action<IRssFeedAction>().GetFeedToProcess();
			}

			foreach (var feed in rssFeeds)
			{
				try
				{
					var storedItemsCount = 0;
					var feedItems = GetFeedItems(feed);

					using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
					{
						var storedItems = feedItemAction.GetByFeedIds(feedItems.Select(feedItem => feedItem.FeedItemId));
						foreach (var feedItem in feedItems)
						{
							var storedItem = storedItems.FirstOrDefault(item => item.FeedItemId == feedItem.FeedItemId);
							if (storedItem != null)
							{
								// if no change continue to next
								if (storedItem.Title == feedItem.Title
								    && storedItem.Text == feedItem.Text)
								{
									continue;
								}
								// else remove from index
								searchAction.DeleteFromIndex(tran, storedItem.FeedItemId);
							}

							// save to db and to index
							feedItemAction.Save(feedItem);
							searchAction.IndexRssItem(tran, feedItem);
							++storedItemsCount;
						}
						tran.Commit();
						Logger.Debug("There were '{0}' new rss feeds in feed {1}.", storedItemsCount, feed.Name);
					}
				}
				catch (Exception ex)
				{
					Logger.ErrorException(string.Format("Unable to load feed '{0}' for url '{1}'.", feed.Name, feed.Url), ex);
					throw;
				}
			}
		}

		/// <summary>
		/// Gets the feed items.
		/// </summary>
		/// <param name="feed">The feed.</param>
		/// <returns>Feed items to save.</returns>
		public static RssItemCRUDModel[] GetFeedItems(RssFeedCRUDModel feed)
		{
			List<RssItemCRUDModel> result = new List<RssItemCRUDModel>();

			var connectionAction = RepositoryFactory.Action<IRssFeedConnection>();

			// get items from remote url
			var items = connectionAction.GetFeedUrlItems(feed.Url);

			foreach (var rssItemRawData in items)
			{
				// if no author regex or author regex match
				if (string.IsNullOrEmpty(feed.AuthorRegex)
					|| Regex.IsMatch(rssItemRawData.Author, feed.AuthorRegex, RegexOptions.Singleline))
				{
					var item = new RssItemCRUDModel
					           	{
					           		FeedItemId = rssItemRawData.Id,
					           		Url = rssItemRawData.Url,
					           		Published = rssItemRawData.Published,
					           		Author = rssItemRawData.Author,
					           		Title = AntiXss.GetSafeHtmlFragment(rssItemRawData.Title),
					           		FeedId = feed.Id
					           	};

					string content = rssItemRawData.Text;

					// if display full content, then get content url
					if (feed.GetDirectContent)
					{
						
						try
						{
							content = connectionAction.GetArticle(item.Url);
						}
						catch (Exception ex)
						{
							LogManager.GetCurrentClassLogger().ErrorException(
								string.Format("Unable to get article content for feed '{0}' for item url '{1}'. \nException: {2}", feed.Name, item.Url), 
								ex);
						}
					}
					// apply content regex
					if (!string.IsNullOrEmpty(feed.ContentParseRegex))
					{
						var match = Regex.Match(content, feed.ContentParseRegex, RegexOptions.Singleline);
						if (match.Success && match.Groups.Count > 0)
						{
							content = match.Groups[1].Value;
						}
						item.Text = AntiXss.GetSafeHtmlFragment(content);
					}

					item.Text = AntiXss.GetSafeHtmlFragment(content);
					item.Description = content.RemoveTags().Shorten(ModelConstants.DescriptionMaxLenghtConst);

					result.Add(item);
				}
			}
			return result.ToArray();
		}
	}
}
