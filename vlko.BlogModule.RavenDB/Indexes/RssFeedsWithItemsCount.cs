using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using vlko.BlogModule.Action.ViewModel;
using vlko.BlogModule.RavenDB.Indexes.ReduceModelView;
using vlko.BlogModule.Roots;

namespace vlko.BlogModule.RavenDB.Indexes
{
	public class RssFeedsWithItemsCount : AbstractIndexCreationTask<RssItem, RssFeedCount>
	{
		public RssFeedsWithItemsCount()
		{
			Map = rssItems => from item in rssItems
							  where item.ContentType == ContentType.RssItem
			                  select new {FeedId = item.RssFeed.Id, Count = 1};
			Reduce = reduce => from item in reduce
							   group item by item.FeedId into g
							   select new { FeedId = g.Key, Count = g.Sum(item => item.Count) };

			SortOptions.Add(x => x.Count, Raven.Abstractions.Indexing.SortOptions.Int);
		}
	}
}
