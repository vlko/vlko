using System;
using System.Collections.Generic;
using System.Linq;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using vlko.model.Action;
using vlko.model.Repository;

namespace vlko.model.Implementation.NH.Action
{
	public class TwitterStatusAction : BaseAction<TwitterStatus>, ITwitterStatusAction
	{

		/// <summary>
		/// Gets the by twitter ids.
		/// </summary>
		/// <param name="twitterIds">The twitter ids.</param>
		/// <returns>Only twitter statuses with matching id.</returns>
		public TwitterStatus[] GetByTwitterIds(IEnumerable<long> twitterIds)
		{
			var twitterIdsArray = twitterIds.ToArray();

			if (twitterIdsArray.Length == 0)
			{
				return new TwitterStatus[] {};
			}

			return ActiveRecordLinqBase<TwitterStatus>.Queryable
				.Where(status => twitterIdsArray.Contains(status.TwitterId))
				.ToArray();
		}

		/// <summary>
		/// Gets the by ids.
		/// </summary>
		/// <param name="ids">The ids.</param>
		/// <returns>
		/// Only twitter statuses matching specified ids.
		/// </returns>
		public IQueryResult<TwitterStatus> GetByIds(IEnumerable<Guid> ids)
		{
			var idArray = ids.ToArray();

			if (idArray.Length == 0)
			{
				return new EmptyQueryResult<TwitterStatus>();
			}

			return new QueryLinqResult<TwitterStatus>(
				ActiveRecordLinqBase<TwitterStatus>.Queryable
				.Where(status => idArray.Contains(status.Id)));
		}

		/// <summary>
		/// Creates the status.
		/// </summary>
		/// <param name="newStatus">The new status.</param>
		/// <returns>Created twitter status.</returns>
		public TwitterStatus CreateStatus(TwitterStatus newStatus)
		{
			ActiveRecordMediator<TwitterStatus>.Create(newStatus);
			return newStatus;
		}

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns>
		/// Query result with list of twitter statuses.
		/// </returns>
		public IQueryResult<TwitterStatus> GetAll()
		{
			return new QueryLinqResult<TwitterStatus>(
				ActiveRecordLinqBase<TwitterStatus>.Queryable);
		}
	}
}