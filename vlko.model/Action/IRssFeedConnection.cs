using System.Collections.Generic;
using System.Linq;
using System.Text;
using vlko.model.Action.ComplexHelpers.Rss;
using vlko.model.Repository;

namespace vlko.model.Action
{
	public interface IRssFeedConnection : IAction<RssFeed>
	{
		/// <summary>
		/// Gets the feed URL items.
		/// </summary>
		/// <param name="feedUrl">The feed URL.</param>
		/// <returns>Rss feed items.</returns>
		RssItemRawData[] GetFeedUrlItems(string feedUrl);

		/// <summary>
		/// Gets the article.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns>Raw content of article.</returns>
		string GetArticle(string url);
	}
}
