using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.Repository;
using vlko.BlogModule.Action;

namespace vlko.web.Controllers
{
	[HandleError]
	public class HomeController : BaseController
	{
		/// <summary>
		/// URL: Home/Index
		/// </summary>
		/// <returns>Action result.</returns>
		public ActionResult Index(PagedModel<object> pageModel)
		{
			pageModel.LoadData(RepositoryFactory.Action<ITimeline>().GetAll(DateTime.Now));
			return ViewWithAjax(pageModel);
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
