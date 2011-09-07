using System;
using System.Linq;
using vlko.core.Action;
using vlko.core.NH.Repository;
using vlko.core.Repository;
using vlko.core.Roots;

namespace vlko.core.NH.Action
{
	public class UserCommands : CommandGroup<User>, IUserCommands
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
				if (SessionFactory<User>.Queryable.FirstOrDefault(user => user.Name == adminName) != null)
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
					SessionFactory<User>.Create(newUser);
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
		public User GetByName(string username)
		{
			return SessionFactory<User>.Queryable.FirstOrDefault(user => user.Name == username);
		}
	}
}