using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericRepository
{
	public interface IAction
	{
		/// <summary>
		/// Initializes this instance.
		/// Should contain such as code: RepositoryFactory.GetRepository<T>().InitalizeAction(this); 
		/// </summary>
		void Initialize();
	}

	public interface IAction<T> : IAction where T : class
	{
		/// <summary>
		/// Gets a value indicating whether this <see cref="IAction&lt;T&gt;"/> is initialized.
		/// </summary>
		/// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
		bool Initialized { get; }

		/// <summary>
		/// Initializes action with the specified repository.
		/// </summary>
		/// <param name="initializeContext">The initialize context.</param>
		void Initialize(InitializeContext<T> initializeContext);
	}
}
