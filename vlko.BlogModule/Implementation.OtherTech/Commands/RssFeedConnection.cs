using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;
using vlko.BlogModule.Commands;
using vlko.BlogModule.Commands.ComplexHelpers.Rss;
using vlko.BlogModule.Roots;
using vlko.core.Repository;

namespace vlko.BlogModule.Implementation.OtherTech.Commands
{
	public class RssFeedConnection : CommandGroup<RssFeed>, IRssFeedConnection
	{
		/// <summary>
		/// Gets the feed URL items.
		/// </summary>
		/// <param name="feedUrl">The feed URL.</param>
		/// <returns>Rss feed items.</returns>
		public RssItemRawData[] GetFeedUrlItems(string feedUrl)
		{
			List<RssItemRawData> result = new List<RssItemRawData>();
			using (XmlReader xmlReader = XmlReader.Create(feedUrl))
			{
				SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

				int count = 0;

				if (feed == null)
				{
					throw new Exception("Feed doesn't contain any data.");
				}

				foreach (SyndicationItem feedItem in feed.Items)
				{
					if (feedItem.Links.Count > 0)
					{
						var item = new RssItemRawData();

						string weburl = feedItem.Links[0].Uri.OriginalString;

						// set item ident
						if (feedItem.Id != null)
						{
							item.Id = feedItem.Id;
						}
						else
						{
							item.Id = weburl;
						}

						item.Url = weburl;
						item.Title = feedItem.Title.Text;
						item.Text = feedItem.Content is TextSyndicationContent
										? ((TextSyndicationContent)feedItem.Content).Text
										: feedItem.Summary != null
											? feedItem.Summary.Text
											: string.Empty;
						item.Published = feedItem.PublishDate.ToLocalTime().DateTime;
						item.Author = string.Join(", ", feedItem.Authors.Select(author => GetFirstNonEmpty(author.Name, author.Email, author.Uri)));

						result.Add(item);
					}
				}
			}
			return result.ToArray();
		}

		/// <summary>
		/// Gets the first non empty.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>First non empty string.</returns>
		private static string GetFirstNonEmpty(params string[] input)
		{
			for (int i = 0; i < input.Length; i++)
			{
				if (!string.IsNullOrEmpty(input[i]))
				{
					return input[i];
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the article.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns>Raw content of article.</returns>
		public string GetArticle(string url)
		{
			using (var request = new WebClient())
			{
				request.Headers.Add("CharSet", "UTF-8"); // or
				request.Headers.Add("Charset", "text/html; charset=UTF-8");
				var result = request.DownloadData(url);
				return System.Text.Encoding.UTF8.GetString(result);
			}
		}
	}
}