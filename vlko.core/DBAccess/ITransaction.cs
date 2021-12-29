using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vlko.core.DBAccess
{
    public interface ITransaction : IDisposable
	{
        /// <summary>
        /// Commits transaction changes.
        /// </summary>
        Task CommitAsync();

        /// <summary>
        /// Commits transaction changes.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollbacks transaction changes.
        /// </summary>
        void Rollback();
	}
}
