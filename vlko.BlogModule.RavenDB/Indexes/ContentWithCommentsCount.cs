using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using vlko.BlogModule.RavenDB.Indexes.ReduceModelView;
using vlko.BlogModule.Roots;

namespace vlko.BlogModule.RavenDB.Indexes
{
	public class ContentWithCommentsCount : AbstractIndexCreationTask<Comment, CommentCount>
	{
		public ContentWithCommentsCount()
		{
			Map = comments => from item in comments
							  select new { ContentId = item.Content.Id, Count = 1 };
			Reduce = reduce => from item in reduce
							   group item by item.ContentId into g
							   select new { ContentId = g.Key, Count = g.Sum(item => item.Count) };

			Sort(x => x.Count, Raven.Abstractions.Indexing.SortOptions.Int);
		}
	}
}
