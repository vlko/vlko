namespace vlko.core.Repository.RepositoryAction
{
    /// <summary>
    /// Save action interface.
    /// </summary>
    /// <typeparam name="T">Generic type.</typeparam>
    public interface ISaveAction<T> : IAction<T> where T : class
    {
        /// <summary>
        /// Saves the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Saved item.</returns>
        T Save(T item);
    }
}
