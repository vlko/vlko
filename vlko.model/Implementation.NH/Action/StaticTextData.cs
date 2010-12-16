using System;
using System.Collections.Generic;
using System.Linq;
using vlko.model.Action;
using vlko.model.Action.ViewModel;
using vlko.core.Repository;
using vlko.model.Implementation.NH.Repository;
using vlko.model.Roots;

namespace vlko.model.Implementation.NH.Action
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
			return new QueryLinqResult<StaticTextViewModel>(GetProjection(
				query =>
				{
					query = query.Where(textVersion => textVersion.StaticText.Hidden == false);
					if (pivotDate != null)
					{
						query = query.Where(textVersion => textVersion.StaticText.PublishDate <= pivotDate);
					}
					return query;
				}));

		}

		/// <summary>
		/// Gets the deleted.
		/// </summary>
		/// <returns>Query result.</returns>
		public IQueryResult<StaticTextViewModel> GetDeleted()
		{
			return new QueryLinqResult<StaticTextViewModel>(GetProjection(
				query => query.Where(textVersion => textVersion.StaticText.Hidden == true)));
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
					query = query.Where(textVersion => textVersion.StaticText.Id == id);
					if (pivotDate != null)
					{
						query = query.Where(textVersion =>
											textVersion.StaticText.Hidden == false
											&& textVersion.StaticText.PublishDate <= pivotDate);
					}
					return query;
				});


			return projection.FirstOrDefault();
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
						query = query.Where(textVersion => textVersion.StaticText.FriendlyUrl == friendlyUrl);
						if (pivotDate != null)
						{
							query = query.Where(textVersion =>
							                    textVersion.StaticText.Hidden == false
							                    && textVersion.StaticText.PublishDate <= pivotDate);
						}
						return query;
					});
			return projection.FirstOrDefault();
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

			return new QueryLinqResult<StaticTextViewModel>(GetProjection(
				query => query.Where(textVersion => textVersion.StaticText.Hidden == false && idArray.Contains(textVersion.StaticText.Id))));
		}

		/// <summary>
		/// Gets the projection query.
		/// </summary>
		/// <param name="additionalFilter">The additional filter.</param>
		/// <returns>Projection query.</returns>
		private static IQueryable<StaticTextViewModel> GetProjection(Func<IQueryable<StaticTextVersion>, IQueryable<StaticTextVersion>> additionalFilter)
		{
			var query = SessionFactory<StaticTextVersion>.Queryable
				.Where(textVersion => textVersion.StaticText.ActualVersion == textVersion.Version);
			query = additionalFilter(query);
			return query.Select(textVersion => new StaticTextViewModel
			                                   	{
			                                   		Id = textVersion.StaticText.Id,
			                                   		FriendlyUrl = textVersion.StaticText.FriendlyUrl,
			                                   		Title = textVersion.StaticText.Title,
			                                   		Description = textVersion.StaticText.Description,
			                                   		Creator = textVersion.StaticText.CreatedBy,
			                                   		ChangeDate = textVersion.CreatedDate,
			                                   		PublishDate = textVersion.StaticText.PublishDate,
													AllowComments = textVersion.StaticText.AreCommentAllowed,
													CommentCounts = SessionFactory<Comment>.Queryable.Count(comm => comm.Content.Id == textVersion.StaticText.Id)
			                                   	});
		}

		/// <summary>
		/// Gets the projection query with full text.
		/// </summary>
		/// <param name="additionalFilter">The additional filter.</param>
		/// <returns>Projection query.</returns>
		private static IQueryable<StaticTextWithFullTextViewModel> GetFullTextProjection(Func<IQueryable<StaticTextVersion>, IQueryable<StaticTextVersion>> additionalFilter)
		{
			var query = SessionFactory<StaticTextVersion>.Queryable
				.Where(textVersion => textVersion.StaticText.ActualVersion == textVersion.Version);
			query = additionalFilter(query);
			return query.Select(textVersion => new StaticTextWithFullTextViewModel
			                                   	{
			                                   		Id = textVersion.StaticText.Id,
			                                   		FriendlyUrl = textVersion.StaticText.FriendlyUrl,
			                                   		Title = textVersion.StaticText.Title,
			                                   		Description = textVersion.StaticText.Description,
													Text = textVersion.Text,
			                                   		Creator = textVersion.StaticText.CreatedBy,
			                                   		ChangeDate = textVersion.CreatedDate,
			                                   		PublishDate = textVersion.StaticText.PublishDate,
			                                   		AllowComments = textVersion.StaticText.AreCommentAllowed,
													CommentCounts = SessionFactory<Comment>.Queryable.Count(comm => comm.Content.Id == textVersion.StaticText.Id)
			                                   	});
		}
	}
}