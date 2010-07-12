using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using GenericRepository;
using Lucene.Net.Search;
using vlko.core.InversionOfControl;
using vlko.core.Models.Action;

namespace vlko.core.Search
{
	public class SearchResult : IQueryResult<object>
	{
		public const string TypeField = "Type";
		public const string IdField = "Id";
		public const string CommentType = "Comment";
		public const string StaticTextType = "StaticText";
		public const int MaximumRawResults = 200;

		private readonly Hits _hits;


		/// <summary>
		/// Initializes a new instance of the <see cref="SearchResult"/> class.
		/// </summary>
		/// <param name="hits">The hits.</param>
		public SearchResult(Hits hits)
		{
			_hits = hits;
		}

		/// <summary>
		/// Orders the by (NotImplementedException).
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Exception: NotImplementedException.</returns>
		public IQueryResult<object> OrderBy(Expression<Func<object, object>> query)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Orders the by descending (NotImplementedException).
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns>Exception: NotImplementedException.</returns>
		public IQueryResult<object> OrderByDescending(Expression<Func<object, object>> query)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Counts of items in query.
		/// </summary>
		/// <returns>Counts of items in query.</returns>
		public int Count()
		{
			return _hits.Length();
		}

		/// <summary>
		/// Return the result as array.
		/// </summary>
		/// <returns>All items from query.</returns>
		public object[] ToArray()
		{
			return ToPage(0, _hits.Length());
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
			var orderedIds = new List<KeyValuePair<Guid, string>>();
			var commentIds = new List<Guid>();
			var staticTextIds = new List<Guid>();

			// check ranges
			startIndex = Math.Min(startIndex, _hits.Length());
			int numberOfResult = Math.Min(Math.Min(startIndex + itemsPerPage, startIndex + MaximumRawResults), _hits.Length());

			// get ids from search results
			for (int i = startIndex; i < numberOfResult; i++)
			{
				var doc = _hits.Doc(i);
				var id = doc.Get(IdField);
				var type = doc.Get(TypeField);
				if (!string.IsNullOrEmpty(id))
				{
					var guidId = new Guid(id);
					orderedIds.Add(new KeyValuePair<Guid, string>(guidId, type));
					switch (type)
					{
						case CommentType:
							commentIds.Add(guidId);
							break;
						case StaticTextType:
							staticTextIds.Add(guidId);
							break;
					}
				}
			}

			// get real data from db
			var comments = RepositoryFactory.Action<ICommentData>().GetByIds(commentIds).ToArray().ToDictionary(comment => comment.Id);
			var staticTexts = RepositoryFactory.Action<IStaticTextData>().GetByIds(staticTextIds).ToArray().ToDictionary(staticText => staticText.Id);

			// compute result
			var result = new List<object>();
			foreach (var orderedId in orderedIds)
			{
				switch (orderedId.Value)
				{
					case CommentType:
						if (comments.ContainsKey(orderedId.Key))
						{
							result.Add(comments[orderedId.Key]);
						}
						break;
					case StaticTextType:
						if (staticTexts.ContainsKey(orderedId.Key))
						{
							result.Add(staticTexts[orderedId.Key]);
						}
						break;
				}
			}

			return result.ToArray();
		}
	}
}
