using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using vlko.BlogModule.Roots;

namespace vlko.BlogModule.RavenDB.Indexes
{
	public class TwitterStatusSortIndex : AbstractIndexCreationTask<TwitterStatus>
	{
		public TwitterStatusSortIndex()
		{
			Map = statuses => from item in statuses
							  where item.ContentType == ContentType.TwitterStatus
			                  select new {item.TwitterId, item.CreatedDate};
		}
	}
}
