using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Security.Application;
using NLog;
using vlko.BlogModule.Action;
using vlko.BlogModule.Action.CRUDModel;
using vlko.BlogModule.Action.ComplexHelpers.Rss;
using vlko.BlogModule.Search;
using vlko.core.Base.Scheduler;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.core.Tools;

namespace vlko.BlogModule.Base.Scheduler
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
			var connectionAction = RepositoryFactory.Action<IRssFeedConnection>();
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
					var rssItems = connectionAction.GetFeedUrlItems(feed.Url);

					using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
					{
						var storedItems = feedItemAction.GetByFeedIds(rssItems.Select(rssItem => rssItem.Id));
						foreach (var rssItem in rssItems)
						{
							var storedItem = storedItems.FirstOrDefault(item => item.FeedItemId == rssItem.Id);
							if (storedItem == null || storedItem.Title != Sanitizer.GetSafeHtmlFragment(rssItem.Title))
							{
								var feedItem = GetFeedItem(rssItem, feed);
								if (feedItem != null)
								{
									// save to db and to index
									feedItemAction.Save(feedItem);
									searchAction.IndexRssItem(tran, feedItem);
									++storedItemsCount;
								}
							}

							
						}
						tran.Commit();
						Logger.Debug("There were '{0}' new rss feeds in feed {1}.", storedItemsCount, feed.Name);
					}
				}
				catch (Exception ex)
				{
					string errorMessage = string.Format("Unable to load feed '{0}' for url '{1}'.", feed.Name, feed.Url);
					Logger.ErrorException(errorMessage, ex);
					throw new Exception(errorMessage, ex);
				}
			}
		}

		/// <summary>Gets the feed items.</summary>
		/// <param name="rssItemRawData">The RSS item raw data.</param>
		/// <param name="feed">The feed.</param>
		/// <returns>Feed items to save.</returns>
		public static RssItemCRUDModel GetFeedItem(RssItemRawData rssItemRawData, RssFeedCRUDModel feed)
		{

			var connectionAction = RepositoryFactory.Action<IRssFeedConnection>();

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
				           		Title = Sanitizer.GetSafeHtmlFragment(rssItemRawData.Title),
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
							string.Format("Unable to get article content for feed '{0}' for item url '{1}'. \nException: {2}", feed.Name,
							              item.Url),
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
					item.Text = Sanitizer.GetSafeHtmlFragment(content);
				}

				item.Text = Sanitizer.GetSafeHtmlFragment(content);
				item.Description = content.RemoveTags().Shorten(ModelConstants.DescriptionMaxLenghtConst);

				return item;
			}
			return null;
		}
	}
}
