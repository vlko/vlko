using System;
using System.Collections.Generic;
using Castle.ActiveRecord.Queries;
using GenericRepository;
using NHibernate.Criterion;
using NHibernate.LambdaExtensions;
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
			throw new NotImplementedException();
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
	}
}