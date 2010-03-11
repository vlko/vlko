using System;
using Castle.ActiveRecord;

namespace vlko.core.Models
{
    [ActiveRecord]
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
        [Property]
        public Int32 Password { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [Property]
        public string Email { get; set; }
    }
}


