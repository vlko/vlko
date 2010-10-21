using System;

namespace vlko.model.Repository.Exceptions
{
    /// <summary>
    /// RepositoryFactory not initialized exception.
    /// </summary>
    public class RepositoryFactoryNotInitializeException : Exception
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactoryNotInitializeException"/> class.
        /// </summary>
        public RepositoryFactoryNotInitializeException()
            : base("RepositoryFactory not initialized, please call RepositoryFactory.Initialize and pass valid IRepositoryFactoryResolver'.")
        {
        }
    }
}