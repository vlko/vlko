using System;
using System.Collections.Generic;
using System.Linq;
using vlko.BlogModule.Commands;
using vlko.BlogModule.Commands.ViewModel;
using vlko.BlogModule.Roots;
using vlko.core.NH.Repository;
using vlko.core.Repository;

namespace vlko.BlogModule.NH.Commands
{
	public class CommentData : CommandGroup<Comment>, ICommentData
	{
		/// <summary>
		/// Gets the comment tree.
		/// </summary>
		/// <param name="contentId">The content id.</param>
		/// <returns>
		/// Root comments for content in tree format.
		/// </returns>
		public IEnumerable<CommentTreeViewModel> GetCommentTree(Guid contentId)
		{
			var result = new List<CommentTreeViewModel>();

			var dataTreeEnumerator = GetTreeData(contentId);

			CommentTreeViewModel currentTop = null;
			foreach (var current in dataTreeEnumerator)
			{
				if (current.ParentCommentId == null)
				{
					currentTop = current;
					result.Add(currentTop);
				}
				else
				{
					if (!AddToTree(currentTop, current, current.Level - 1))
					{
						throw new Exception("Inconsistent data for comment tree.");
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Adds to tree.
		/// </summary>
		/// <param name="currentNode">The current node.</param>
		/// <param name="current">The current.</param>
		/// <param name="level">The level.</param>
		/// <returns>True if success, otherwise false.</returns>
		private bool AddToTree(CommentTreeViewModel currentNode, CommentTreeViewModel current, int level)
		{
			if (level == 0)
			{
				if (currentNode.Id == current.ParentCommentId)
				{
					currentNode.AddChildNode(current);
					return true;
				}
				return false;
			}
			foreach (var childNode in currentNode.ChildNodes)
			{
				if (AddToTree(childNode, current, level - 1))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Gets the tree data.
		/// </summary>
		/// <param name="contentId">The content id.</param>
		/// <returns>Raw tree data.</returns>
		private IEnumerable<CommentTreeViewModel> GetTreeData(Guid contentId)
		{
			return SessionFactory<CommentVersion>.Queryable
				.Where(commentVersion =>
				       commentVersion.Comment.ActualVersion == commentVersion.Version
				       && commentVersion.Comment.Content.Id == contentId)
				.OrderBy(commentVersion => commentVersion.Comment.TopComment.CreatedDate)
				.OrderBy(commentVersion => commentVersion.Comment.Level)
				.OrderBy(commentVersion => commentVersion.Comment.CreatedDate)
				.Select(commentVersion => new CommentTreeViewModel
				                          	{
				                          		Id = commentVersion.Comment.Id,
				                          		TopCommentId = commentVersion.Comment.TopComment.Id,
				                          		ParentCommentId = commentVersion.Comment.ParentComment.Id,
				                          		Name = commentVersion.Comment.Name,
				                          		CreatedDate = commentVersion.Comment.CreatedDate,
				                          		Text = commentVersion.Text,
				                          		Version = commentVersion.Version,
				                          		Owner = commentVersion.Comment.Owner,
				                          		AnonymousName = commentVersion.Comment.AnonymousName,
				                          		ClientIp = commentVersion.ClientIp,
				                          		Level = commentVersion.Comment.Level
				                          	}).ToArray();
		}

		/// <summary>
		/// Gets all ordered by date desc.
		/// </summary>
		/// <param name="contentId">The content id.</param>
		/// <returns>
		/// Get all comments in flat format ordered by date desc
		/// </returns>
		public IQueryResult<CommentViewModel> GetAllByDateDesc(Guid contentId)
		{
			var projection = GetFlatProjection(contentId);

			return projection
				.OrderByDescending(comment => comment.CreatedDate)
				.OrderByDescending(comment => comment.Level);
		}

		/// <summary>
		/// Gets all ordered by date.
		/// </summary>
		/// <param name="contentId">The content id.</param>
		/// <returns>
		/// Get all comments in flat format ordered by date
		/// </returns>
		public IQueryResult<CommentViewModel> GetAllByDate(Guid contentId)
		{
			var projection = GetFlatProjection(contentId);

			return projection
				.OrderBy(comment => comment.CreatedDate)
				.OrderBy(comment => comment.Level);
		}

		/// <summary>
		/// Gets the flat projection.
		/// </summary>
		/// <param name="contentId">The content id.</param>
		/// <returns>Projection for flat data.</returns>
		private QueryLinqResult<CommentViewModel> GetFlatProjection(Guid contentId)
		{
			return new QueryLinqResult<CommentViewModel>(
				SessionFactory<CommentVersion>.Queryable
					.Where(commentVersion =>
					       commentVersion.Comment.ActualVersion == commentVersion.Version
					       && commentVersion.Comment.Content.Id == contentId)
					.Select(commentVersion => new CommentViewModel
					                          	{
					                          		Id = commentVersion.Comment.Id,
					                          		Name = commentVersion.Comment.Name,
					                          		CreatedDate = commentVersion.Comment.CreatedDate,
					                          		Text = commentVersion.Text,
					                          		Version = commentVersion.Version,
					                          		Owner = commentVersion.Comment.Owner,
					                          		AnonymousName = commentVersion.Comment.AnonymousName,
					                          		ClientIp = commentVersion.ClientIp,
					                          		Level = commentVersion.Comment.Level
					                          	}));
		}

		/// <summary>
		/// Gets all for administration.
		/// </summary>
		/// <returns></returns>
		public IQueryResult<CommentForAdminViewModel> GetAllForAdmin()
		{
			return new QueryLinqResult<CommentForAdminViewModel>(
				SessionFactory<CommentVersion>.Queryable
					.Where(commentVersion => commentVersion.Comment.ActualVersion == commentVersion.Version)
					.Select(commentVersion => new CommentForAdminViewModel
					                          	{
					                          		Id = commentVersion.Comment.Id,
					                          		Name = commentVersion.Comment.Name,
					                          		CreatedDate = commentVersion.Comment.CreatedDate,
					                          		Text = commentVersion.Text,
					                          		Version = commentVersion.Version,
					                          		Owner = commentVersion.Comment.Owner,
					                          		AnonymousName = commentVersion.Comment.AnonymousName,
					                          		ClientIp = commentVersion.ClientIp,
					                          		Level = commentVersion.Comment.Level,
					                          		ContentId = commentVersion.Comment.Content.Id,
					                          		ContentType = commentVersion.Comment.Content.ContentType
					                          	}));
		}

		/// <summary>
		/// Gets the by ids.
		/// </summary>
		/// <param name="ids"></param>
		/// <returns>All comments matching specified ids.</returns>
		public IQueryResult<CommentSearchViewModel> GetByIds(IEnumerable<Guid> ids)
		{
			var idArray = ids.ToArray();

			if (idArray.Length == 0)
			{
				return new EmptyQueryResult<CommentSearchViewModel>();
			}

			return new QueryLinqResult<CommentSearchViewModel>(
				SessionFactory<CommentVersion>.Queryable
					.Where(commentVersion => commentVersion.Comment.ActualVersion == commentVersion.Version
					                         && idArray.Contains(commentVersion.Comment.Id))
					.Select(commentVersion => new CommentSearchViewModel
					                          	{
					                          		Id = commentVersion.Comment.Id,
					                          		Name = commentVersion.Comment.Name,
					                          		CreatedDate = commentVersion.Comment.CreatedDate,
					                          		Text = commentVersion.Text,
					                          		Version = commentVersion.Version,
					                          		Owner = commentVersion.Comment.Owner,
					                          		AnonymousName = commentVersion.Comment.AnonymousName,
					                          		ClientIp = commentVersion.ClientIp,
					                          		Level = commentVersion.Comment.Level,
					                          		Content = commentVersion.Comment.Content
					                          	}));
		}
	}
}