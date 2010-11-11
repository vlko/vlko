namespace vlko.core.Repository
{
	public interface IAction
	{
		/// <summary>
		/// Gets a value indicating whether this <see cref="IAction&lt;T&gt;"/> is initialized.
		/// </summary>
		/// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
		bool Initialized { get; }
		/// <summary>
		/// Initializes this instance.
		/// Should contain such as code: RepositoryFactory.GetRepository<T>().InitalizeAction(this); 
		/// </summary>
		void Initialize();
	}

	public interface IAction<in T> : IAction where T : class
	{
		/// <summary>
		/// Initializes action with the specified repository.
		/// </summary>
		/// <param name="initializeContext">The initialize context.</param>
		void Initialize(IInitializeContext<T> initializeContext);
	}
}
