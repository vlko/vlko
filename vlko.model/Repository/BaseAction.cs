namespace vlko.model.Repository
{
	public class BaseAction<T> : IAction<T> where T : class
	{

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		public void Initialize()
		{
			RepositoryFactory.GetRepository<T>().InitalizeAction(this);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="BaseAction&lt;T&gt;"/> is initialized.
		/// </summary>
		/// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
		public bool Initialized { get; private set; }

		/// <summary>
		/// Initializes the specified initialize context.
		/// </summary>
		/// <param name="initializeContext">The initialize context.</param>
		public virtual void Initialize(InitializeContext<T> initializeContext)
		{
			Initialized = true;
		}
	}
}