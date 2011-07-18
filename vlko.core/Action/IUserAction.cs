using System.ComponentModel.Composition;
using vlko.core.Repository;
using vlko.core.Roots;

namespace vlko.core.Action
{
	[InheritedExport]
	public interface IUserAction : IAction<IUser>
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

		/// <summary>
		/// Gets the name of the by.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <returns>User instance.</returns>
		IUser GetByName(string username);
	}
}
