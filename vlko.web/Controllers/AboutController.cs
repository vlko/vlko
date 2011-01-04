using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vlko.core.Base;
using vlko.core.Repository;
using vlko.BlogModule.Action;

namespace vlko.web.Controllers
{
	public class AboutController : BaseController
	{
		/// <summary>
		/// URL: About/Index
		/// </summary>
		/// <returns>Action result.</returns>
		public ActionResult Index()
		{
			var staticText = RepositoryFactory.Action<IStaticTextData>().Get("about");

			if (staticText != null)
			{
				return ViewWithAjax(staticText);
			}
			return new HttpNotFoundResult();
		}
	}
}