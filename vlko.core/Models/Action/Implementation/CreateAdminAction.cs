using System;
using System.Linq;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Linq;
using GenericRepository;

namespace vlko.core.Models.Action.Implementation
{
    public class CreateAdminAction : ICreateAdminAction
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="IAction&lt;T&gt;"/> is initialized.
        /// </summary>
        /// <value><c>true</c> if initialized; otherwise, <c>false</c>.</value>
        public bool Initialized { get; set; }

        /// <summary>
        /// Initializes queryAction with the specified repository.
        /// </summary>
        /// <param name="initializeContext">The initialize context.</param>
        public void Initialize(InitializeContext<User> initializeContext)
        {
            Initialized = true;
        }

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
                if (ActiveRecordLinq.AsQueryable<User>().FirstOrDefault(user => user.Name == adminName) != null)
                {
                    return false;
                }

                User newUser = new User()
                                   {
                                       Name = adminName,
                                       Email = adminEmail,
                                       Password = UserAuthentication.HashPassword(defaultPassword),
                                       LastSeen = DateTime.Now,
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
    }
}