using System;
using System.Collections.Generic;
using System.Linq;
using Castle.ActiveRecord.Queries;
using GenericRepository;
using NHibernate.Criterion;
using NHibernate.LambdaExtensions;
using vlko.core.ActiveRecord;
using vlko.core.Models.Action.ViewModel;

namespace vlko.core.Models.Action.Implementation
{
    public class StaticTextData : IStaticTextData
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
        public void Initialize(InitializeContext<StaticText> initializeContext)
        {
            Initialized = true;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>Query result.</returns>
        public IQueryResult<StaticTextViewModel> GetAll()
        {
            var projection = GetProjection(criteria =>
                criteria.Add<StaticTextVersion>(staticTextVersion => staticTextVersion.StaticText.Deleted == false));

            return projection;
        }

        /// <summary>
        /// Gets the deleted.
        /// </summary>
        /// <returns>Query result.</returns>
        public IQueryResult<StaticTextViewModel> GetDeleted()
        {
            var projection = GetProjection(criteria =>
                criteria.Add<StaticTextVersion>(staticTextVersion => staticTextVersion.StaticText.Deleted == true));

            return projection;
        }

        /// <summary>
        /// Gets the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Data model for specific id.</returns>
        public StaticTextViewModel Get(Guid id)
        {
            var projection = GetProjection(criteria =>
                criteria.Add<StaticTextVersion>(staticTextVersion => staticTextVersion.StaticText.Id == id));

            return projection.ToArray().FirstOrDefault(); ;
        }

        /// <summary>
        /// Gets the specified nice URL.
        /// </summary>
        /// <param name="friendlyUrl">The friendly URL.</param>
        /// <returns>Data model for</returns>
        public StaticTextViewModel Get(string friendlyUrl)
        {
            var projection = GetProjection(criteria =>
                criteria.Add<StaticTextVersion>(staticTextVersion => staticTextVersion.StaticText.FriendlyUrl == friendlyUrl));

            return projection.ToArray().FirstOrDefault();
        }

        /// <summary>
        /// Gets the projection query.
        /// </summary>
        /// <param name="additionalFilter">The additional filter.</param>
        /// <returns>Projection query.</returns>
        private static ProjectionQueryResult<StaticTextVersion, StaticTextViewModel> GetProjection(Func<DetachedCriteria, DetachedCriteria> additionalFilter)
        {
            StaticTextViewModel result = null;

            StaticText StaticText = null;
            Content Content = null;

            return new ProjectionQueryResult<StaticTextVersion, StaticTextViewModel>(

                // add alias and
                additionalFilter(DetachedCriteria.For<StaticTextVersion>()
                    .CreateAlias<StaticTextVersion>(staticTextVersion => staticTextVersion.StaticText, () => StaticText)
                    .Add<StaticTextVersion>(
                        staticTextVersion => staticTextVersion.StaticText.ActualVersion == staticTextVersion.Version)),

                // map projection
                Projections.ProjectionList()
                    .Add(LambdaProjection.Property<StaticTextVersion>(
                        staticTextVersion => staticTextVersion.StaticText.Id)
                             .As(() => result.Id))
                    .Add(LambdaProjection.Property<StaticTextVersion>(
                        staticTextVersion => staticTextVersion.StaticText.FriendlyUrl)
                             .As(() => result.FriendlyUrl))
                    .Add(LambdaProjection.Property<StaticTextVersion>(
                        staticTextVersion => staticTextVersion.StaticText.Title)
                             .As(() => result.Title))
                    .Add(LambdaProjection.Property<StaticTextVersion>(
                        staticTextVersion => staticTextVersion.Text)
                             .As(() => result.Text))
                    .Add(LambdaProjection.Property<StaticTextVersion>(
                        staticTextVersion => staticTextVersion.StaticText.CreatedBy)
                             .As(() => result.Creator))
                    .Add(LambdaProjection.Property<StaticTextVersion>(
                        staticTextVersion => staticTextVersion.CreatedDate)
                             .As(() => result.ChangeDate))
                    .Add(LambdaProjection.Property<StaticTextVersion>(
                        staticTextVersion => staticTextVersion.StaticText.PublishDate)
                             .As(() => result.PublishDate))
                    .Add(LambdaProjection.Property<StaticTextVersion>(
                        staticTextVersion => staticTextVersion.StaticText.AreCommentAllowed)
                             .As(() => result.AllowComments))
                    .Add(Projections.SubQuery(
                        DetachedCriteria.For<Comment>()
                            .CreateAlias<Comment>(comment => comment.Content, () => Content)
                            .Add<Comment>(
                                comment => comment.Content.Id == StaticText.Id)
                            .SetProjection(LambdaProjection.Count<Comment>(comm => comm.Id))
                             ), "CommentCounts"));
        }
    }
}