using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenericRepository;

namespace vlko.core.Models.Action
{
    public interface ICreateAdminAction : IAction<User>
    {
        /// <summary>
        /// Creates the admin.
        /// </summary>
        /// <param name="adminName">Name of the admin.</param>
        /// <param name="adminEmail">The admin email.</param>
        /// <param name="defaultPassword">The default password.</param>
        /// <returns>
        /// True if admin created; false if admin already exists.
        /// </returns>
        bool CreateAdmin(string adminName, string adminEmail, string defaultPassword);
    }
}
