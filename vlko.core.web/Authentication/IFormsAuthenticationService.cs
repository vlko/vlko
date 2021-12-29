using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace vlko.core.web.Authentication
{
    public interface IFormsAuthenticationService
    {
        bool IsActualPersistent(IEnumerable<Claim> claims);
        Task SignIn(HttpContext context, string userName, string roles, bool createPersistentCookie);
        Task SignOut(HttpContext context);
    }
}