using System.Web.Mvc;
using System.Web.Routing;
using vlko.core.Authentication;
using vlko.core.Repository;

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
			using (var session = RepositoryFactory.StartUnitOfWork())
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
			return ViewWithAjax(ControllerContext.RouteData.Route != null ? ControllerContext.RouteData.GetRequiredString("action") : string.Empty, model);
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
				return View(viewName, "~/Views/Shared/_AjaxLayout.cshtml", model);
			}
			return View(viewName, model);
		}

		/// <summary>
		/// Redirects to action with ajax support.
		/// </summary>
		/// <param name="actionName">Name of the action.</param>
		/// <param name="controllerName">Name of the controller.</param>
		/// <param name="additionalActionLink">The additional action link.</param>
		/// <param name="routeValues">The route values.</param>
		/// <returns>Action result.</returns>
		protected ActionResult RedirectToActionWithAjax(string actionName, string controllerName = null, object routeValues = null, bool allowJsonGetRequest = false)
		{
			var result = RedirectToAction(actionName, controllerName, routeValues);
			if (Request.IsAjaxRequest())
			{
				actionName = UrlHelper.GenerateUrl(result.RouteName, null /* actionName */, null /* controllerName */, result.RouteValues, RouteTable.Routes, Request.RequestContext, false /* includeImplicitMvcValues */);
				return Json(new { actionName }, "json", allowJsonGetRequest ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet);
			}
			return result;

		}
	}
}
