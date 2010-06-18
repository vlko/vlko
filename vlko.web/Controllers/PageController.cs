using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.Models.Action;
using vlko.core.Models.Action.ViewModel;
using vlko.model.IoC;

namespace vlko.web.Controllers
{
	public class PageController : BaseController
	{
		[HttpGet]
		public ActionResult Index(PagedModel<StaticTextViewModel> pageModel)
		{
			pageModel.LoadData(IoC.Resolve<IStaticTextData>()
				.GetAll(DateTime.Now)
				.OrderByDescending(item => item.PublishDate));
			return ViewWithAjax(pageModel);
		}

		/// <summary>
		/// URL: Page/View
		/// </summary>
		/// <param name="friendlyUrl">The friendly URL.</param>
		/// <returns>Action result.</returns>
		public ActionResult View(string friendlyUrl)
		{
			var staticText = IoC.Resolve<IStaticTextData>().Get(friendlyUrl, DateTime.Now);
			return ViewWithAjax(staticText);
		}
	}
}