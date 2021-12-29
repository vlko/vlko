using Lamar;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace vlko.core.web.HtmlExtender
{

    public abstract class Bundle
    {
        private static Logger _logger = LogManager.GetLogger("Bundles");
        public class BundleFile
        {
            public string RelativePath { get; set; }
            public IFileInfo FileInfo { get; set; }
            public string Hash { get; set; }
        }
        public bool UseIncludeInsteadOfProdFile { get; set; }
        public string Name { get; protected set; }
        public IList<BundleFile> Files { get; protected set; }
        public BundleFile ProdFile { get; protected set; }

        public Bundle(string name)
        {
            Name = name;
            Files = new List<BundleFile>();
        }

        public Bundle Prod(string relativePath, string rootPath = "wwwroot")
        {
            if (ProdFile != null)
            {
                throw new Exception($"[Bundle {Name}] Only single Prod file allowed!");
            }
            if (UseIncludeInsteadOfProdFile)
            {
                throw new Exception($"[Bundle {Name}] Prod file cannot be used with ProdInclude! Use separate Prod and Include.");
            }
            if (!string.IsNullOrEmpty(relativePath))
            {
                var fileInfo = GetFileInfo(rootPath, relativePath);
                if (fileInfo.Exists)
                {
                    ProdFile = new BundleFile { RelativePath = relativePath, FileInfo = fileInfo };
                    ProdFile.Hash = GenerateHash(fileInfo);
                }
                else
                {
                    _logger.Warn($"Bundle {Name} has missing prod file '{relativePath}'");
                }
            }
            return this;
        }

        public Bundle ProdInclude(string rootPath, params string[] files)
        {
            if (ProdFile != null)
            {
                throw new Exception($"[Bundle {Name}] ProdInclude cannot be used with Prod. Use separate Prod and Include.");
            }
            UseIncludeInsteadOfProdFile = true;
            if (files != null && files.Any(x => !string.IsNullOrEmpty(x)))
            {
                foreach (string file in files.Where(x => !string.IsNullOrEmpty(x)))
                {
                    Include(rootPath, file);
                }
            }
            return this;
        }

        public Bundle Include(string rootPath, params string[] files)
        {
            if (UseIncludeInsteadOfProdFile)
            {
                throw new Exception($"[Bundle {Name}] Include files cannot be used ProdInclude! Use separate Prod and Include.");
            }
            if (ProdFile == null)
            {
                throw new Exception($"[Bundle {Name}] Use Prod before Include or use ProdInclude if you do not have separate production file.");
            }
            // only include in devel mode
            if (WebHostedEnvironment.IsDevelMode)
            {
                if (files != null && files.Any(x => !string.IsNullOrEmpty(x)))
                {
                    foreach (string file in files.Where(x => !string.IsNullOrEmpty(x)))
                    {
                        Include(rootPath, file);
                    }
                }
            }
            return this;
        }

        public IEnumerable<BundleFile> GetFiles()
        {
            if (!WebHostedEnvironment.IsDevelMode && ProdFile == null && !UseIncludeInsteadOfProdFile)
            {
                _logger.Warn($"Bundle {Name} has missing prod file");
            }
            return (WebHostedEnvironment.IsDevelMode && Files.Count > 0) || (ProdFile == null) 
                ? Files
                :  new[] { ProdFile };
        }

        private string GenerateHash(IFileInfo file)
        {
            if (file.Exists)
            {
                using (var md5 = System.Security.Cryptography.MD5.Create())
                using (var stream = file.CreateReadStream())
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var i in md5.ComputeHash(stream))
                    {
                        sb.Append(i.ToString("X2"));
                    }
                    return sb.ToString();
                }
            }
            return null;
        }

        private void Include(string rootPath, string relativePath)
        {
            if (!string.IsNullOrEmpty(relativePath))
            {
                IFileInfo fileInfo = GetFileInfo(rootPath, relativePath);
                if (!fileInfo.Exists)
                {
                    _logger.Warn($"Bundle {Name} has missing include file '{relativePath}'");
                }
                if (fileInfo.Exists && !Files.Any(x => x.RelativePath == relativePath))
                {
                    var newFile = new BundleFile { RelativePath = relativePath, FileInfo = fileInfo };
                    newFile.Hash = GenerateHash(fileInfo);
                    Files.Add(newFile);
                }
            }
        }

        private static IFileInfo GetFileInfo(string rootPath, string relativePath)
        {
            var transformedPath = relativePath.StartsWith("~/") ? relativePath.Substring(2)
                                : relativePath.TrimStart('/', '\\');
            var transformedRootPath = rootPath.TrimEnd('/', '\\') + (relativePath.Contains("/") ? "/" : "\\");
            var fileInfo = WebHostedEnvironment.GetContentFile(Path.Combine(transformedRootPath, transformedPath));
            return fileInfo;
        }
    }

    public class StyleBundle : Bundle
    {
        public StyleBundle(string name) : base(name)
        {
        }
    }

    public class ScriptBundle : Bundle
    {
        public bool IsAsync { get; protected set; }
        public ScriptBundle(string name, bool async = false) : base(name)
        {
            Async(async);
        }

        public Bundle Async(bool async = false)
        {
            IsAsync = async;
            return this;
        }
    }

    public static class BundlesRegister
    {
        private static Dictionary<string, Bundle> Bundles { get; set; } = new Dictionary<string, Bundle>();

        public static Bundle CreateScriptBundle(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var existingBundle = Get(name);
                if (existingBundle == null)
                {
                    var bundle = new ScriptBundle(name);
                    Bundles.Add(name, bundle);
                    return bundle;
                }
                return existingBundle;
            }
            return null;
        }

        public static Bundle CreateStyleBundle(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var existingBundle = Get(name);
                if (existingBundle == null)
                {
                    var bundle = new StyleBundle(name);
                    Bundles.Add(name, bundle);
                    return bundle;
                }
                return existingBundle;
            }
            return null;
        }

        public static Bundle Get(string name)
        {
            if (Bundles.ContainsKey(name))
            {
                return Bundles[name];
            }
            return null;
        }
    }

    public static class StylesExtensions
    {
        public static HtmlString UseStyles(this IUrlHelper urlHelper, string bundleName)
        {
            var bundle = BundlesRegister.Get(bundleName);
            if (bundle != null && bundle is StyleBundle)
            {

                var result = new StringBuilder();
                var files = bundle.GetFiles();
                if (files != null && files.Any())
                {
                    foreach (var file in files)
                    {
                        result.Append(CssManager.RenderStylesheetLink(urlHelper, file.RelativePath, WebHostedEnvironment.IsDevelMode ? null : file.Hash));
                    }
                }

                return new HtmlString(result.ToString());
            }
            return new HtmlString(CssManager.RenderStylesheetLink(urlHelper, bundleName, null));
        }
    }

    public static class Scripts
    {
        public static HtmlString UseScripts(this IUrlHelper urlHelper, string bundleName)
        {
            var bundle = BundlesRegister.Get(bundleName);
            if (bundle != null && bundle is ScriptBundle)
            {
                var result = new StringBuilder();
                var files = bundle.GetFiles();
                if (files != null && files.Any())
                {
                    foreach (var file in files)
                    {
                        result.Append(ScriptManager.RenderScriptContent(urlHelper, file.RelativePath,
                            async: (bundle as ScriptBundle).IsAsync,
                            hash: WebHostedEnvironment.IsDevelMode ? null : file.Hash));
                    }
                }
                return new HtmlString(result.ToString());
            }
            return new HtmlString(ScriptManager.RenderScriptContent(urlHelper, bundleName, true, null, null));
        }
    }
}
