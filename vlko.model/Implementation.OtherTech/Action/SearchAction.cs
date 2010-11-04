using System;
using System.Text.RegularExpressions;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Microsoft.Security.Application;
using vlko.model.Action;
using vlko.model.Action.CRUDModel;
using vlko.model.Repository;
using vlko.model.Search;

namespace vlko.model.Implementation.OtherTech.Action
{
	public class SearchAction :  BaseAction<SearchRoot>, ISearchAction
	{
		private const int MaximalSearchDepth = 1000;
		/// <summary>
		/// Indexes the comment.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="comment">The comment.</param>
		public void IndexComment(ITransaction transaction, CommentCRUDModel comment)
		{
			SearchUpdateContext tranContext = transaction.TransactionContext as SearchUpdateContext;
			if (tranContext == null)
			{
				throw new Exception("SearchUpdateContext not part of ITransaction!");
			}

			Document doc = new Document();
			doc.Add(new Field(SearchResult.IdField, comment.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field(SearchResult.TypeField, SearchResult.CommentType, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("Title", comment.Name, Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field("Text", RemoveTags(comment.Text), Field.Store.NO, Field.Index.ANALYZED));
			doc.Add(new Field("Published", DateTools.DateToString(comment.ChangeDate, DateTools.Resolution.SECOND), Field.Store.NO, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("Date", DateTools.DateToString(comment.ChangeDate, DateTools.Resolution.SECOND), Field.Store.NO, Field.Index.NOT_ANALYZED));
			var user = comment.ChangeUser != null ? comment.ChangeUser.Name : comment.AnonymousName;
			doc.Add(new Field("User", user, Field.Store.NO, Field.Index.ANALYZED));
			
			tranContext.IndexWriter.AddDocument(doc);
		}

		/// <summary>
		/// Indexes the twitter status.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="status">The status.</param>
		public void IndexTwitterStatus(ITransaction transaction, TwitterStatus status)
		{
			SearchUpdateContext tranContext = transaction.TransactionContext as SearchUpdateContext;
			if (tranContext == null)
			{
				throw new Exception("SearchUpdateContext not part of ITransaction!");
			}

			Document doc = new Document();
			doc.Add(new Field(SearchResult.IdField, status.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field(SearchResult.TypeField, SearchResult.TwitterStatusType, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("Title", string.Empty, Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field("Text", RemoveTags(status.Text), Field.Store.NO, Field.Index.ANALYZED));
			doc.Add(new Field("Published", DateTools.DateToString(status.CreatedDate, DateTools.Resolution.SECOND), Field.Store.NO, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("Date", DateTools.DateToString(status.CreatedDate, DateTools.Resolution.SECOND), Field.Store.NO, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("User", status.User +  " " + status.RetweetUser, Field.Store.NO, Field.Index.ANALYZED));

			tranContext.IndexWriter.AddDocument(doc);
		}

		/// <summary>
		/// Indexes the static text.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="staticText">The static text.</param>
		public void IndexStaticText(ITransaction transaction, StaticTextCRUDModel staticText)
		{
			SearchUpdateContext tranContext = transaction.TransactionContext as SearchUpdateContext;
			if (tranContext == null)
			{
				throw new Exception("SearchUpdateContext not part of ITransaction!");
			}

			Document doc = new Document();
			doc.Add(new Field(SearchResult.IdField, staticText.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field(SearchResult.TypeField, SearchResult.StaticTextType, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("Title", staticText.Title, Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field("Text", RemoveTags(staticText.Text), Field.Store.NO, Field.Index.ANALYZED));
			doc.Add(new Field("Published", DateTools.DateToString(staticText.PublishDate, DateTools.Resolution.SECOND), Field.Store.NO, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("Date", DateTools.DateToString(staticText.ChangeDate, DateTools.Resolution.SECOND), Field.Store.NO, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("User", staticText.Creator.Name, Field.Store.NO, Field.Index.ANALYZED));

			doc.SetBoost(5F);

			tranContext.IndexWriter.AddDocument(doc);
		}

		/// <summary>
		/// Deletes from index.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="id">The id.</param>
		public void DeleteFromIndex(ITransaction transaction, Guid id)
		{
			SearchUpdateContext tranContext = transaction.TransactionContext as SearchUpdateContext;
			if (tranContext == null)
			{
				throw new Exception("SearchUpdateContext not part of ITransaction!");
			}
			tranContext.IndexWriter.DeleteDocuments(new TermQuery(new Term(SearchResult.IdField, id.ToString())));
		}


		/// <summary>
		/// Searches for data.
		/// </summary>
		/// <param name="session">The session.</param>
		/// <param name="queryString">The query string.</param>
		/// <returns>Search result wrapper.</returns>
		public SearchResult Search(IUnitOfWork session, string queryString)
		{
			SearchContext searchContext = session.UnitOfWorkContext as SearchContext;
			if (searchContext == null)
			{
				throw new Exception("SearchContext not part of IUnitOfWork!");
			}
			var queryParser = searchContext.GetQueryParser(new[] { "Text", "Title", "User"});
			var query = queryParser.Parse(queryString);

			Filter filter = RangeFilter.Less("Published", DateTools.DateToString(DateTime.Now.AddSeconds(1), DateTools.Resolution.SECOND));


			var topDocs = searchContext.IndexSearcher.Search(query, filter, MaximalSearchDepth);

			return new SearchResult(topDocs, searchContext.IndexSearcher);
		}

		/// <summary>
		/// Searches for data sorted by date.
		/// </summary>
		/// <param name="session">The session.</param>
		/// <param name="queryString">The query string.</param>
		/// <returns>Search result wrapper.</returns>
		public SearchResult SearchByDate(IUnitOfWork session, string queryString)
		{
			SearchContext searchContext = session.UnitOfWorkContext as SearchContext;
			if (searchContext == null)
			{
				throw new Exception("SearchContext not part of IUnitOfWork!");
			}
			var queryParser = searchContext.GetQueryParser(new[] { "Text", "Title", "User" });
			var query = queryParser.Parse(queryString);

			Filter filter = RangeFilter.Less("Published", DateTools.DateToString(DateTime.Now.AddSeconds(1), DateTools.Resolution.SECOND));

			var topDocs = searchContext.IndexSearcher.Search(query, filter, MaximalSearchDepth, new Sort("Date", true));

			return new SearchResult(topDocs, searchContext.IndexSearcher);
		}

		/// <summary>
		/// Removes the tags.
		/// </summary>
		/// <param name="htmlInput">The HTML input.</param>
		/// <returns>Html input with removed tags.</returns>
		public static string RemoveTags(string htmlInput)
		{
			string result = AntiXss.GetSafeHtmlFragment(htmlInput).Trim();
			return Regex.Replace(result, @"<(.|\n)*?>", string.Empty);
		}
	}
}