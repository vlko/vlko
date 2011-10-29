namespace vlko.core.Repository
{
	public class CommandGroup<T> : ICommandGroup<T> where T : class
	{

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		public void Initialize()
		{

		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="CommandGroup{T}"/> is initialized.
		/// </summary>
		/// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
		public bool Initialized { get; private set; }

		/// <summary>
		/// Initializes the specified initialize context.
		/// </summary>
		/// <param name="initializeContext">The initialize context.</param>
		public virtual void Initialize(IInitializeContext<T> initializeContext)
		{
			Initialized = true;
		}
	}
}