using System;
using System.Collections.Generic;
using vlko.model.Repository;
using vlko.model.ViewModel;

namespace vlko.model.Action
{
	public interface IStaticTextData : IAction<StaticText>
	{
		/// <summary>
		/// Gets all.
		/// </summary>
		/// <param name="pivotDate">The pivot date (only data younger, that this date).</param>
		/// <returns>Query result.</returns>
		IQueryResult<StaticTextViewModel> GetAll(DateTime? pivotDate = null);

		/// <summary>
		/// Gets the deleted.
		/// </summary>
		/// <returns>Query result.</returns>
		IQueryResult<StaticTextViewModel> GetDeleted();

		/// <summary>
		/// Gets the specified id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="pivotDate">The pivot date (only data younger, that this date).</param>
		/// <returns>Data model for specific id.</returns>
		StaticTextWithFullTextViewModel Get(Guid id, DateTime? pivotDate = null);

		/// <summary>
		/// Gets the specified nice URL.
		/// </summary>
		/// <param name="friendlyUrl">The friendly URL.</param>
		/// <param name="pivotDate">The pivot date (only data younger, that this date).</param>
		/// <returns>Data model for</returns>
		StaticTextWithFullTextViewModel Get(string friendlyUrl, DateTime? pivotDate = null);

		/// <summary>
		/// Gets the by ids.
		/// </summary>
		/// <param name="ids"></param>
		/// <returns>All static text matching specified ids.</returns>
		IQueryResult<StaticTextViewModel> GetByIds(IEnumerable<Guid> ids);
	}
}
