using System;
using System.Collections.Generic;
using System.Text;

namespace vlko.core.web.Commands.Model
{
    public class AuthWithRoles
    {
        public static AuthWithRoles<T> Create<T>(T result, string roles = null) 
            => new AuthWithRoles<T> { Status = result, Roles = roles };
    }
    public class AuthWithRoles<T>
    {
        public T Status { get; set; }
        public string Roles { get; set; }

        
    }
}
