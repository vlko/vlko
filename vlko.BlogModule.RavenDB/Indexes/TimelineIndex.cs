using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using vlko.BlogModule.Roots;

namespace vlko.BlogModule.RavenDB.Indexes
{
	public class TimelineIndex : AbstractIndexCreationTask<Content>
	{
		public TimelineIndex()
		{
			Map = contents => from item in contents
							  select new {item.PublishDate, item.Hidden };
			TransformResults = (database, contents) => from item in contents
			                                           select new
			                                                  	{
			                                                  		Id = item.Id,
			                                                  		ContentType = item.ContentType
			                                                  	};
		}

		public override Raven.Database.Indexing.IndexDefinition CreateIndexDefinition()
		{
			var indexDefinition = base.CreateIndexDefinition();
			return indexDefinition;
		}
	}
}
