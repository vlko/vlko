using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Linq;
using Castle.ActiveRecord.Queries;
using GenericRepository;
using NHibernate.Criterion;
using NHibernate.LambdaExtensions;
using vlko.core.ActiveRecord;
using vlko.core.Models.Action.ActionModel;
using vlko.core.Tools;
using NotFoundException = GenericRepository.Exceptions.NotFoundException;

namespace vlko.core.Models.Action.Implementation
{
    public class StaticTextCrud : IStaticTextCrud
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
        /// Creates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Created item.</returns>
        public StaticTextActionModel Create(StaticTextActionModel item)
        {
            var staticText = new StaticText
                                 {
                                     Title = item.Title,
                                     FriendlyUrl = item.FriendlyUrl,
                                     CreatedDate = item.ChangeDate,
                                     PublishDate = item.PublishDate,
                                     CreatedBy = item.Creator,
                                     AreCommentAllowed = item.AllowComments,
                                     ActualVersion = 0,
                                     StaticTextVersions = new List<StaticTextVersion>()
                                                              {
                                                                  new StaticTextVersion
                                                                      {
                                                                          CreatedDate = item.ChangeDate,
                                                                          CreatedBy = item.Creator,
                                                                          Text = item.Text,
                                                                          Version = 0
                                                                      }
                                                              }
                                 };
            ActiveRecordMediator<StaticText>.Create(staticText);

            // assign id
            item.Id = staticText.Id;

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
        public StaticTextActionModel FindByPk(Guid id)
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
        public StaticTextActionModel FindByPk(Guid id, bool throwOnNotFound)
        {
            StaticTextActionModel result = null;

            StaticText StaticText = null;
            var projection = new ProjectionQuery<StaticTextVersion, StaticTextActionModel>(

                // add alias and
                DetachedCriteria.For<StaticTextVersion>()
                    .CreateAlias<StaticTextVersion>(staticTextVersion => staticTextVersion.StaticText, () => StaticText)
                    .Add<StaticTextVersion>(
                        staticTextVersion => staticTextVersion.StaticText.ActualVersion == staticTextVersion.Version)
                    .Add<StaticTextVersion>(staticTextVersion => staticTextVersion.StaticText.Id == id),

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
                             .As(() => result.AllowComments)));

            result = projection.Execute().FirstOrDefault();
            if (throwOnNotFound && result == null)
            {
                throw new NotFoundException(typeof (StaticText), id,
                                            "with relation to StaticTextVersion via Version number");
            }
            return result;
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Updated item.</returns>
        public StaticTextActionModel Update(StaticTextActionModel item)
        {
            var staticText = ActiveRecordMediator<StaticText>.FindByPrimaryKey(item.Id);

                        if (string.IsNullOrEmpty(item.FriendlyUrl))
            {
                item.FriendlyUrl = item.Title;
            }

            staticText.Title = item.Title;
            staticText.FriendlyUrl = item.FriendlyUrl;
            staticText.PublishDate = item.PublishDate;
            staticText.CreatedBy = item.Creator;
            staticText.AreCommentAllowed = item.AllowComments;
            staticText.ActualVersion = staticText.StaticTextVersions.Count;

            staticText.StaticTextVersions.Add(
                new StaticTextVersion
                    {
                        CreatedDate = item.ChangeDate,
                        CreatedBy = item.Creator,
                        Text = item.Text,
                        Version = staticText.ActualVersion
                    }
                );

            ActiveRecordMediator<StaticText>.Save(staticText);

            return item;
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Delete(StaticTextActionModel item)
        {
            var staticText = ActiveRecordMediator<StaticText>.FindByPrimaryKey(item.Id);
            staticText.Deleted = true;
            ActiveRecordMediator<StaticText>.Save(staticText);
        }
    }
}
