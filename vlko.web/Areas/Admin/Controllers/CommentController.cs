using System;
using System.Web.Mvc;
using GenericRepository;
using vlko.core;
using vlko.core.Authentication;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.IoC;
using vlko.core.Models.Action.ActionModel;
using vlko.core.Models.Action.ViewModel;
using vlko.core.ValidationAtribute;
using vlko.core.Models.Action;

namespace vlko.web.Areas.Admin.Controllers
{
	[AuthorizeRoles(AccountValidation.AdminRole)]
	[AreaCheck("Admin")]
	public class CommentController : BaseController
	{
		/// <summary>
		/// URL: Admin/Comment/Index
		/// </summary>
		/// <param name="pageModel">The page model.</param>
		/// <returns>Action result.</returns>
		[HttpGet]
		public ActionResult Index(PagedModel<CommentForAdminViewModel> pageModel)
		{
			var data = IoC.Resolve<ICommentData>().GetAllForAdmin()
				.OrderByDescending(comment => comment.CreatedDate);
			pageModel.LoadData(data);
			return ViewWithAjax(pageModel);
		}

		/// <summary>
		/// URL: Admin/Comment/Details
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult Details(Guid id)
		{
			var item = IoC.Resolve<ICommentCrud>().FindByPk(id);
			return ViewWithAjax(item);
		}

		/// <summary>
		/// URL: Admin/Comment/Delete
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult Delete(Guid id)
		{
			return ViewWithAjax(IoC.Resolve<ICommentCrud>().FindByPk(id));
		}

		/// <summary>
		/// URL: Admin/Comment/Delete
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>Action result.</returns>
		[HttpPost]
		public ActionResult Delete(CommentActionModel model)
		{
			var item = IoC.Resolve<ICommentCrud>().FindByPk(model.Id);
			using (var tran = RepositoryFactory.StartTransaction())
			{
				IoC.Resolve<ICommentCrud>().Delete(item);
				tran.Commit();
			}
			return RedirectToActionWithAjax("Index");
		}

		/// <summary>
		/// URL: Admin/Comment/Edit
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult Edit(Guid id)
		{
			return ViewWithAjax(IoC.Resolve<ICommentCrud>().FindByPk(id));
		}

		[HttpPost]
		[AntiXss]
		public ActionResult Edit(CommentActionModel model)
		{
			if (ModelState.IsValid)
			{
				var crudOperations = IoC.Resolve<ICommentCrud>();

				// get original item to test change permissions
				var originalItem = crudOperations.FindByPk(model.Id);

				if (originalItem != null)
				{
					originalItem.Name = model.Name;
					originalItem.Text = model.Text;

					using (var tran = RepositoryFactory.StartTransaction())
					{
						crudOperations.Update(originalItem);
						tran.Commit();
					}
					return RedirectToActionWithAjax("Index");

				}
				else
				{
					ModelState.AddModelError(string.Empty, ModelResources.ItemNotExistsError);
				}
			}
			return ViewWithAjax(model);
		}
	}
}