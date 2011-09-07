namespace vlko.core.Repository.RepositoryAction
{
    /// <summary>
    /// Update action interface.
    /// </summary>
    /// <typeparam name="T">Generic type.</typeparam>
    public interface IUpdateCommand<T> : ICommandGroup<T> where T : class
    {
        /// <summary>
        /// Update the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Update item.</returns>
        T Update(T item);
    }
}
