using Lamar;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.Routing.Tree;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vlko.core.InversionOfControl;
using vlko.core.web.Services;

namespace vlko.core.web.Tools
{
    public static class UrlTools
    {
        public const string ProxyUrlHeader = "X-ORIGINAL";
        public const string ProxyIPHeader = "X-Forwarded-For";
        private static IRouter _router;

        public static void InitRouter(IRouteBuilder routes)
        {
            _router = routes.Build();
        }

        /// <summary>
        /// Return Form or Query value with provided indent in specified order
        /// </summary>
        public static string GetParameter(this HttpRequest request, string ident)
        {
            string result = null;
            if (request.HasFormContentType)
            {
                result = request.Form[ident];
            }
            if (string.IsNullOrEmpty(result))
            {
                result = request.Query[ident];
            }
            return result;
        }
        public static Uri GetRequestUrl(this HttpRequest request)
        {
            var proxyOriginalUrl = request.Headers[ProxyUrlHeader];
            Uri requestUrl;
            if (!string.IsNullOrEmpty(proxyOriginalUrl))
            {
                requestUrl = new Uri(proxyOriginalUrl);
            }
            else
            {
                var builder = new UriBuilder
                {
                    Scheme = request.Scheme,
                    Host = request.Host.Host
                };
                builder.Port = request.Host.Port ?? builder.Port;
                builder.Path = request.Path;
                builder.Query = request.QueryString.ToUriComponent();
                requestUrl = builder.Uri;
            }

            return requestUrl;
        }

        /// <summary>
        /// Determines whether the specified HTTP request is an AJAX request.
        /// </summary>
        ///
        /// <returns>
        /// true if the specified HTTP request is an AJAX request; otherwise, false.
        /// </returns>
        /// <param name="request">The HTTP request.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> parameter is null (Nothing in Visual Basic).</exception>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            return false;
        }


        public static IDictionary<string, object> MatchUrlToRoute(this IUrlHelper helper, string requestPath)
        {
            if (_router == null)
            {
                throw new NullReferenceException("Router notnot configured! Please configure in Startup.cs class with calling UrlTools.InitRouter passing IRoutesBuilder as parameter before first use.");
            }

            if (!string.IsNullOrEmpty(requestPath))
            {
                var uri = new Uri(requestPath);
                var matchUrl = uri.AbsolutePath;
                var rootRoute = (RouteCollection)_router;
                for (int i = 0; i < rootRoute.Count; i++)
                {
                    if (rootRoute[i] is Route route)
                    {
                        var values = new RouteValueDictionary();
                        var matcher = new TemplateMatcher(route.ParsedTemplate, route.Defaults);
                        if (matcher.TryMatch(matchUrl, values))
                        {
                            var link = helper.Link(route.Name, values);
                            if (link != null)
                            {
                                foreach (var item in QueryHelpers.ParseQuery(uri.Query))
                                {
                                    if (!values.ContainsKey(item.Key))
                                    {
                                        values.Add(item.Key, item.Value.ToString());
                                    }
                                }
                                return values;
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// This method extracts the default argument values from the template.
        /// </summary>
        private static RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
        {
            var result = new RouteValueDictionary();

            foreach (var parameter in parsedTemplate.Parameters)
            {
                if (parameter.DefaultValue != null)
                {
                    result.Add(parameter.Name, parameter.DefaultValue);
                }
            }

            return result;
        }

        /// <summary>
        /// Creates static url helper with current httpContext or from static capture content.
        /// </summary>
        /// <returns>Url helper.</returns>
        public static IUrlHelper CreateStaticUrlHelper(string baseUrl = null)
        {
            if (_router == null)
            {
                throw new NullReferenceException("Router notnot configured! Please configure in Startup.cs class with calling UrlTools.InitRouter passing IRoutesBuilder as parameter before first use.");
            }

            HttpContext httpContext = WebHostedEnvironment.CurrentHttpContext;
            if (httpContext == null || baseUrl != null)
            {
                var baseUri = new Uri(baseUrl ?? IoC.Scope.Resolve<IAppInfoService>().RootUrl);
                httpContext = new DefaultHttpContext()
                {
                    RequestServices = IoC.Scope.Resolve<IServiceProvider>(),
                    Request =
                {
                    Scheme = baseUri.Scheme,
                    Host = HostString.FromUriComponent(baseUri),
                    PathBase = PathString.FromUriComponent(baseUri),
                },
                };
            }

            var actionContext = new ActionContext
            {
                HttpContext = httpContext,
                RouteData = new RouteData { Routers = { _router } },
                ActionDescriptor = new ActionDescriptor(),
            };

            return IoC.Scope.Resolve<IUrlHelperFactory>().GetUrlHelper(actionContext);
        }

        public static string GetRemoteIpAddress(HttpRequest request)
        {
            var proxyIp = request.Headers[ProxyIPHeader].ToString();
            return !string.IsNullOrEmpty(proxyIp) ? proxyIp : request.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public static string GetRemoteUrl(HttpRequest request)
        {
            var proxyUrl = request.Headers[ProxyUrlHeader].ToString();
            return !string.IsNullOrEmpty(proxyUrl) ? proxyUrl : request.GetRequestUrl().ToString();
        }
    }
}
