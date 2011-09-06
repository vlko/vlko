using System;
using System.Linq;
using System.Collections.Generic;
using vlko.BlogModule.Action;
using vlko.BlogModule.Action.CRUDModel;
using vlko.BlogModule.Roots;
using vlko.core.NH.Repository;
using vlko.core.Repository;
using vlko.core.Repository.Exceptions;
using vlko.core.Roots;

namespace vlko.BlogModule.NH.Action
{
	public class StaticTextCrud : BaseAction<StaticText>, IStaticTextCrud
	{
		/// <summary>
		/// Creates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Created item.</returns>
		public StaticTextCRUDModel Create(StaticTextCRUDModel item)
		{
			var staticText = new StaticText
								 {
									 Id = Guid.NewGuid(),
									 Title = item.Title,
									 FriendlyUrl = item.FriendlyUrl,
									 CreatedDate = item.ChangeDate,
									 Modified = item.ChangeDate,
									 PublishDate = item.PublishDate,
									 Description = item.Description,
									 CreatedBy = (User)item.Creator,
									 AreCommentAllowed = item.AllowComments,
									 ActualVersion = 0,
									 StaticTextVersions = new List<StaticTextVersion>()
															  {
																  new StaticTextVersion
																	  {
																		  Id = Guid.NewGuid(),
																		  CreatedDate = item.ChangeDate,
																		  CreatedBy = (User)item.Creator,
																		  Text = item.Text,
																		  Version = 0
																	  }
															  }
								 };
			SessionFactory<StaticText>.Create(staticText);

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
		public StaticTextCRUDModel FindByPk(Guid id)
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
		public StaticTextCRUDModel FindByPk(Guid id, bool throwOnNotFound)
		{
			StaticTextCRUDModel result = null;

			var query = SessionFactory<StaticTextVersion>.Queryable
				.Where(textVersion => textVersion.StaticText.Id == id &&  textVersion.StaticText.ActualVersion == textVersion.Version)
				.Select(textVersion => new StaticTextCRUDModel{
				                       	Id = textVersion.StaticText.Id,
										FriendlyUrl = textVersion.StaticText.FriendlyUrl,
										Title = textVersion.StaticText.Title,
										Text = textVersion.Text,
										Description = textVersion.StaticText.Description,
										Creator = textVersion.StaticText.CreatedBy,
										ChangeDate = textVersion.CreatedDate,
										PublishDate = textVersion.StaticText.PublishDate,
										AllowComments = textVersion.StaticText.AreCommentAllowed});


			result = query.FirstOrDefault();
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
		public StaticTextCRUDModel Update(StaticTextCRUDModel item)
		{
			var staticText = SessionFactory<StaticText>.FindByPrimaryKey(item.Id);

						if (string.IsNullOrEmpty(item.FriendlyUrl))
			{
				item.FriendlyUrl = item.Title;
			}

			staticText.Title = item.Title;
			staticText.FriendlyUrl = item.FriendlyUrl;
			staticText.Modified = item.ChangeDate;
			staticText.PublishDate = item.PublishDate;
			staticText.CreatedBy = (User)item.Creator;
			staticText.AreCommentAllowed = item.AllowComments;
			staticText.ActualVersion = staticText.StaticTextVersions.Count;

			staticText.Description = item.Description;

			staticText.StaticTextVersions.Add(
				new StaticTextVersion
					{
						Id = Guid.NewGuid(),
						CreatedDate = item.ChangeDate,
						CreatedBy = (User)item.Creator,
						Text = item.Text,
						Version = staticText.ActualVersion
					}
				);

			SessionFactory<StaticText>.Update(staticText);

			return item;
		}

		/// <summary>
		/// Deletes the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Delete(StaticTextCRUDModel item)
		{
			var staticText = SessionFactory<StaticText>.FindByPrimaryKey(item.Id);
			staticText.Hidden = true;
			SessionFactory<StaticText>.Update(staticText);
		}

	}
}
