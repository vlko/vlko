using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace vlko.core.web.Tools
{
    /// <summary>
    /// Class that renders MVC views to a string using the
    /// standard MVC View Engine to render the view.
    ///
    /// Requires that ASP.NET HttpContext is present to
    /// work, but works outside of the context of MVC
    /// </summary>
    public class RazorGenerator
    {

        private readonly IWebHostEnvironment _env;

        public RazorGenerator(IWebHostEnvironment env)
        {
            _env = env;
        }


        public async Task<string> RenderViewAsync<TModel>(Controller controller, string viewName, TModel model, IDictionary<string, object> viewBag = null, bool partial = false)
        {
            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = FindView(controller, viewName);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                if (viewBag != null)
                {
                    foreach (var pair in viewBag)
                    {
                        viewContext.ViewData[pair.Key] = pair.Value;
                    }
                }

                await viewResult.View.RenderAsync(viewContext).ConfigureAwait(false);
                return sw.ToString();
            }
        }

        public ViewEngineResult FindView(Controller controller, string viewName)
        {
            var viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
            var viewResult = viewEngine.GetView(_env.WebRootPath, viewName, false);
            return viewResult;
        }
    }
}