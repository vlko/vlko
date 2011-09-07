namespace vlko.core.Repository.RepositoryAction
{
    /// <summary>
    /// Create action interface.
    /// </summary>
    /// <typeparam name="T">Generic type.</typeparam>
    public interface ICreateCommand<T>: ICommandGroup<T> where T : class
    {
        /// <summary>
        /// Creates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Created item.</returns>
        T Create(T item);
    }
}
