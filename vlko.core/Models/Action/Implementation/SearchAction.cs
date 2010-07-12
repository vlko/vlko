using System;
using GenericRepository;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using vlko.core.Models.Action.ActionModel;
using vlko.core.Search;
using vlko.core.Tools;

namespace vlko.core.Models.Action.Implementation
{
	public class SearchAction :  BaseAction<Search>, ISearchAction
	{

		/// <summary>
		/// Indexes the comment.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="comment">The comment.</param>
		public void IndexComment(ITransaction transaction, CommentActionModel comment)
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
			doc.Add(new Field("Text", HtmlManipulation.RemoveTags(comment.Text), Field.Store.NO, Field.Index.ANALYZED));
			doc.Add(new Field("Published", DateField.DateToString(comment.ChangeDate), Field.Store.NO, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("Date", DateField.DateToString(comment.ChangeDate), Field.Store.NO, Field.Index.NOT_ANALYZED));
			var user = comment.ChangeUser != null ? comment.ChangeUser.Name : comment.AnonymousName;
			doc.Add(new Field("User", user, Field.Store.NO, Field.Index.ANALYZED));
			
			tranContext.IndexWriter.AddDocument(doc);
		}

		/// <summary>
		/// Indexes the static text.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="staticText">The static text.</param>
		public void IndexStaticText(ITransaction transaction, StaticTextActionModel staticText)
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
			doc.Add(new Field("Text", HtmlManipulation.RemoveTags(staticText.Text), Field.Store.NO, Field.Index.TOKENIZED));
			doc.Add(new Field("Published", DateField.DateToString(staticText.PublishDate), Field.Store.NO, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("Date", DateField.DateToString(staticText.ChangeDate), Field.Store.NO, Field.Index.NOT_ANALYZED));
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

			Filter filter = RangeFilter.Less("Published", DateField.DateToString(DateTime.Now.AddSeconds(1)));


			var hits = searchContext.IndexSearcher.Search(query, filter);

			return new SearchResult(hits);
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

			Filter filter = RangeFilter.Less("Published", DateField.DateToString(DateTime.Now.AddSeconds(1)));

			var hits = searchContext.IndexSearcher.Search(query, filter, new Sort("Date", true));

			return new SearchResult(hits);
		}
	}
}