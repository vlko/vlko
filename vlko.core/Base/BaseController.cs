using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using vlko.core.Action;
using vlko.core.Authentication;
using vlko.core.Repository;
using vlko.core.Roots;

namespace vlko.core.Base
{
	/// <summary>
	/// Base controller with session.
	/// </summary>
	public class BaseController : Controller
	{
		/// <summary>
		/// Gets the user info (returns null if user not authenticated).
		/// </summary>
		public User CurrentUser
		{
			get { return User is UserPrincipal ? ((UserPrincipal) User).User : null; }
		}

		/// <summary>
		/// Executes the specified request context.
		/// </summary>
		/// <param name="requestContext">The request context.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestContext"/> parameter is null.</exception>
		protected override void Execute(RequestContext requestContext)
		{
			using (var session = RepositoryFactory.StartUnitOfWork())
			{
				base.Execute(requestContext);
			}
		}

		/// <summary>
		/// Called when authorization occurs.
		/// </summary>
		/// <param name="filterContext">Information about the current request and action.</param>
		protected override void OnAuthorization(AuthorizationContext filterContext)
		{
			base.OnAuthorization(filterContext);
			// if user is authenticated then set custom user principal
			if (filterContext.HttpContext.User.Identity.IsAuthenticated)
			{
				var user = InversionOfControl.IoC.Resolve<IUserAction>().GetByName(filterContext.HttpContext.User.Identity.Name);
				filterContext.HttpContext.User = new UserPrincipal(user);
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
		/// <param name="routeValues">The route values.</param>
		/// <param name="allowJsonGetRequest">if set to <c>true</c> [allow json get request].</param>
		/// <returns>
		/// Action result.
		/// </returns>
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



		/// <summary>
		/// Gets the model errors in ajax format.
		/// </summary>
		/// <returns>The model errors in ajax format.</returns>
		public AjaxModelErrorInfo[] GetAjaxModelErrors()
		{
			return ModelState.AsEnumerable()
				.Where(item => item.Value.Errors.Count > 0)
				.Select(
					item =>
					new AjaxModelErrorInfo {Field = item.Key, Errors = item.Value.Errors.Select(error => error.ErrorMessage).ToArray()})
				.ToArray();
		}
	}
}
