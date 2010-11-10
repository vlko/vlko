using System;
using System.Collections.Generic;
using System.Text;
using vlko.model.Action.CRUDModel;
using vlko.model.Action.ViewModel;
using vlko.model.Repository;

namespace vlko.model.Action
{
	public interface IRssItemAction : IAction<RssItem>
	{

		/// <summary>
		/// Gets by the feed ids.
		/// </summary>
		/// <param name="feedIds">The feed ids.</param>
		/// <returns>Feed items with matching id.</returns>
		RssItemCRUDModel[] GetByFeedIds(IEnumerable<string> feedIds);

		/// <summary>
		/// Gets the by ids.
		/// </summary>
		/// <param name="feedIds">The feed ids.</param>
		/// <returns>Only Feed items matching specified ids.</returns>
		IQueryResult<RssItemViewModel> GetByIds(IEnumerable<string> feedIds);

		/// <summary>
		/// Saves the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Saved item.</returns>
		RssItemCRUDModel Save(RssItemCRUDModel item);

		/// <summary>
		/// Deletes the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		void Delete(RssItemCRUDModel item);

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns>Query result with all rss feeds.</returns>
		IQueryResult<RssItemViewModel> GetAll();
	}
}
