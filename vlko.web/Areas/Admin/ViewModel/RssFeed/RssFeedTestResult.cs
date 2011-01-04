using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using vlko.BlogModule.Action.CRUDModel;

namespace vlko.web.Areas.Admin.ViewModel.RssFeed
{
	public class RssFeedTestResult
	{
		public Exception Exception { get; set; }

		public RssItemCRUDModel[] Items { get; set; }
	}
}