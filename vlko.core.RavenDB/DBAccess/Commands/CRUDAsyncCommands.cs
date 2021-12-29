using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vlko.core.RavenDB.DBAccess.Commands
{
    public class CRUDAsyncCommands<T> : BaseAsyncCommands where T : class
    {

        /// <summary>
        /// Finds item by PK.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Item matching id or null if not exists.</returns>
        public async Task<T> LoadById(object id)
        {
            return await Session.LoadAsync<T>(id, false);
        }

        /// <summary>
        /// Saves the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Saved item.</returns>
        public async Task<T> Store(T item)
        {
            await Session.StoreAsync(item);
            return item;
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public async Task Delete(T item)
        {
            await Session.DeleteAsync(item);
        }
    }
}
