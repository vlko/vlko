using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace vlko.core.web.Request
{
    /// <summary>
    /// This functionality is based on MapRouteWithName extension function which stores name in route data, please do not register default fallback routes with name.
    /// Works only with GET do not use on functions with POST processing, redirect can loose data informations.
    /// </summary>
    public class RedirectToNamedRouteAttribute : ActionFilterAttribute
    {
        private IUrlHelperFactory _urlHelper;

        public RedirectToNamedRouteAttribute()
        {

        }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            // get name of route based on registered data with MapRouteWithName
            var currentNameRoute = actionContext.RouteData.GetRouteName();
            if (string.IsNullOrEmpty(currentNameRoute))
            {
                _urlHelper = InversionOfControl.IoC.Scope.Resolve<IUrlHelperFactory>();
                // generate new url based on curretn route data
                var helper =  _urlHelper.GetUrlHelper(actionContext);
                var url = helper.RouteUrl(string.Empty, actionContext.RouteData.Values);
                // if current and original url is different redirect permanent
                if (!actionContext.HttpContext.Request.Path.Value.StartsWith(url))
                {
                    actionContext.Result = new RedirectResult(url, true);
                    return;
                }
            }
            base.OnActionExecuting(actionContext);
        }
    }
}
