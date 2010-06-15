using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.Models.Action.ViewModel;
using vlko.model.IoC;
using vlko.core.Models.Action;

namespace vlko.web.Controllers
{
	public class CommentController : BaseController
	{
		/// <summary>
		/// URL: Comment/Index
		/// </summary>
		/// <param name="pageModel">The page model.</param>
		/// <returns>Action result.</returns>
		[HttpGet]
		public ActionResult Index(PagedModel<CommentForAdminViewModel> pageModel)
		{
			pageModel.LoadData(IoC.Resolve<ICommentData>().GetAllForAdmin());
			return ViewWithAjax(pageModel);
		}

		/// <summary>
		/// URL: Comment/Details
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult Details(Guid id)
		{
			var item = IoC.Resolve<ICommentCrud>().FindByPk(id);
			return ViewWithAjax(item);
		}

		/// <summary>
		/// URL: Comment/Delete
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult Delete(Guid id)
		{
			return ViewWithAjax(IoC.Resolve<ICommentCrud>().FindByPk(id));
		}

		/// <summary>
		/// URL: Comment/Edit
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult Edit(Guid id)
		{
			return ViewWithAjax(IoC.Resolve<ICommentCrud>().FindByPk(id));
		}
	}
}