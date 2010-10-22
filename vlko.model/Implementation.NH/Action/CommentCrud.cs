﻿using System;
using System.Collections.Generic;
using System.Linq;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using vlko.model.Action;
using vlko.model.Action.CRUDModel;
using vlko.model.Repository;
using NotFoundException = vlko.model.Repository.Exceptions.NotFoundException;

namespace vlko.model.Implementation.NH.Action
{
	public class CommentCrud : BaseAction<Comment>, ICommentCrud
	{
		/// <summary>
		/// Creates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Created item.</returns>
		public CommentCRUDModel Create(CommentCRUDModel item)
		{
			var content = ActiveRecordMediator<Content>.FindByPrimaryKey(item.ContentId);
			var comment = new Comment()
							  {
								  Name = item.Name,
								  Content = content,
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
			var query = ActiveRecordLinqBase<CommentVersion>.Queryable
				.Where(commentVersion => commentVersion.Comment.ActualVersion == commentVersion.Version
				                         && commentVersion.Comment.Id == id)
				.Select(commentVersion => new CommentCRUDModel
				                          	{
				                          		Id = commentVersion.Comment.Id,
				                          		ContentId = commentVersion.Comment.Content.Id,
				                          		Name = commentVersion.Comment.Name,
				                          		Text = commentVersion.Text,
				                          		ChangeDate = commentVersion.CreatedDate,
				                          		ParentId = commentVersion.Comment.ParentComment.Id,
				                          		ChangeUser = commentVersion.CreatedBy,
				                          		AnonymousName = commentVersion.Comment.AnonymousName,
				                          		ClientIp = commentVersion.ClientIp,
				                          		UserAgent = commentVersion.UserAgent
				                          	});

			var result = query.FirstOrDefault();
			if (throwOnNotFound && result == null)
			{
				throw new NotFoundException(typeof (Comment), id,
				                            "with relation to StaticTextVersion via Version number");
			}
			return result;
		}

		/// <summary>
		/// Updates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>Updted item.</returns>
		public CommentCRUDModel Update(CommentCRUDModel item)
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
		public void Delete(CommentCRUDModel item)
		{
			var comment = ActiveRecordMediator<Comment>.FindByPrimaryKey(item.Id);
			ActiveRecordMediator<Comment>.Delete(comment);
		}
	}
}