using System;
using System.Linq;
using vlko.BlogModule.Commands;
using vlko.BlogModule.NH.Commands.QueryHelpers;
using vlko.BlogModule.Roots;
using vlko.core.NH.Repository;
using vlko.core.Repository;

namespace vlko.BlogModule.NH.Commands
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
			var timelineItems = SessionFactory<Content>.Queryable.Where(content => content.Hidden == false);
			// apply pivot date if specified
			if (pivotDate.HasValue)
			{
				timelineItems = timelineItems.Where(content => content.PublishDate <= pivotDate);
			}
			
			// sort descending by publish date and transform just to id
			return new TimelineResult(timelineItems
			                          	.OrderByDescending(content => content.PublishDate)
                                        .Where(content => content.ContentType == ContentType.TwitterStatus)
			                          	.Select(content => new TimelineData
			                          	                   	{
			                          	                   		Id = content.Id,
			                          	                   		ContentType = content.ContentType
			                          	                   	}),
                                      timelineItems
                                        .OrderByDescending(content => content.PublishDate)
                                        .Where(content => content.ContentType != ContentType.TwitterStatus)
                                        .Select(content => new TimelineData
                                        {
                                            Id = content.Id,
                                            ContentType = content.ContentType
                                        }));
		}
	}
}