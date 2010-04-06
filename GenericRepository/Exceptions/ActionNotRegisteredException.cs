using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericRepository.Exceptions
{
    /// <summary>
    /// Action not registered exception.
    /// </summary>
    public class ActionNotRegisteredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionNotRegisteredException"/> class.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="repositoryType">Type of the repository.</param>
        /// <param name="repositoryGenericsType">Type of the repository generics.</param>
        public ActionNotRegisteredException(Type actionType, Type repositoryType, Type repositoryGenericsType)
            : base(string.Format("Unable to find action type '{0}' for repository type '{1}' handling generic type '{2}'",
                                 actionType, repositoryType, repositoryGenericsType))
        {
        }
    }
}
