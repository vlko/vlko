using System;
using System.Linq;
using System.Security.Principal;

namespace vlko.core.Roots
{
	/// <summary>
	///
	/// </summary>
	public class User : IIdentity
	{
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		public virtual Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public virtual string Name { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		public virtual string Password { get; set; }

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>The email.</value>
		public virtual string Email { get; set; }

		/// <summary>
		/// Gets or sets the last seen date.
		/// </summary>
		/// <value>The last seen date.</value>
		public virtual DateTime? LastSeen { get; set; }

		/// <summary>
		/// Gets or sets the verify token.
		/// </summary>
		/// <value>The verify token.</value>
		public virtual string VerifyToken { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="User"/> is verified.
		/// </summary>
		/// <value><c>true</c> if verified; otherwise, <c>false</c>.</value>
		public virtual bool Verified { get; set; }

		/// <summary>
		/// Gets or sets the roles.
		/// </summary>
		/// <value>The roles.</value>
		public virtual string Roles { get; set; }


		/// <summary>
		/// Gets the type of authentication used.
		/// </summary>
		/// <returns>The type of authentication used to identify the user.</returns>
		public virtual string AuthenticationType
		{
			get { return "Custom"; }
		}

		/// <summary>
		/// Gets a value that indicates whether the user has been authenticated.
		/// </summary>
		/// <returns>true if the user was authenticated; otherwise, false.</returns>
		public virtual bool IsAuthenticated
		{
			get { return true; }
		}

		/// <summary>
		/// Determines whether [is user in role] [the specified role].
		/// </summary>
		/// <param name="role">The role.</param>
		/// <returns>
		///   <c>true</c> if [is user in role] [the specified role]; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IsInRole(string role)
		{
			if (!string.IsNullOrEmpty(Roles))
			{
				string[] roles = Roles.Split(',');
				return roles.Contains(role);
			}
			return false;
		}

		/// <summary>
		/// Determines whether this instance is admin.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if this instance is admin; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool IsAdmin()
		{
			return IsInRole(Settings.AdminRole);
		}
	}
}


