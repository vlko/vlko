using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericRepository.RepositoryAction
{
    /// <summary>
    /// Create action interface.
    /// </summary>
    /// <typeparam name="T">Generic type.</typeparam>
    public interface ICreateAction<T>: IAction<T> where T : class
    {
        /// <summary>
        /// Creates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Created item.</returns>
        T Create(T item);
    }
}
