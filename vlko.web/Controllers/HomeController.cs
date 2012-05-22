using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vlko.BlogModule.Commands;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.Repository;

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
			pageModel.PageItems = 16;
			pageModel.LoadData(RepositoryFactory.Command<ITimeline>().GetAll(DateTime.Now));
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
