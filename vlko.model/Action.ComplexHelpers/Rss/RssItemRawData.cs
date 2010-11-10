using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vlko.model.Action.ComplexHelpers.Rss
{
	public class RssItemRawData
	{
		public string Id { get; set; }

		public string Url { get; set; }

		public string Title { get; set; }

		public string Author { get; set; }

		public DateTime Published { get; set; }

		public string Text { get; set; }
	}
}
