using System;
using System.Collections.Generic;
using System.Linq;
using vlko.BlogModule.Action;
using vlko.BlogModule.Action.ViewModel;
using vlko.BlogModule.RavenDB.Indexes;
using vlko.BlogModule.RavenDB.Indexes.ReduceModelView;
using vlko.BlogModule.Roots;
using vlko.core.RavenDB.Repository;
using vlko.core.Repository;

namespace vlko.BlogModule.RavenDB.Action
{
	public class StaticTextData : BaseAction<StaticText>, IStaticTextData
	{

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <param name="pivotDate">The pivot date (only published data younger, that this date).</param>
		/// <returns>Query result.</returns>
		public IQueryResult<StaticTextViewModel> GetAll(DateTime? pivotDate = null)
		{
			return GetProjection(
				query =>
				{
					query = query.Where(staticText => staticText.Hidden == false);
					if (pivotDate != null)
					{
						query = query.Where(staticText => staticText.PublishDate <= pivotDate);
					}
					return query;
				});

		}

		/// <summary>
		/// Gets the deleted.
		/// </summary>
		/// <returns>Query result.</returns>
		public IQueryResult<StaticTextViewModel> GetDeleted()
		{
			return GetProjection(
				query => query.Where(staticText => staticText.Hidden == true));
		}

		/// <summary>
		/// Gets the specified id.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="pivotDate">The pivot date (only published data younger, that this date).</param>
		/// <returns>Data model for specific id.</returns>
		public StaticTextWithFullTextViewModel Get(Guid id, DateTime? pivotDate = null)
		{
			var projection = GetFullTextProjection(
				query =>
				{
					query = query.Where(staticText => staticText.Id == id);
					if (pivotDate != null)
					{
						query = query.Where(staticText =>
											staticText.Hidden == false
											&& staticText.PublishDate <= pivotDate);
					}
					return query;
				});


			return projection.ToPage(0, 1).FirstOrDefault();
		}

		/// <summary>
		/// Gets the specified nice URL.
		/// </summary>
		/// <param name="friendlyUrl">The friendly URL.</param>
		/// <param name="pivotDate">The pivot date (only published data younger, that this date).</param>
		/// <returns>Data model for</returns>
		public StaticTextWithFullTextViewModel Get(string friendlyUrl, DateTime? pivotDate = null)
		{
			var projection = GetFullTextProjection(
				query =>
					{
						query = query.Where(staticText => staticText.FriendlyUrl == friendlyUrl);
						if (pivotDate != null)
						{
							query = query.Where(staticText =>
							                    staticText.Hidden == false
							                    && staticText.PublishDate <= pivotDate);
						}
						return query;
					});
			return projection.ToPage(0, 1).FirstOrDefault();
		}

		/// <summary>
		/// Gets the by ids.
		/// </summary>
		/// <param name="ids"></param>
		/// <returns>All static text matching specified ids.</returns>
		public IQueryResult<StaticTextViewModel> GetByIds(IEnumerable<Guid> ids)
		{
			var idArray = ids.ToArray();

			if (idArray.Length == 0)
			{
				return new EmptyQueryResult<StaticTextViewModel>();
			}

			return GetProjection(
				query => query
							.WhereContainsAsOr(idArray, id => id, id => staticText => staticText.Id == id)
							.Where(staticText => staticText.Hidden == false));
		}

		/// <summary>
		/// Gets the projection query.
		/// </summary>
		/// <param name="additionalFilter">The additional filter.</param>
		/// <returns>Projection query.</returns>
		private static IQueryResult<StaticTextViewModel> GetProjection(Func<IQueryable<StaticText>, IQueryable<StaticText>> additionalFilter)
		{
			IQueryable<StaticText> query = SessionFactory<StaticText>.IndexQuery<StaticTextSortIndex>();
			query = additionalFilter(query);
			return new ResultProjectionQueryResult<StaticText, StaticTextViewModel>(
				query,
				staticTexts =>
					{
						// get comment counts
						var commentCounts = SessionFactory<Comment>.IndexQuery<ContentWithCommentsCount, CommentCount>()
							.WhereContainsAsOr(staticTexts, content => content.Id, id => item => item.ContentId == id)
							.ToDictionary(item => item.ContentId);
						// merge results
						return staticTexts
							.Select(staticText =>
							        	{
							        		var textVersion = staticText.StaticTextVersions.First(version => version.Version == staticText.ActualVersion);
							        		return new StaticTextViewModel
							        		       	{
							        		       		Id = staticText.Id,
							        		       		FriendlyUrl = staticText.FriendlyUrl,
							        		       		Title = staticText.Title,
							        		       		Description = staticText.Description,
							        		       		Creator = staticText.CreatedBy,
							        		       		ChangeDate = textVersion.CreatedDate,
							        		       		PublishDate = staticText.PublishDate,
							        		       		AllowComments = staticText.AreCommentAllowed,
							        		       		CommentCounts = commentCounts.ContainsKey(staticText.Id) ? commentCounts[staticText.Id].Count : 0
							        		       	};
							        	})
							.ToArray();
					}
				)
				.AddSortMapping(staticText => staticText.PublishDate, staticText => staticText.PublishDate)
				.AddSortMapping(staticText => staticText.FriendlyUrl, staticText => staticText.FriendlyUrl);
		}

		/// <summary>
		/// Gets the projection query with full text.
		/// </summary>
		/// <param name="additionalFilter">The additional filter.</param>
		/// <returns>Projection query.</returns>
		private static IQueryResult<StaticTextWithFullTextViewModel> GetFullTextProjection(Func<IQueryable<StaticText>, IQueryable<StaticText>> additionalFilter)
		{
			IQueryable<StaticText> query = SessionFactory<StaticText>.IndexQuery<StaticTextSortIndex>();
			query = additionalFilter(query);
			return new ResultProjectionQueryResult<StaticText, StaticTextWithFullTextViewModel>(
				query,
				staticTexts =>
					{
						// get comment counts
						var commentCounts = SessionFactory<Comment>.IndexQuery<ContentWithCommentsCount, CommentCount>()
							.WhereContainsAsOr(staticTexts, content => content.Id, id => item => item.ContentId == id)
							.ToDictionary(item => item.ContentId);
						// merge results
						return staticTexts
							.Select(staticText =>
							        	{
							        		var textVersion = staticText.StaticTextVersions.First(version => version.Version == staticText.ActualVersion);
							        		return new StaticTextWithFullTextViewModel
							        		       	{
							        		       		Id = staticText.Id,
							        		       		FriendlyUrl = staticText.FriendlyUrl,
							        		       		Title = staticText.Title,
							        		       		Description = staticText.Description,
							        		       		Text = textVersion.Text,
							        		       		Creator = staticText.CreatedBy,
							        		       		ChangeDate = textVersion.CreatedDate,
							        		       		PublishDate = staticText.PublishDate,
							        		       		AllowComments = staticText.AreCommentAllowed,
							        		       		CommentCounts = commentCounts.ContainsKey(staticText.Id) ? commentCounts[staticText.Id].Count : 0
							        		       	};
							        	})
							.ToArray();
					}
				)
				.AddSortMapping(staticText => staticText.PublishDate, staticText => staticText.PublishDate)
				.AddSortMapping(staticText => staticText.FriendlyUrl, staticText => staticText.FriendlyUrl);
		}
	}
}