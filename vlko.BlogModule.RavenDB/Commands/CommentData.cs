using System;
using System.Collections.Generic;
using System.Linq;
using vlko.BlogModule.Commands;
using vlko.BlogModule.Commands.ViewModel;
using vlko.BlogModule.RavenDB.Indexes;
using vlko.BlogModule.Roots;
using vlko.core.RavenDB.Repository;
using vlko.core.Repository;

namespace vlko.BlogModule.RavenDB.Commands
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
			var dictionary = new Dictionary<Guid, CommentTreeViewModel>();

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
					dictionary[current.ParentCommentId.Value].AddChildNode(current);
				}
				dictionary.Add(current.Id, current);
			}

			dictionary.Clear();
			return result.OrderBy(item => item.CreatedDate);
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
			return SessionFactory<Comment>.IndexQuery<CommentSortIndex>()
				.Where(comment => comment.Content.Id == contentId)
				.OrderBy(comment => comment.Level)
				.OrderBy(comment => comment.CreatedDate)
				.ToArray()
				.Select(comment =>
				        	{
								var commentVersion = comment.CommentVersions.First(version => version.Version == comment.ActualVersion);
				        		return new CommentTreeViewModel
				        		       	{
				        		       		Id = comment.Id,
				        		       		TopCommentId = comment.TopComment.Id,
											ParentCommentId = comment.ParentComment != null ? (Guid?)comment.ParentComment.Id : null,
				        		       		Name = comment.Name,
				        		       		CreatedDate = comment.CreatedDate,
				        		       		Text = commentVersion.Text,
				        		       		Version = commentVersion.Version,
				        		       		Owner = comment.Owner,
				        		       		AnonymousName = comment.AnonymousName,
				        		       		ClientIp = commentVersion.ClientIp,
				        		       		Level = comment.Level
				        		       	};
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
		private ProjectionQueryResult<Comment, CommentViewModel> GetFlatProjection(Guid contentId)
		{
			return
				new ProjectionQueryResult<Comment, CommentViewModel>(
					SessionFactory<Comment>.IndexQuery<CommentSortIndex>()
						.Where(comment => comment.Content.Id == contentId),
					comment =>
						{
							var commentVersion = comment.CommentVersions.First(version => version.Version == comment.ActualVersion);
							return new CommentViewModel
							       	{
							       		Id = comment.Id,
							       		Name = comment.Name,
							       		CreatedDate = comment.CreatedDate,
							       		Text = commentVersion.Text,
							       		Version = commentVersion.Version,
							       		Owner = comment.Owner,
							       		AnonymousName = comment.AnonymousName,
							       		ClientIp = commentVersion.ClientIp,
							       		Level = comment.Level
							       	};
						}
					)
					.AddSortMapping(comment => comment.Level, comment => comment.Level)
					.AddSortMapping(comment => comment.CreatedDate, comment => comment.CreatedDate);
		}

		/// <summary>
		/// Gets all for administration.
		/// </summary>
		/// <returns></returns>
		public IQueryResult<CommentForAdminViewModel> GetAllForAdmin()
		{
			return new ProjectionQueryResult<Comment, CommentForAdminViewModel>(
				SessionFactory<Comment>.IndexQuery<CommentSortIndex>(),
				comment =>
					{
						var commentVersion = comment.CommentVersions.First(version => version.Version == comment.ActualVersion);
						return new CommentForAdminViewModel
						       	{
						       		Id = comment.Id,
						       		Name = comment.Name,
						       		CreatedDate = comment.CreatedDate,
						       		Text = commentVersion.Text,
						       		Version = commentVersion.Version,
						       		Owner = comment.Owner,
						       		AnonymousName = comment.AnonymousName,
						       		ClientIp = commentVersion.ClientIp,
						       		Level = comment.Level,
						       		ContentId = comment.Content.Id,
						       		ContentType = comment.Content.ContentType
						       	};
					}
				)
				.AddSortMapping(comment => comment.Level, comment => comment.Level)
				.AddSortMapping(comment => comment.CreatedDate, comment => comment.CreatedDate);
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

			return new ProjectionQueryResult<Comment, CommentSearchViewModel>(
				SessionFactory<Comment>.LoadMore(idArray).AsQueryable(),
				comment =>
					{
						var commentVersion = comment.CommentVersions.First(version => version.Version == comment.ActualVersion);
						return new CommentSearchViewModel
						       	{
						       		Id = comment.Id,
						       		Name = comment.Name,
						       		CreatedDate = comment.CreatedDate,
						       		Text = commentVersion.Text,
						       		Version = commentVersion.Version,
						       		Owner = comment.Owner,
						       		AnonymousName = comment.AnonymousName,
						       		ClientIp = commentVersion.ClientIp,
						       		Level = comment.Level,
						       		Content = comment.Content
						       	};
					})
				.AddSortMapping(comment => comment.Level, comment => comment.Level)
				.AddSortMapping(comment => comment.CreatedDate, comment => comment.CreatedDate);
		}
	}
}