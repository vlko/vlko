using System;
using vlko.BlogModule.Action.CRUDModel;
using vlko.BlogModule.Action.ViewModel;
using vlko.BlogModule.Roots;
using vlko.core.Repository;
using vlko.core.Repository.Exceptions;

namespace vlko.BlogModule.Action
{
	public interface IRssFeedAction : IAction<RssFeed>
	{
		/// <summary>
		/// Creates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Created item.</returns>
		RssFeedCRUDModel Create(RssFeedCRUDModel item);

		/// <summary>
		/// Finds the by primary key.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>
		/// Item matching id or exception if not exists.
		/// </returns>
		/// <exception cref="NotFoundException">If matching id was not found.</exception>
		RssFeedCRUDModel FindByPk(Guid id);

		/// <summary>
		/// Finds item by PK.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="throwOnNotFound">if set to <c>true</c> [throw exception on not found].</param>
		/// <returns>Item matching id or null/exception if not exists.</returns>
		RssFeedCRUDModel FindByPk(Guid id, bool throwOnNotFound);

		/// <summary>
		/// Updates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Updated item.</returns>
		RssFeedCRUDModel Update(RssFeedCRUDModel item);

		/// <summary>
		/// Deletes the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		void Delete(RssFeedCRUDModel item);

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns>Query result with all rss feeds.</returns>
		IQueryResult<RssFeedViewModel> GetAll();

		/// <summary>
		/// Gets the feed to process.
		/// </summary>
		/// <returns>Feeds to process.</returns>
		RssFeedCRUDModel[] GetFeedToProcess();
	}
}
