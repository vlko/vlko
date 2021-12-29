using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vlko.core.RavenDB.DBAccess.Commands
{
    public class CRUDCommands<T> : BaseCommands where T : class
    {

        /// <summary>
        /// Finds item by PK.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Item matching id or null if not exists.</returns>
        public T LoadById(object id)
        {
            return Session.Load<T>(id, false);
        }

        /// <summary>
        /// Saves the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Saved item.</returns>
        public T Store(T item)
        {
            Session.Store(item);
            return item;
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Delete(T item)
        {
            Session.Delete(item);
        }
    }
}
