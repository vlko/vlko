using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vlko.model.Action.ViewModel
{
	public class RssItemViewModel
	{
		/// <summary>
		/// Gets or sets the feed item id.
		/// </summary>
		/// <value>The feed item id.</value>
		public string FeedItemId { get; set; }

		/// <summary>
		/// Gets or sets the published date.
		/// </summary>
		/// <value>The published date.</value>
		public DateTime Published { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		/// <value>The URL.</value>
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the author.
		/// </summary>
		/// <value>The author.</value>
		public string Author { get; set; }
	}
}
