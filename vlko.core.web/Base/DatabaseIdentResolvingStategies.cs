using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using vlko.core.web.Tools;

namespace vlko.core.web.Base
{
    public static class DatabaseIdentResolvingStategies
    {
        public static string ResolveBySubdomain(HttpRequest request)
        {
            string resolvedIdent = null;
            if (request != null)
            {
                var proxyOriginalUrl = request.Headers[UrlTools.ProxyUrlHeader];
                try
                {
                    if (!string.IsNullOrEmpty(proxyOriginalUrl))
                    {
                        var requestUrl = new Uri(proxyOriginalUrl);
                        resolvedIdent = requestUrl?.Host?.ToLower().Split(new[] { '.' }, 2)[0];
                    }
                    else
                    {
                        resolvedIdent = request.Host.Host?.ToLower().Split(new[] { '.' }, 2)[0];
                    }
                }
                catch (UriFormatException)
                {
                    NLog.LogManager.GetLogger("DatabaseIdentResolvingStategies").Error($"UriFormatException for URI: {request.GetDisplayUrl()}; {UrlTools.ProxyUrlHeader}: {proxyOriginalUrl}");
                }
            }
            if (WebHostedEnvironment.IsBetaMode && resolvedIdent == "betacz")
            {
                resolvedIdent = "cz";
            }
            return resolvedIdent;
        }
    }
}
