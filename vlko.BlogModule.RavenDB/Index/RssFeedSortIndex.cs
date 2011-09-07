using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using vlko.BlogModule.Roots;

namespace vlko.BlogModule.RavenDB.Indexes
{
	public class RssFeedSortIndex : AbstractIndexCreationTask<RssFeed>
	{
		public RssFeedSortIndex()
		{
			Map = feeds => from item in feeds
			               select new {item.Name};
		}
	}
}
