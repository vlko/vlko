using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericRepository;
using GenericRepository.Exceptions;
using vlko.core.Models.Action.ActionModel;

namespace vlko.core.Models.Action
{
    public interface ICommentCrud : IAction<Comment>
    {
        /// <summary>
        /// Creates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Created item.</returns>
        CommentActionModel Create(CommentActionModel item);

        /// <summary>
        /// Finds the by primary key.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// Item matching id or exception if not exists.
        /// </returns>
        /// <exception cref="NotFoundException">If matching id was not found.</exception>
        CommentActionModel FindByPk(Guid id);

        /// <summary>
        /// Finds item by PK.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="throwOnNotFound">if set to <c>true</c> [throw exception on not found].</param>
        /// <returns>Item matching id or null/exception if not exists.</returns>
        CommentActionModel FindByPk(Guid id, bool throwOnNotFound);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Updted item.</returns>
        CommentActionModel Update(CommentActionModel item);

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Delete(CommentActionModel item);
    }
}
