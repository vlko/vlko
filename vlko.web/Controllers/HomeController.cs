using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace vlko.web.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
		/// <summary>
		/// URL: Home/Index
		/// </summary>
		/// <returns>Action result.</returns>
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

		/// <summary>
		/// URL: Home/About
		/// </summary>
		/// <returns>Action result.</returns>
        public ActionResult About()
        {
            return View();
        }
    }
}
