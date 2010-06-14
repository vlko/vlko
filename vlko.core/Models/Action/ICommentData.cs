using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericRepository;
using vlko.core.Models.Action.ViewModel;

namespace vlko.core.Models.Action
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
	}
}
