using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using vlko.BlogModule.Roots;

namespace vlko.BlogModule.RavenDB.Indexes
{
	public class StaticTextSortIndex : AbstractIndexCreationTask<StaticText>
	{
		public StaticTextSortIndex()
		{
			Map = texts => from item in texts
						   where item.ContentType == ContentType.StaticText
			               select new {item.Id, item.FriendlyUrl, item.PublishDate, item.Hidden};
		}
	}
}
