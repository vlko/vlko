using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using vlko.BlogModule.Roots;

namespace vlko.BlogModule.RavenDB.Indexes
{
	public class CommentSortIndex : AbstractIndexCreationTask<Comment>
	{
		public CommentSortIndex()
		{
			Map = comments => from item in comments
			               select new {item.Content.Id, item.CreatedDate, item.Level};
		}
	}
}
