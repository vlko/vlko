using System;
using System.Collections.Generic;
using System.Linq;
using vlko.BlogModule.Commands;
using vlko.BlogModule.Commands.CRUDModel;
using vlko.BlogModule.Roots;
using vlko.core.RavenDB.Repository;
using vlko.core.Repository;
using vlko.core.Repository.Exceptions;
using vlko.core.Roots;

namespace vlko.BlogModule.RavenDB.Commands
{
	public class StaticTextCrud : CommandGroup<StaticText>, IStaticTextCrud
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
																		  CreatedDate = item.ChangeDate,
																		  CreatedBy = (User)item.Creator,
																		  Text = item.Text,
																		  Version = 0
																	  }
															  }
								 };
			SessionFactory<StaticText>.Store(staticText);

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
			var staticText = SessionFactory<StaticText>.Load(id, throwOnNotFound);
			var staticTextVersion = staticText.StaticTextVersions.FirstOrDefault(textVersion => textVersion.Version == staticText.ActualVersion);
			if (staticText != null && staticTextVersion != null)
			{
				return new StaticTextCRUDModel
				       	{
				       		Id = staticText.Id,
				       		FriendlyUrl = staticText.FriendlyUrl,
				       		Title = staticText.Title,
				       		Text = staticTextVersion.Text,
				       		Description = staticText.Description,
				       		Creator = staticText.CreatedBy,
				       		ChangeDate = staticTextVersion.CreatedDate,
				       		PublishDate = staticText.PublishDate,
				       		AllowComments = staticText.AreCommentAllowed
				       	};
			}
			return null;
		}

		/// <summary>
		/// Updates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Updated item.</returns>
		public StaticTextCRUDModel Update(StaticTextCRUDModel item)
		{
			var staticText = SessionFactory<StaticText>.Load(item.Id);

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
						CreatedDate = item.ChangeDate,
						CreatedBy = (User)item.Creator,
						Text = item.Text,
						Version = staticText.ActualVersion
					}
				);

			SessionFactory<StaticText>.Store(staticText);

			return item;
		}

		/// <summary>
		/// Deletes the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Delete(StaticTextCRUDModel item)
		{
			var staticText = SessionFactory<StaticText>.Load(item.Id);
			staticText.Hidden = true;
			SessionFactory<StaticText>.Store(staticText);
		}

	}
}
