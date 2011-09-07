using System;
using System.ComponentModel.Composition;
using vlko.BlogModule.Action.CRUDModel;
using vlko.BlogModule.Roots;
using vlko.core.Repository.Exceptions;
using vlko.core.Repository;

namespace vlko.BlogModule.Action
{
	[InheritedExport]
    public interface ICommentCrud : ICommandGroup<Comment>
    {
        /// <summary>
        /// Creates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Created item.</returns>
        CommentCRUDModel Create(CommentCRUDModel item);

        /// <summary>
        /// Finds the by primary key.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// Item matching id or exception if not exists.
        /// </returns>
        /// <exception cref="NotFoundException">If matching id was not found.</exception>
        CommentCRUDModel FindByPk(Guid id);

        /// <summary>
        /// Finds item by PK.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="throwOnNotFound">if set to <c>true</c> [throw exception on not found].</param>
        /// <returns>Item matching id or null/exception if not exists.</returns>
        CommentCRUDModel FindByPk(Guid id, bool throwOnNotFound);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Updted item.</returns>
        CommentCRUDModel Update(CommentCRUDModel item);

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Delete(CommentCRUDModel item);
    }
}
