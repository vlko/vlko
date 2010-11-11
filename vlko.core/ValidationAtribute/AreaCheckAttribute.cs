using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace vlko.core.ValidationAtribute
{
	/// <summary>
	/// Attribute to check if controller is in area.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class AreaCheckAttribute : ActionFilterAttribute
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="AreaCheckAttribute"/> class.
		/// </summary>
		/// <param name="areaName">Name of the area.</param>
		public AreaCheckAttribute(string areaName)
		{
			AreaName = areaName;
		}

		/// <summary>
		/// Gets or sets the name of the area.
		/// </summary>
		/// <value>The name of the area.</value>
		public string AreaName { get; set; }

		/// <summary>
		/// Called by the MVC framework before the action method executes.
		/// </summary>
		/// <param name="filterContext">The filter context.</param>
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var areaToken = filterContext.RouteData.DataTokens["area"];
			if (areaToken == null || AreaName != areaToken.ToString())
			{
				throw new HttpException((int)HttpStatusCode.NotFound, "File not found");
			}
			base.OnActionExecuting(filterContext);
		}
	}
}
