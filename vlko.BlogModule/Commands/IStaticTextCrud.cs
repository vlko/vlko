using System;
using System.ComponentModel.Composition;
using vlko.BlogModule.Commands.CRUDModel;
using vlko.BlogModule.Roots;
using vlko.core.Repository;
using vlko.core.Repository.Exceptions;

namespace vlko.BlogModule.Commands
{
	[InheritedExport]
    public interface IStaticTextCrud : ICommandGroup<StaticText>
    {

        /// <summary>
        /// Creates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Created item.</returns>
        StaticTextCRUDModel Create(StaticTextCRUDModel item);

        /// <summary>
        /// Finds the by primary key.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// Item matching id or exception if not exists.
        /// </returns>
        /// <exception cref="NotFoundException">If matching id was not found.</exception>
        StaticTextCRUDModel FindByPk(Guid id);

        /// <summary>
        /// Finds item by PK.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="throwOnNotFound">if set to <c>true</c> [throw exception on not found].</param>
        /// <returns>Item matching id or null/exception if not exists.</returns>
        StaticTextCRUDModel FindByPk(Guid id, bool throwOnNotFound);

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Updated item.</returns>
        StaticTextCRUDModel Update(StaticTextCRUDModel item);

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Delete(StaticTextCRUDModel item);
    }
}
