using System;
using vlko.BlogModule.Roots;
using vlko.core.Repository;

namespace vlko.BlogModule.Action
{
	public interface ITimeline : IAction<Content>
	{
		/// <summary>
		/// Gets all items in timeline.
		/// </summary>
		/// <returns>Time line query results.</returns>
		IQueryResult<object> GetAll(DateTime? pivotDate = null);
	}
}
