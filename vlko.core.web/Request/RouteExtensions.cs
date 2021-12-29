using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;

namespace vlko.core.web.Request
{
    public static class RouteExtensions
    {
        public const string RouteNameIdent = "_RouteName_";


        /// <summary>
        /// Do the same as MapRoute, but register name of route do route data..
        /// </summary>
        public static IRouteBuilder MapRouteWithName(this IRouteBuilder routes, string name, string template, object defaults, object constraints = null)
        {
            var builder = routes.MapRoute(name, template, defaults, constraints);
            Route route = (Route)builder.Routes.Last();
            route.DataTokens.Add(RouteNameIdent, name);

            return builder;
        }

        /// <summary>
        /// Gets the name of the route from routes registered with MapRouteWithName
        /// </summary>
        public static string GetRouteName(this RouteData route)
        {
            return route.DataTokens.ContainsKey(RouteNameIdent)
                ? route.DataTokens[RouteNameIdent] as string
                : null;
        }

        public static void AddMultiple<T>(this RouteValueDictionary routeDictionary, string ident, T[] values)
        {
            if (values != null)
            {
                if (values.Length == 1)
                {
                    routeDictionary.Add(ident, values[0]);
                }
                else
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        var k = string.Format("{0}[{1}]", ident, i);
                        routeDictionary.Add(k, values[i]);
                    }
                }
            }
        }
    }
}
