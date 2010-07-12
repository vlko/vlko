using System;
using System.Collections.Generic;
using System.Linq;
using Castle.ActiveRecord.Queries;
using GenericRepository;
using NHibernate.Criterion;
using NHibernate.LambdaExtensions;
using NHibernate.SqlCommand;
using vlko.core.ActiveRecord;
using vlko.core.Models.Action.ViewModel;

namespace vlko.core.Models.Action.Implementation
{
	public class CommentData : ICommentData
	{
		/// <summary>
		/// Gets a value indicating whether this <see cref="IAction&lt;T&gt;"/> is initialized.
		/// </summary>
		/// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
		public bool Initialized { get; set; }

		/// <summary>
		/// Initializes queryAction with the specified repository.
		/// </summary>
		/// <param name="initializeContext">The initialize context.</param>
		public void Initialize(InitializeContext<Comment> initializeContext)
		{
			Initialized = true;
		}

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
				if (current.ParentCommentId == Guid.Empty)
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
			// lambda helpers
			CommentTreeViewModel result = null;
			Comment Comment = null;
			Comment ParentComment = null;
			Comment TopComment = null;
			Content Content = null;

			// projection query
			var projection = new ProjectionQueryResult<CommentVersion, CommentTreeViewModel>(

				// add alias and filter
				DetachedCriteria.For<CommentVersion>()
					.CreateAlias<CommentVersion>(commentVersion => commentVersion.Comment, () => Comment)
					.CreateAlias<CommentVersion>(commentVersion => commentVersion.Comment.ParentComment, () => ParentComment, JoinType.LeftOuterJoin)
					.CreateAlias<CommentVersion>(commentVersion => commentVersion.Comment.TopComment, () => TopComment)
					.CreateAlias<CommentVersion>(commentVersion => commentVersion.Comment.Content, () => Content)
					.Add<CommentVersion>(
						commentVersion => commentVersion.Comment.ActualVersion == commentVersion.Version)
					.Add<Comment>(comment => comment.Content.Id == contentId)
					.AddOrder<Comment>(comment => comment.TopComment.CreatedDate, Order.Asc)
					.AddOrder<CommentVersion>(commentVersion => commentVersion.Comment.Level, Order.Asc)
					.AddOrder<CommentVersion>(commentVersion => commentVersion.Comment.CreatedDate, Order.Asc),

				// map projection
				Projections.ProjectionList()
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.Id)
							.As(() => result.Id))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.TopComment.Id)
							.As(() => result.TopCommentId))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.ParentComment.Id)
							.As(() => result.ParentCommentId))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.Name)
							.As(() => result.Name))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.CreatedDate)
							.As(() => result.CreatedDate))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Text)
							.As(() => result.Text))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Version)
							.As(() => result.Version))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.Owner)
							.As(() => result.Owner))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.AnonymousName)
							.As(() => result.AnonymousName))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.ClientIp)
							.As(() => result.ClientIp))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.Level)
							.As(() => result.Level)));

			return projection.ToArray();
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
		private ProjectionQueryResult<CommentVersion, CommentViewModel> GetFlatProjection(Guid contentId)
		{
			// lambda helpers
			CommentViewModel result = null;
			Comment Comment = null;
			Content Content = null;

			// projection query
			return new ProjectionQueryResult<CommentVersion, CommentViewModel>(

				// add alias and filter
				DetachedCriteria.For<CommentVersion>()
					.CreateAlias<CommentVersion>(commentVersion => commentVersion.Comment, () => Comment)
					.CreateAlias<CommentVersion>(commentVersion => commentVersion.Comment.Content, () => Content)
					.Add<CommentVersion>(
						commentVersion => commentVersion.Comment.ActualVersion == commentVersion.Version)
					.Add<Comment>(comment => comment.Content.Id == contentId),

				// map projection
				Projections.ProjectionList()
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.Id)
					     	.As(() => result.Id))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.Name)
					     	.As(() => result.Name))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.CreatedDate)
					     	.As(() => result.CreatedDate))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Text)
					     	.As(() => result.Text))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Version)
					     	.As(() => result.Version))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.Owner)
					     	.As(() => result.Owner))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.AnonymousName)
					     	.As(() => result.AnonymousName))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.ClientIp)
					     	.As(() => result.ClientIp))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.Level)
							.As(() => result.Level)));
		}

		/// <summary>
		/// Gets all for administration.
		/// </summary>
		/// <returns></returns>
		public IQueryResult<CommentForAdminViewModel> GetAllForAdmin()
		{
			// lambda helpers
			CommentForAdminViewModel result = null;
			Comment Comment = null;
			Content Content = null;
			ContentType ContentType = 0; 
			User Owner = null;

			// projection query
			return new ProjectionQueryResult<CommentVersion, CommentForAdminViewModel>(

				// add alias and filter
				DetachedCriteria.For<CommentVersion>()
					.CreateAlias<CommentVersion>(commentVersion => commentVersion.Comment, () => Comment)
					.CreateAlias<CommentVersion>(commentVersion => commentVersion.Comment.Content, () => Content)
					.CreateAlias<CommentVersion>(commentVersion => commentVersion.Comment.Owner, () => Owner, JoinType.LeftOuterJoin)
					.Add<CommentVersion>(commentVersion => commentVersion.Comment.ActualVersion == commentVersion.Version),

				// map projection
				Projections.ProjectionList()
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.Id)
							.As(() => result.Id))
					.Add(LambdaProjection.Property<Comment>(
						comment => comment.Content.Id)
							.As(() => result.ContentId))
					.Add(LambdaProjection.Property<Comment>(
						comment => comment.Content.ContentType)
							.As(() => result.ContentType))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.Name)
							.As(() => result.Name))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.CreatedDate)
							.As(() => result.CreatedDate))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Text)
							.As(() => result.Text))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Version)
							.As(() => result.Version))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.Owner)
							.As(() => result.Owner))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.AnonymousName)
							.As(() => result.AnonymousName))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.ClientIp)
							.As(() => result.ClientIp))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.Level)
							.As(() => result.Level)));
		}

		/// <summary>
		/// Gets the by ids.
		/// </summary>
		/// <param name="ids"></param>
		/// <returns>All comments matching specified ids.</returns>
		public IQueryResult<CommentSearchViewModel> GetByIds(IEnumerable<Guid> ids)
		{
			// lambda helpers
			CommentSearchViewModel result = null;
			Comment Comment = null;
			Content Content = null;
			ContentType ContentType = 0;
			User Owner = null;

			// projection query
			return new ProjectionQueryResult<CommentVersion, CommentSearchViewModel>(

				// add alias and filter
				DetachedCriteria.For<CommentVersion>()
					.CreateAlias<CommentVersion>(commentVersion => commentVersion.Comment, () => Comment)
					.CreateAlias<CommentVersion>(commentVersion => commentVersion.Comment.Content, () => Content)
					.CreateAlias<CommentVersion>(commentVersion => commentVersion.Comment.Owner, () => Owner, JoinType.LeftOuterJoin)
					.Add<CommentVersion>(commentVersion => commentVersion.Comment.ActualVersion == commentVersion.Version)
					.Add(SqlExpression.In<CommentVersion>(commentVersion => commentVersion.Comment.Id, ids.ToArray())),

				// map projection
				Projections.ProjectionList()
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.Id)
							.As(() => result.Id))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.Name)
							.As(() => result.Name))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.CreatedDate)
							.As(() => result.CreatedDate))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Text)
							.As(() => result.Text))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Version)
							.As(() => result.Version))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.Owner)
							.As(() => result.Owner))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.AnonymousName)
							.As(() => result.AnonymousName))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.ClientIp)
							.As(() => result.ClientIp))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.Level)
							.As(() => result.Level))
					.Add(LambdaProjection.Property<CommentVersion>(
						commentVersion => commentVersion.Comment.Content)
							.As(() => result.Content)));
		}
	}
}