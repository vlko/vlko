using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lucene.Net.Search;
using vlko.BlogModule.Action;
using vlko.core.Repository;
using vlko.BlogModule.Implementation.OtherTech.Action;

namespace vlko.BlogModule.Search
{
	public class SearchResult : IQueryResult<object>
	{
		public const string TypeField = "Type";
		public const string IdField = "Id";
		public const string CommentType = "Comment";
		public const string StaticTextType = "StaticText";
		public const string TwitterStatusType = "TwitterStatus";
		public const string RssItemType = "RssItem";
		public const int MaximumRawResults = 200;

		private readonly TopDocs _topDocs;
		private readonly Searcher _searcher;


		/// <summary>
		/// Initializes a new instance of the <see cref="SearchResult"/> class.
		/// </summary>
		/// <param name="topDocs">The topDocs.</param>
		public SearchResult(TopDocs topDocs, Searcher searcher)
		{
			_topDocs = topDocs;
			_searcher = searcher;
		}

		/// <summary>
		/// Orders the by (NotImplementedException).
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Exception: NotImplementedException.</returns>
		public IQueryResult<object> OrderBy<TKey>(Expression<Func<object, TKey>> query)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Orders the by descending (NotImplementedException).
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Exception: NotImplementedException.</returns>
		public IQueryResult<object> OrderByDescending<TKey>(Expression<Func<object, TKey>> query)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Counts of items in query.
		/// </summary>
		/// <returns>Counts of items in query.</returns>
		public int Count()
		{
			return Math.Min(_topDocs.totalHits, SearchAction.MaximalSearchDepthConst);
		}

		/// <summary>
		/// Return the result as array.
		/// </summary>
		/// <returns>All items from query.</returns>
		public object[] ToArray()
		{
			return ToPage(0, Count());
		}

		/// <summary>
		/// Return the paged result.
		/// </summary>
		/// <param name="startIndex">The start index.</param>
		/// <param name="itemsPerPage">The items per page.</param>
		/// <returns>All items in the specified page.</returns>
		public object[] ToPage(int startIndex, int itemsPerPage)
		{
			// store local variables for ids
			var orderedIds = new List<KeyValuePair<string, string>>();
			var commentIds = new List<Guid>();
			var staticTextIds = new List<Guid>();
			var twitterStatusIds = new List<Guid>();
			var rssItemIdents = new List<string>();

			// check ranges
			startIndex = Math.Min(startIndex * itemsPerPage, _topDocs.totalHits);
			int numberOfResult = Math.Min(Math.Min(startIndex + itemsPerPage, startIndex + MaximumRawResults), _topDocs.totalHits);

			// get ids from search results
			for (int i = startIndex; i < numberOfResult; i++)
			{
				var docId = _topDocs.scoreDocs[i].doc;
				var doc = _searcher.Doc(docId);
				var id = doc.Get(IdField);
				var type = doc.Get(TypeField);
				if (!string.IsNullOrEmpty(id))
				{
					orderedIds.Add(new KeyValuePair<string, string>(id, type));
					switch (type)
					{
						case CommentType:
							commentIds.Add(new Guid(id));
							break;
						case StaticTextType:
							staticTextIds.Add(new Guid(id));
							break;
						case TwitterStatusType:
							twitterStatusIds.Add(new Guid(id));
							break;
						case RssItemType:
							rssItemIdents.Add(id);
							break;
					}
				}
			}

			// get real data from db
			var comments = RepositoryFactory.Action<ICommentData>().GetByIds(commentIds).ToArray().ToDictionary(comment => comment.Id);
			var staticTexts = RepositoryFactory.Action<IStaticTextData>().GetByIds(staticTextIds).ToArray().ToDictionary(staticText => staticText.Id);
			var twitterStatuses = RepositoryFactory.Action<ITwitterStatusAction>().GetByIds(twitterStatusIds).ToArray().ToDictionary(twitterStatus => twitterStatus.Id);
			var rssItems = RepositoryFactory.Action<IRssItemAction>().GetByIds(rssItemIdents).ToArray().ToDictionary(rssItem => rssItem.FeedItemId);

			// compute result
			var result = new List<object>();
			foreach (var orderedId in orderedIds)
			{
				switch (orderedId.Value)
				{
					case CommentType:
						if (comments.ContainsKey(new Guid(orderedId.Key)))
						{
							result.Add(comments[new Guid(orderedId.Key)]);
						}
						break;
					case StaticTextType:
						if (staticTexts.ContainsKey(new Guid(orderedId.Key)))
						{
							result.Add(staticTexts[new Guid(orderedId.Key)]);
						}
						break;
					case TwitterStatusType:
						if (twitterStatuses.ContainsKey(new Guid(orderedId.Key)))
						{
							result.Add(twitterStatuses[new Guid(orderedId.Key)]);
						}
						break;
					case RssItemType:
						if (rssItems.ContainsKey(orderedId.Key))
						{
							result.Add(rssItems[orderedId.Key]);
						}
						break;
				}
			}

			return result.ToArray();
		}
	}
}
