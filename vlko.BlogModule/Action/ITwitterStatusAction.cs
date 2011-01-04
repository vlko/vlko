using System;
using System.Collections.Generic;
using vlko.BlogModule.Roots;
using vlko.core.Repository;

namespace vlko.BlogModule.Action
{
	public interface ITwitterStatusAction : IAction<TwitterStatus>
	{

		/// <summary>
		/// Gets the by twitter ids.
		/// </summary>
		/// <param name="twitterIds">The twitter ids.</param>
		/// <returns>Only twitter statuses with matching id.</returns>
		TwitterStatus[] GetByTwitterIds(IEnumerable<long> twitterIds);

		/// <summary>
		/// Gets the by ids.
		/// </summary>
		/// <param name="ids">The ids.</param>
		/// <returns>
		/// Only twitter statuses matching specified ids.
		/// </returns>
		IQueryResult<TwitterStatus> GetByIds(IEnumerable<Guid> ids);

		/// <summary>
		/// Creates the status.
		/// </summary>
		/// <param name="newStatus">The new status.</param>
		/// <returns>Created twitter status.</returns>
		TwitterStatus CreateStatus(TwitterStatus newStatus);

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns>Query result with list of twitter statuses.</returns>
		IQueryResult<TwitterStatus> GetAll();
	}
}
