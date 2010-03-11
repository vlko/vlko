using System;

namespace GenericRepository.Exceptions
{
    /// <summary>
    /// RepositoryIoC not initialized exception.
    /// </summary>
    public class RepositoryIoCNotInitializeException : Exception
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryIoCNotInitializeException"/> class.
        /// </summary>
        public RepositoryIoCNotInitializeException()
            : base("RepositoryIoC not initialized, please call RepositoryIoC.Initialize and pass valid IIocResolver'.")
        {
        }
    }
}