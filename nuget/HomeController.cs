using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.Repository;

namespace $safeprojectname$.web.Controllers
{
	[HandleError]
	public class HomeController : BaseController
	{
		/// <summary>
		/// URL: Home/Index
		/// </summary>
		/// <returns>Action result.</returns>
		public ActionResult Index()
		{
			ViewBag.Message = "Welcome vlko/vlko template!";

			return View();
		}
	}
}