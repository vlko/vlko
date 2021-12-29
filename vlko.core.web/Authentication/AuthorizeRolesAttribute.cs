using Microsoft.AspNetCore.Authorization;

namespace vlko.core.web.Authentication
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeRolesAttribute"/> class.
        /// </summary>
        /// <param name="roles">The roles.</param>
        public AuthorizeRolesAttribute(params string[] roles) : base()
        {
            Roles = string.Join(",", roles ?? new string[0]);
        }


    }
}
