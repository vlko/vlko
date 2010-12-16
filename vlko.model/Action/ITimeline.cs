using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vlko.core.Repository;
using vlko.model.Roots;

namespace vlko.model.Action
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
