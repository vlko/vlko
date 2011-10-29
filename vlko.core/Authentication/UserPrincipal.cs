using System;
using System.Security.Principal;
using vlko.core.Roots;

namespace vlko.core.Authentication
{
	public class UserPrincipal : IPrincipal
	{
		private readonly User _user;

		/// <summary>
		/// Initializes a new instance of the <see cref="UserPrincipal"/> class.
		/// </summary>
		/// <param name="user">The user.</param>
		public UserPrincipal(User user)
		{
			_user = user;
		}

		/// <summary>
		/// Determines whether [is in role] [the specified role name].
		/// </summary>
		/// <param name="roleName">Name of the role.</param>
		/// <returns>
		/// 	<c>true</c> if [is in role] [the specified role name]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsInRole(string roleName)
		{
			return User.IsInRole(roleName);
		}

		/// <summary>
		/// Gets the identity of the current principal.
		/// </summary>
		/// <returns>The <see cref="T:System.Security.Principal.IIdentity"/> object associated with the current principal.</returns>
		public IIdentity Identity
		{
			get { return User; }
		}

		/// <summary>
		/// Determines whether this instance is admin.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if this instance is admin; otherwise, <c>false</c>.
		/// </returns>
		public bool IsAdmin()
		{
			return User.IsAdmin();
		}

		/// <summary>
		/// Gets the user.
		/// </summary>
		/// <value>The user.</value>
		public User User
		{
			get { return _user; }
		}
	}
}
