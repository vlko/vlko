using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using vlko.BlogModule.Roots;

namespace vlko.BlogModule.RavenDB.Indexes
{
	public class RssItemSortIndex : AbstractIndexCreationTask<RssItem>
	{
		public RssItemSortIndex()
		{
			Map = rssItems => from item in rssItems
			                  where item.ContentType == ContentType.RssItem
			                  select new {item.FeedItemId, item.PublishDate, item.Hidden};
		}
	}
}
