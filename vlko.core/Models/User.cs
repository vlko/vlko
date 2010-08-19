using System;
using Castle.ActiveRecord;

namespace vlko.core.Models
{
    /// <summary>
    /// 
    /// </summary>
    [ActiveRecord("Users")]
    public class User
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [PrimaryKey(PrimaryKeyType.GuidComb)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Property(Unique = true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [Property(Length = 64)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [Property(Unique = true)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the last seen date.
        /// </summary>
        /// <value>The last seen date.</value>
        [Property]
        public DateTime LastSeen { get; set; }

        /// <summary>
        /// Gets or sets the verify token.
        /// </summary>
        /// <value>The verify token.</value>
        [Property]
        public string VerifyToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="User"/> is verified.
        /// </summary>
        /// <value><c>true</c> if verified; otherwise, <c>false</c>.</value>
        [Property]
        public bool Verified { get; set; }

        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        /// <value>The roles.</value>
        [Property]
        public string Roles { get; set; }
    }
}


