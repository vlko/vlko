using vlko.core.Repository.Exceptions;

namespace vlko.core.Repository.RepositoryAction
{
    /// <summary>
    /// Find by Primary Key command interface.
    /// </summary>
    /// <typeparam name="T">Generic type.</typeparam>
    public interface IFindByPkCommand<T> : ICommandGroup<T> where T : class
    {
        /// <summary>
        /// Finds the by PK.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Item matching id or exception if not exists.</returns>
        /// <exception cref="NotFoundException">If matching id was not found.</exception>
        T FindByPk(object id);

        /// <summary>
        /// Finds item by PK.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="throwOnNotFound">if set to <c>true</c> [throw exception on not found].</param>
        /// <returns>Item matching id or null/exception if not exists.</returns>
        T FindByPk(object id, bool throwOnNotFound);
    }
}
