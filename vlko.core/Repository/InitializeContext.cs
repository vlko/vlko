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

	public class InitializeContext<T> : IInitializeContext<T> where T : class
	{
		private readonly BaseRepository<T> _baseRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="InitializeContext{T}"/> class.
		/// </summary>
		/// <param name="baseRepository">The BaseRepository.</param>
		public InitializeContext(BaseRepository<T> baseRepository)
		{
			_baseRepository = baseRepository;
		}

		/// <summary>
		/// Gets the BaseRepository.
		/// </summary>
		/// <value>The BaseRepository.</value>
		public IRepository<T> Repository
		{
			get { return _baseRepository; }
		}
	}
}
