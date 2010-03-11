namespace GenericRepository
{
    /// <summary>
    /// Query interface.
    /// </summary>
    /// <typeparam name="T">Generic type.</typeparam>
    public interface IQuery<T> where T : class
    {
        /// <summary>
        /// Initializes query with the specified repository.
        /// </summary>
        /// <param name="initializeContext">The initialize context.</param>
        void Initialize(QueryInitializeContext<T> initializeContext);
    }
}
