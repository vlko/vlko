using System;
using System.Web.Mvc;
using vlko.model;
using vlko.core.Authentication;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.InversionOfControl;
using vlko.model.Action;
using vlko.model.Action.CRUDModel;
using vlko.model.Action.ViewModel;
using vlko.model.Repository;
using vlko.model.Search;
using vlko.model.ValidationAtribute;

namespace vlko.web.Areas.Admin.Controllers
{
	[AuthorizeRoles(vlko.model.User.AdminRole)]
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
			var data = RepositoryFactory.Action<ICommentData>().GetAllForAdmin()
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
			var item = RepositoryFactory.Action<ICommentCrud>().FindByPk(id);
			return ViewWithAjax(item);
		}

		/// <summary>
		/// URL: Admin/Comment/Delete
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult Delete(Guid id)
		{
			return ViewWithAjax(RepositoryFactory.Action<ICommentCrud>().FindByPk(id));
		}

		/// <summary>
		/// URL: Admin/Comment/Delete
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>Action result.</returns>
		[HttpPost]
		public ActionResult Delete(CommentCRUDModel model)
		{
			var item = RepositoryFactory.Action<ICommentCrud>().FindByPk(model.Id);
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				RepositoryFactory.Action<ICommentCrud>().Delete(item);
				tran.Commit();
				RepositoryFactory.Action<ISearchAction>().DeleteFromIndex(tran, item.Id);
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
			return ViewWithAjax(RepositoryFactory.Action<ICommentCrud>().FindByPk(id));
		}

		[HttpPost]
		[AntiXss]
		public ActionResult Edit(CommentCRUDModel model)
		{
			if (ModelState.IsValid)
			{
				var crudOperations = RepositoryFactory.Action<ICommentCrud>();

				// get original item to test change permissions
				var originalItem = crudOperations.FindByPk(model.Id);

				if (originalItem != null)
				{
					originalItem.Name = model.Name;
					originalItem.Text = model.Text;

					using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
					{
						crudOperations.Update(originalItem);
						tran.Commit();
						RepositoryFactory.Action<ISearchAction>().DeleteFromIndex(tran, originalItem.Id);
						RepositoryFactory.Action<ISearchAction>().IndexComment(tran, originalItem);
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