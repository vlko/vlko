using System;
using System.Collections.Generic;
using System.Linq;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Queries;
using GenericRepository;
using NHibernate.Criterion;
using NHibernate.LambdaExtensions;
using vlko.core.Models.Action.ActionModel;
using NotFoundException = GenericRepository.Exceptions.NotFoundException;

namespace vlko.core.Models.Action.Implementation
{
    public class CommentCrud : ICommentCrud
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
        /// Creates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Created item.</returns>
        public CommentActionModel Create(CommentActionModel item)
        {
            var comment = new Comment()
                              {
                                  Name = item.Name,
                                  Content = item.Content,
                                  Owner = item.ChangeUser,
                                  AnonymousName = item.AnonymousName,
                                  CreatedDate = item.ChangeDate,
                                  ActualVersion = 0,
                                  CommentVersions = new List<CommentVersion>()
                                                        {
                                                            new CommentVersion
                                                                {
                                                                    CreatedDate = item.ChangeDate,
                                                                    CreatedBy = item.ChangeUser,
                                                                    ClientIp = item.ClientIp,
                                                                    UserAgent = item.UserAgent,
                                                                    Text = item.Text,
                                                                    Version = 0
                                                                }
                                                        }
                              };

            if (item.ParentId == null)
            {
                comment.Level = 0;
                comment.TopComment = comment;
                comment.ParentVersion = comment.ActualVersion;
            }
            else
            {
                var parentComment = ActiveRecordMediator<Comment>.FindByPrimaryKey(item.ParentId.Value);
                comment.Level = parentComment.Level + 1;
                comment.ParentComment = parentComment;
                comment.TopComment = parentComment.TopComment;
                comment.ParentVersion = parentComment.ActualVersion;
            }

            ActiveRecordMediator<Comment>.Create(comment);

            // assign id
            item.Id = comment.Id;

            return item;
        }

        /// <summary>
        /// Finds the by primary key.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// Item matching id or exception if not exists.
        /// </returns>
        /// <exception cref="NotFoundException">If matching id was not found.</exception>
        public CommentActionModel FindByPk(Guid id)
        {
            return FindByPk(id, true);
        }

        /// <summary>
        /// Finds item by PK.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="throwOnNotFound">if set to <c>true</c> [throw exception on not found].</param>
        /// <returns>
        /// Item matching id or null/exception if not exists.
        /// </returns>
        public CommentActionModel FindByPk(Guid id, bool throwOnNotFound)
        {
            CommentActionModel result = null;

            Comment Comment = null;
            var projection = new ProjectionQuery<CommentVersion, CommentActionModel>(

                // add alias and filter
                DetachedCriteria.For<CommentVersion>()
                    .CreateAlias<CommentVersion>(commentVersion => commentVersion.Comment, () => Comment)
                    .Add<CommentVersion>(
                        commentVersion => commentVersion.Comment.ActualVersion == commentVersion.Version)
                    .Add<CommentVersion>(commentVersion => commentVersion.Comment.Id == id),

                // map projection
                Projections.ProjectionList()
                    .Add(LambdaProjection.Property<CommentVersion>(
                        commentVersion => commentVersion.Comment.Id)
                             .As(() => result.Id))
                    .Add(LambdaProjection.Property<CommentVersion>(
                        commentVersion => commentVersion.Comment.Content)
                             .As(() => result.Content))
                    .Add(LambdaProjection.Property<CommentVersion>(
                        commentVersion => commentVersion.Comment.Name)
                             .As(() => result.Name))
                    .Add(LambdaProjection.Property<CommentVersion>(
                        commentVersion => commentVersion.Text)
                             .As(() => result.Text))
                    .Add(LambdaProjection.Property<CommentVersion>(
                        commentVersion => commentVersion.CreatedDate)
                             .As(() => result.ChangeDate))
                    .Add(LambdaProjection.Property<CommentVersion>(
                        commentVersion => commentVersion.Comment.ParentComment.Id)
                             .As(() => result.ParentId))
                    .Add(LambdaProjection.Property<CommentVersion>(
                        commentVersion => commentVersion.CreatedBy)
                             .As(() => result.ChangeUser))
                    .Add(LambdaProjection.Property<CommentVersion>(
                        commentVersion => commentVersion.Comment.AnonymousName)
                             .As(() => result.AnonymousName))
                    .Add(LambdaProjection.Property<CommentVersion>(
                        commentVersion => commentVersion.ClientIp)
                             .As(() => result.ClientIp))
                    .Add(LambdaProjection.Property<CommentVersion>(
                        commentVersion => commentVersion.UserAgent)
                             .As(() => result.UserAgent)));

            result = projection.Execute().FirstOrDefault();
            if (throwOnNotFound && result == null)
            {
                throw new NotFoundException(typeof(Comment), id,
                                            "with relation to StaticTextVersion via Version number");
            }
            return result;
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Updted item.</returns>
        public CommentActionModel Update(CommentActionModel item)
        {
            var comment = ActiveRecordMediator<Comment>.FindByPrimaryKey(item.Id);

            comment.Name = item.Name;
            comment.ActualVersion = comment.CommentVersions.Count;

            comment.CommentVersions.Add(
                new CommentVersion()
                {
                    CreatedDate = item.ChangeDate,
                    CreatedBy = item.ChangeUser,
                    ClientIp = item.ClientIp,
                    UserAgent = item.UserAgent,
                    Text = item.Text,
                    Version = comment.ActualVersion
                }
                );

            ActiveRecordMediator<Comment>.Save(comment);

            return item;
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Delete(CommentActionModel item)
        {
            var comment = ActiveRecordMediator<Comment>.FindByPrimaryKey(item.Id);
            ActiveRecordMediator<Comment>.Delete(comment);
        }
    }
}