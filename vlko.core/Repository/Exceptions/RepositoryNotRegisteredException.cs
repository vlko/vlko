using System;

namespace vlko.core.Repository.Exceptions
{
	/// <summary>
	/// Repository not registered exception.
	/// </summary>
	public class RepositoryNotRegisteredException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RepositoryNotRegisteredException"/> class.
		/// </summary>
		/// <param name="repositoryType">Type of the repository.</param>
		/// <param name="innerException">The inner exception.</param>
		public RepositoryNotRegisteredException(Type repositoryType, Exception innerException)
			:base(string.Format("Unable to locate repository type '{0}'!", repositoryType), innerException)
		{
		}
	}
}
