using System;
using System.Linq;
using vlko.BlogModule.Action;
using vlko.BlogModule.Action.CRUDModel;
using vlko.BlogModule.Action.ViewModel;
using vlko.BlogModule.Roots;
using vlko.core.NH.Repository;
using vlko.core.Repository;
using vlko.core.Repository.Exceptions;

namespace vlko.BlogModule.NH.Action
{
	public class RssFeedAction : BaseAction<RssFeed>, IRssFeedAction
	{
		/// <summary>
		/// Creates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Created item.</returns>
		public RssFeedCRUDModel Create(RssFeedCRUDModel item)
		{
			var feed = new RssFeed
			           	{
			           		Name = item.Name,
			           		AuthorRegex = item.AuthorRegex,
			           		GetDirectContent = item.GetDirectContent,
			           		DisplayFullContent = item.DisplayFullContent,
			           		ContentParseRegex = item.ContentParseRegex,
			           		Url = item.Url
			           	};

			SessionFactory<RssFeed>.Create(feed);

			item.Id = feed.Id;

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
		public RssFeedCRUDModel FindByPk(Guid id)
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
		public RssFeedCRUDModel FindByPk(Guid id, bool throwOnNotFound)
		{
			RssFeedCRUDModel result = null;

			var query = SessionFactory<RssFeed>.Queryable
				.Where(item => item.Id == id)
				.Select(item => new RssFeedCRUDModel
				{
					Id = item.Id,
					Name = item.Name,
					AuthorRegex = item.AuthorRegex,
					GetDirectContent = item.GetDirectContent,
					DisplayFullContent = item.DisplayFullContent,
					ContentParseRegex = item.ContentParseRegex,
					Url = item.Url
				});


			result = query.FirstOrDefault();

			if (throwOnNotFound && result == null)
			{
				throw new NotFoundException(typeof(RssFeed), id,
											"with relation to StaticTextVersion via Version number");
			}
			return result;
		}

		/// <summary>
		/// Updates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Updated item.</returns>
		public RssFeedCRUDModel Update(RssFeedCRUDModel item)
		{
			var feed = SessionFactory<RssFeed>.FindByPrimaryKey(item.Id);
			feed.Name = item.Name;
			feed.AuthorRegex = item.AuthorRegex;
			feed.GetDirectContent = item.GetDirectContent;
			feed.DisplayFullContent = item.DisplayFullContent;
			feed.ContentParseRegex = item.ContentParseRegex;
			feed.Url = item.Url;

			SessionFactory<RssFeed>.Update(feed);

			return item;
		}

		/// <summary>
		/// Deletes the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Delete(RssFeedCRUDModel item)
		{
			var feed = SessionFactory<RssFeed>.FindByPrimaryKey(item.Id);
			SessionFactory<RssFeed>.Delete(feed);
		}

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns>Query result with all rss feeds.</returns>
		public IQueryResult<RssFeedViewModel> GetAll()
		{
			return new QueryLinqResult<RssFeedViewModel>(SessionFactory<RssFeed>.Queryable
				.Select(item => new RssFeedViewModel
				{
					Id = item.Id,
					Name = item.Name,
					DisplayFullContent = item.DisplayFullContent,
					FeedUrl = item.Url,
					FeedItemCount = item.RssItems.Count()
				}));
		}

		/// <summary>
		/// Gets the feed to process.
		/// </summary>
		/// <returns>Feeds to process.</returns>
		public RssFeedCRUDModel[] GetFeedToProcess()
		{
			return SessionFactory<RssFeed>.Queryable
				.Select(item => new RssFeedCRUDModel
				{
					Id = item.Id,
					Name = item.Name,
					AuthorRegex = item.AuthorRegex,
					GetDirectContent = item.GetDirectContent,
					DisplayFullContent = item.DisplayFullContent,
					ContentParseRegex = item.ContentParseRegex,
					Url = item.Url
				})
				.ToArray();
		}
	}
}