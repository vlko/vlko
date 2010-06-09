using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using GenericRepository;
using vlko.core.Authentication;

namespace vlko.core.Base
{
    /// <summary>
    /// Base controller with session.
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Gets or sets the user info.
        /// </summary>
        /// <value>The user info.</value>
        public UserInfo UserInfo { get; set; }

        /// <summary>
        /// Executes the specified request context.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="requestContext"/> parameter is null.</exception>
        protected override void Execute(RequestContext requestContext)
        {
            using (RepositoryFactory.StartUnitOfWork())
            {
                UserInfo = new UserInfo(requestContext.HttpContext.User.Identity.Name);
                base.Execute(requestContext);
            }
        }

        /// <summary>
        /// Views the with ajax support.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>View.</returns>
        protected ViewResult ViewWithAjax(object model = null)
        {
            return ViewWithAjax(null, model);
        }

        /// <summary>
        /// Views the with ajax.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="model">The model.</param>
        /// <returns>View.</returns>
        protected ViewResult ViewWithAjax(string viewName, object model = null)
        {
            if (Request.IsAjaxRequest())
            {
                return View(viewName, "~/Views/Shared/Ajax.Master", model);
            }
            return View(viewName, model);
        }

        /// <summary>
        /// Redirects to action with ajax support.
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        /// <returns>Action result.</returns>
        protected ActionResult RedirectToActionWithAjax(string actionName, string controllerName = null)
        {
            if (controllerName == null)
            {
                controllerName = this.ValueProvider.GetValue("controller").RawValue.ToString();
            }
            if (Request.IsAjaxRequest())
            {
                return Json(new { actionName, controllerName}, "json");
            }
            return RedirectToAction(actionName);
        }
    }
}
