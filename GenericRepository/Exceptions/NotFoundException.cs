using System;

namespace GenericRepository.Exceptions
{
    /// <summary>
    /// Not found exception.
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="id">The id.</param>
        /// <param name="innerException">The inner exception.</param>
        public NotFoundException(Type objectType, object id, Exception innerException)
            : base(string.Format("Object with id '{0}' in type '{1}' not found!", id, objectType),
                   innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="id">The id.</param>
        /// <param name="additinalInfo">The additinal info.</param>
        public NotFoundException(Type objectType, object id, string additinalInfo)
            : base(string.Format("Object with id '{0}' in type '{1}' not found! Additional info: {2}", id, objectType, additinalInfo))
        {
        }
    }
}