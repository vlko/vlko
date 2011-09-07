using System;
using System.Collections.Generic;
using System.Linq;
using vlko.BlogModule.Action;
using vlko.BlogModule.Action.CRUDModel;
using vlko.BlogModule.Roots;
using vlko.core.RavenDB.Repository;
using vlko.core.Repository;
using vlko.core.Repository.Exceptions;
using vlko.core.Roots;

namespace vlko.BlogModule.RavenDB.Action
{
	public class CommentCrud : CommandGroup<Comment>, ICommentCrud
	{
		/// <summary>
		/// Creates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Created item.</returns>
		public CommentCRUDModel Create(CommentCRUDModel item)
		{
			var content = SessionFactory<Content>.Load(item.ContentId);
			var comment = new Comment()
							  {
								  Id = Guid.NewGuid(),
								  Name = item.Name,
								  Content = content,
								  Owner = (User)item.ChangeUser,
								  AnonymousName = item.AnonymousName,
								  CreatedDate = item.ChangeDate,
								  ActualVersion = 0,
								  CommentVersions = new List<CommentVersion>()
														{
															new CommentVersion
																{
																	CreatedDate = item.ChangeDate,
																	CreatedBy = (User)item.ChangeUser,
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
				var parentComment = SessionFactory<Comment>.Load(item.ParentId.Value);
				comment.Level = parentComment.Level + 1;
				comment.ParentComment = parentComment;
				comment.TopComment = parentComment.TopComment;
				comment.ParentVersion = parentComment.ActualVersion;
			}

			SessionFactory<Comment>.Store(comment);

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
		public CommentCRUDModel FindByPk(Guid id)
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
		public CommentCRUDModel FindByPk(Guid id, bool throwOnNotFound)
		{
			var comment = SessionFactory<Comment>.Load(id, throwOnNotFound);
			var commentVersion = comment.CommentVersions.FirstOrDefault(version => version.Version == comment.ActualVersion);
			if (comment != null && commentVersion != null)
			{

				return new CommentCRUDModel
				       	{
				       		Id = comment.Id,
				       		ContentId = comment.Content.Id,
				       		Name = comment.Name,
				       		Text = commentVersion.Text,
				       		ChangeDate = commentVersion.CreatedDate,
				       		ParentId = comment.ParentComment != null ? (Guid?)comment.ParentComment.Id : null,
				       		ChangeUser = commentVersion.CreatedBy,
				       		AnonymousName = comment.AnonymousName,
				       		ClientIp = commentVersion.ClientIp,
				       		UserAgent = commentVersion.UserAgent
				       	};
			}
			return null;
		}

		/// <summary>
		/// Updates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Updted item.</returns>
		public CommentCRUDModel Update(CommentCRUDModel item)
		{
			var comment = SessionFactory<Comment>.Load(item.Id);

			comment.Name = item.Name;
			comment.ActualVersion = comment.CommentVersions.Count;

			comment.CommentVersions.Add(
				new CommentVersion()
				{
					CreatedDate = item.ChangeDate,
					CreatedBy = (User)item.ChangeUser,
					ClientIp = item.ClientIp,
					UserAgent = item.UserAgent,
					Text = item.Text,
					Version = comment.ActualVersion
				}
				);

			SessionFactory<Comment>.Store(comment);

			return item;
		}

		/// <summary>
		/// Deletes the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Delete(CommentCRUDModel item)
		{
			var comment = SessionFactory<Comment>.Load(item.Id);
			SessionFactory<Comment>.Delete(comment);
		}
	}
}