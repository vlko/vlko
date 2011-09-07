using System;
using Raven.Client.Linq;
using vlko.BlogModule.Commands;
using vlko.BlogModule.RavenDB.Commands.QueryHelpers;
using vlko.BlogModule.RavenDB.Indexes;
using vlko.BlogModule.Roots;
using vlko.core.RavenDB.Repository;
using vlko.core.Repository;

namespace vlko.BlogModule.RavenDB.Commands
{
	public class Timeline : CommandGroup<Content>, ITimeline
	{

		/// <summary>
		/// Gets all items in timeline.
		/// </summary>
		/// <returns>Time line query results.</returns>
		public IQueryResult<object> GetAll(DateTime? pivotDate = null)
		{
			// skip hidden items
			var timelineItems = SessionFactory<Content>.IndexQuery<TimelineIndex>()
				.Where(content => content.Hidden == false);
			// apply pivot date if specified
			if (pivotDate.HasValue)
			{
				timelineItems = timelineItems.Where(content => content.PublishDate <= pivotDate);
			}

			// sort descending by publish date and transform just to id
			return new TimelineResult(timelineItems
										.OrderByDescending(content => content.PublishDate)
										);
		}
	}
}