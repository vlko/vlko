using System;
using System.Collections.Generic;
using vlko.model.Action.ViewModel;
using vlko.core.Repository;
using vlko.model.Roots;

namespace vlko.model.Action
{
	public interface ICommentData : IAction<Comment>
	{
		/// <summary>
		/// Gets the comment tree.
		/// </summary>
		/// <param name="contentId">The content id.</param>
		/// <returns>Root comments for content in tree format.</returns>
		IEnumerable<CommentTreeViewModel> GetCommentTree(Guid contentId);

		/// <summary>
		/// Gets all ordered by date desc.
		/// </summary>
		/// <param name="contentId">The content id.</param>
		/// <returns>Get all comments in flat format ordered by date desc</returns>
		IQueryResult<CommentViewModel> GetAllByDateDesc(Guid contentId);

		/// <summary>
		/// Gets all ordered by date.
		/// </summary>
		/// <param name="contentId">The content id.</param>
		/// <returns>Get all comments in flat format ordered by date</returns>
		IQueryResult<CommentViewModel> GetAllByDate(Guid contentId);

		/// <summary>
		/// Gets all for administration.
		/// </summary>
		/// <returns>Get all comments in flat format for all contents.</returns>
		IQueryResult<CommentForAdminViewModel> GetAllForAdmin();

		/// <summary>
		/// Gets the by ids.
		/// </summary>
		/// <param name="ids"></param>
		/// <returns>All comments matching specified ids.</returns>
		IQueryResult<CommentSearchViewModel> GetByIds(IEnumerable<Guid> ids);
	}
}
