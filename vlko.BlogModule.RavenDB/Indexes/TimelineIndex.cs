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
		}

		public override Raven.Database.Indexing.IndexDefinition CreateIndexDefinition()
		{
			var indexDefinition = base.CreateIndexDefinition();
			// manually create transform function as original one doesn't convert id to __document_id
			indexDefinition.TransformResults =
				@"results.Select(item => new { Id = item.__document_id, ContentType = item.ContentType })";
			return indexDefinition;
		}
	}
}
