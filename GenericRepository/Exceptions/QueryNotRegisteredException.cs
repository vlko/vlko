using System;

namespace GenericRepository.Exceptions
{
    /// <summary>
    /// Query not registered exception.
    /// </summary>
    public class QueryNotRegisteredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryNotRegisteredException"/> class.
        /// </summary>
        /// <param name="queryType">Type of the query.</param>
        /// <param name="repositoryType">Type of the repository.</param>
        /// <param name="repositoryGenericsType">Type of the repository generics.</param>
        public QueryNotRegisteredException(Type queryType, Type repositoryType, Type repositoryGenericsType)
            : base(string.Format("Unable to find query type '{0}' for repository type '{1}' handling generic type '{2}'",
                                 queryType, repositoryType, repositoryGenericsType))
        {
        }
    }
}