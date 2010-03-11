using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericRepository
{
    public class QueryInitializeContext<T> where T : class
    {
        private readonly BaseRepository<T> _baseRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryInitializeContext&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="baseRepository">The BaseRepository.</param>
        public QueryInitializeContext(BaseRepository<T> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        /// <summary>
        /// Gets the BaseRepository.
        /// </summary>
        /// <value>The BaseRepository.</value>
        public BaseRepository<T> BaseRepository
        {
            get { return _baseRepository; }
        }
    }
}
