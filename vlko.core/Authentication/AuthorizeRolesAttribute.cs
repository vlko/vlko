using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using GenericRepository;

namespace vlko.core.Authentication
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        private readonly string[] _roles;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeRolesAttribute"/> class.
        /// </summary>
        /// <param name="roles">The roles.</param>
        public AuthorizeRolesAttribute(params string[] roles)
        {
            _roles = roles;
        }

        /// <summary>
        /// Determines whether access to the core framework is authorized.
        /// </summary>
        /// <param name="httpContext">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param>
        /// <returns>
        /// true if access is authorized; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="httpContext"/> parameter is null.</exception>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var result = base.AuthorizeCore(httpContext);
            if (result)
            {
                var userAuthenticationService = IoC.IoC.Resolve<IUserAuthenticationService>();
                foreach (var role in _roles)
                {
                    var isInRole = userAuthenticationService.IsUserInRole(httpContext.User.Identity.Name, role);
                    if (isInRole)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Processes HTTP requests that fail authorization.
        /// </summary>
        /// <param name="filterContext">Encapsulates the information for using <see cref="T:System.Web.Mvc.AuthorizeAttribute"/>. The <paramref name="filterContext"/> object contains the controller, HTTP context, request context, action result, and route data.</param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new {controller = "Account", action = "NotAuthorized"}));
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}
