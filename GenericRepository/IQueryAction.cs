namespace GenericRepository
{
    /// <summary>
    /// Query action interface.
    /// </summary>
    /// <typeparam name="T">Generic type.</typeparam>
    public interface IQueryAction<T> : IAction<T> where T : class
    {

    }
}
