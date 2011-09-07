using System;

namespace vlko.core.Repository.Exceptions
{
    /// <summary>
    /// Command group not registered exception.
    /// </summary>
    public class CommandGroupNotRegisteredException : Exception
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="CommandGroupNotRegisteredException"/> class.
		/// </summary>
		/// <param name="commandGroupType">Type of the command group.</param>
		/// <param name="repositoryType">Type of the repository.</param>
		/// <param name="repositoryGenericsType">Type of the repository generics.</param>
		/// <param name="innerException">The inner exception.</param>
        public CommandGroupNotRegisteredException(Type commandGroupType, Type repositoryType, Type repositoryGenericsType, Exception innerException)
            : base(string.Format("Unable to find command group type '{0}' for repository type '{1}' handling generic type '{2}'",
                                 commandGroupType, repositoryType, repositoryGenericsType), innerException)
        {
        }
    }
}
