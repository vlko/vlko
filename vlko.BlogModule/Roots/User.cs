using System;
using vlko.core.Roots;

namespace vlko.BlogModule.Roots
{
	/// <summary>
	/// 
	/// </summary>
	public class User : IUser
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
		public new virtual string Name { get; set; }

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
	}
}


