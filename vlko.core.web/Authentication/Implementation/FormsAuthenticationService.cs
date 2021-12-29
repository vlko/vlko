using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace vlko.core.web.Authentication.Implementation
{
    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public bool IsActualPersistent(IEnumerable<Claim> claims)
        {
            return claims.Any(x => x.Type == "persistent" && x.Value == "true");
        }

        public async Task SignIn(HttpContext context, string userName, string roles, bool createPersistentCookie)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            var claims = new List<Claim>
                {
                    new Claim("user", userName),
                    new Claim("persistent", createPersistentCookie ? "true" : "false")
                };
            // add roles to claims
            if (!string.IsNullOrEmpty(roles))
            {
                foreach(var role in roles.Split(','))
                {
                    claims.Add(new Claim("role", role));
                }
            }

            await context.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")),
                new AuthenticationProperties
                {
                    IsPersistent = createPersistentCookie
                });
        }

        public async Task SignOut(HttpContext context)
        {
            await context.SignOutAsync();
        }
    }
}