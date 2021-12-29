using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace vlko.core.web
{
    public static class WebHostedEnvironment
    {
        private static IHttpContextAccessor _httpContextAccessor;
        private static IHostEnvironment _env;


        public static void Configure(IHttpContextAccessor httpContextAccessor, IHostEnvironment env)
        {
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }


        public static HttpContext CurrentHttpContext => _httpContextAccessor?.HttpContext;

        public static bool IsDevelMode => _env?.IsDevelopment() ?? false;
        public static bool IsBetaMode => _env?.EnvironmentName.StartsWith("Beta") ?? false;

        public static IFileInfo GetContentFile(string relativePath)
        {
            if (_env == null)
            {
                throw new NullReferenceException("HttpContextStatic class is not configured, please configure before first use.");
            }
            return _env.ContentRootFileProvider.GetFileInfo(relativePath);
        }
        public static IFileInfo GetWWWContentFile(string relativePath)
        {
            if (_env == null)
            {
                throw new NullReferenceException("HttpContextStatic class is not configured, please configure before first use.");
            }
            var transformedPath = relativePath.StartsWith("~/") ? relativePath.Substring(2)
                                : relativePath.TrimStart('/', '\\');
            var transformedRootPath = "wwwroot" + (relativePath.Contains("/") ? "/" : "\\");
            return _env.ContentRootFileProvider.GetFileInfo(Path.Combine(transformedRootPath, transformedPath));
        }

        public static IDirectoryContents GetDirectoryContent(string relativePath)
        {
            if (_env == null)
            {
                throw new NullReferenceException("HttpContextStatic class is not configured, please configure before first use.");
            }
            return _env.ContentRootFileProvider.GetDirectoryContents(relativePath);
        }

        public static string GetContentRoot()
        {
            if (_env == null)
            {
                throw new NullReferenceException("HttpContextStatic class is not configured, please configure before first use.");
            }
            return _env.ContentRootPath;
        }

        public static string ReadAllText(this IFileInfo file)
        {
            StreamReader reader = new StreamReader(file.CreateReadStream());
            return reader.ReadToEnd();
        }
    }
}