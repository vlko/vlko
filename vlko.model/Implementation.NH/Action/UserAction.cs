using System;
using System.Linq;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using vlko.core.Action;
using vlko.core.Roots;
using vlko.model.Action;
using vlko.core.Repository;
using vlko.model.Roots;

namespace vlko.model.Implementation.NH.Action
{
	public class UserAction : BaseAction<IUser>, IUserAction
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
        public bool CreateAdmin(string adminName, string adminEmail, string defaultPassword)
        {
            using (RepositoryFactory.StartUnitOfWork())
            {
                // check for unique username
				if (ActiveRecordLinqBase<User>.Queryable.FirstOrDefault(user => user.Name == adminName) != null)
                {
                    return false;
                }

                User newUser = new User()
                                   {
                                       Name = adminName,
                                       Email = adminEmail,
                                       Password = UserAuthentication.HashPassword(defaultPassword),
                                       LastSeen = DateTime.Now,
                                       Roles = Settings.AdminRole,
                                       Verified = true,

                                   };
                using (var tran = RepositoryFactory.StartTransaction())
                {
                    ActiveRecordMediator<User>.Create(newUser);
                    tran.Commit();
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the name of the by.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>User instance.</returns>
        public IUser GetByName(string username)
        {
            return ActiveRecordLinq.AsQueryable<User>().FirstOrDefault(user => user.Name == username);
        }
    }
}