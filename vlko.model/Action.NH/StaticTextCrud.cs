using System;
using System.Collections.Generic;
using System.Linq;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using vlko.model.ActionModel;
using vlko.model.Repository;
using NotFoundException = vlko.model.Repository.Exceptions.NotFoundException;

namespace vlko.model.Action.NH
{
	public class StaticTextCrud : BaseAction<StaticText>, IStaticTextCrud
	{
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
									 Modified = item.ChangeDate,
									 PublishDate = item.PublishDate,
									 Description = item.Description,
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
		/// <exception cref="Repository.Exceptions.NotFoundException">If matching id was not found.</exception>
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

			var query = ActiveRecordLinqBase<StaticTextVersion>.Queryable
				.Where(textVersion => textVersion.StaticText.Id == id &&  textVersion.StaticText.ActualVersion == textVersion.Version)
				.Select(textVersion => new StaticTextActionModel{
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
		public StaticTextActionModel Update(StaticTextActionModel item)
		{
			var staticText = ActiveRecordMediator<StaticText>.FindByPrimaryKey(item.Id);

						if (string.IsNullOrEmpty(item.FriendlyUrl))
			{
				item.FriendlyUrl = item.Title;
			}

			staticText.Title = item.Title;
			staticText.FriendlyUrl = item.FriendlyUrl;
			staticText.Modified = item.ChangeDate;
			staticText.PublishDate = item.PublishDate;
			staticText.CreatedBy = item.Creator;
			staticText.AreCommentAllowed = item.AllowComments;
			staticText.ActualVersion = staticText.StaticTextVersions.Count;

			staticText.Description = item.Description;

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
