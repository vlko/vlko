using System;
using Castle.ActiveRecord;
using vlko.core.Roots;

namespace vlko.model.Roots
{
	/// <summary>
	/// 
	/// </summary>
	[ActiveRecord("Users")]
	public class User : IUser
	{
		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		[PrimaryKey(PrimaryKeyType.GuidComb)]
		public virtual Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[Property(Unique = true)]
		public new virtual string Name { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		[Property(Length = 64)]
		public virtual string Password { get; set; }

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>The email.</value>
		[Property(Unique = true)]
		public virtual string Email { get; set; }

		/// <summary>
		/// Gets or sets the last seen date.
		/// </summary>
		/// <value>The last seen date.</value>
		[Property]
		public virtual DateTime LastSeen { get; set; }

		/// <summary>
		/// Gets or sets the verify token.
		/// </summary>
		/// <value>The verify token.</value>
		[Property]
		public virtual string VerifyToken { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="User"/> is verified.
		/// </summary>
		/// <value><c>true</c> if verified; otherwise, <c>false</c>.</value>
		[Property]
		public virtual bool Verified { get; set; }

		/// <summary>
		/// Gets or sets the roles.
		/// </summary>
		/// <value>The roles.</value>
		[Property]
		public virtual string Roles { get; set; }
	}
}


