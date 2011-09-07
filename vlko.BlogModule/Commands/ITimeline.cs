using System;
using System.ComponentModel.Composition;
using vlko.BlogModule.Roots;
using vlko.core.Repository;

namespace vlko.BlogModule.Commands
{
	[InheritedExport]
	public interface ITimeline : ICommandGroup<Content>
	{
		/// <summary>
		/// Gets all items in timeline.
		/// </summary>
		/// <returns>Time line query results.</returns>
		IQueryResult<object> GetAll(DateTime? pivotDate = null);
	}
}
