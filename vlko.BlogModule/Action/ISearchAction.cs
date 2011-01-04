using vlko.BlogModule.Action.CRUDModel;
using vlko.BlogModule.Roots;
using vlko.BlogModule.Search;
using vlko.core.Repository;

namespace vlko.BlogModule.Action
{
	public interface ISearchAction : IAction<SearchRoot>
	{
		/// <summary>
		/// Indexes the comment.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="comment">The comment.</param>
		void IndexComment(ITransaction transaction, CommentCRUDModel comment);

		/// <summary>
		/// Indexes the twitter status.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="status">The status.</param>
		void IndexTwitterStatus(ITransaction transaction, TwitterStatus status);

		/// <summary>
		/// Indexes the static text.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="staticText">The static text.</param>
		void IndexStaticText(ITransaction transaction, StaticTextCRUDModel staticText);

		/// <summary>
		/// Indexes the rss item.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="rssItem">The RSS item.</param>
		void IndexRssItem(ITransaction transaction, RssItemCRUDModel rssItem);

		/// <summary>
		/// Deletes from index.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="id">The id.</param>
		void DeleteFromIndex(ITransaction transaction, string id);

		/// <summary>
		/// Searches for data.
		/// </summary>
		/// <param name="session">The session.</param>
		/// <param name="queryString">The query string.</param>
		/// <returns>Search result wrapper.</returns>
		SearchResult Search(IUnitOfWork session, string queryString);

		/// <summary>
		/// Searches for data sorted by date.
		/// </summary>
		/// <param name="session">The session.</param>
		/// <param name="queryString">The query string.</param>
		/// <returns>Search result wrapper.</returns>
		SearchResult SearchByDate(IUnitOfWork session, string queryString);
	}
}
