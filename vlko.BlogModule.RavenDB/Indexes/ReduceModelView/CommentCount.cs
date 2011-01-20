using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vlko.BlogModule.RavenDB.Indexes.ReduceModelView
{
	public class CommentCount
	{
		public Guid ContentId { get; set; }
		public int Count { get; set; }
	}
}
