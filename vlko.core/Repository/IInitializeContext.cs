namespace vlko.core.Repository
{
	public interface IInitializeContext<out T> where T : class
	{
		/// <summary>
		/// Gets the BaseRepository.
		/// </summary>
		/// <value>The BaseRepository.</value>
		IRepository<T> Repository { get; }
	}
}