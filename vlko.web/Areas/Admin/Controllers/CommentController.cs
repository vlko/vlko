using System;
using System.Web.Mvc;
using vlko.core.Repository;
using vlko.core.ValidationAtribute;
using vlko.BlogModule;
using vlko.core.Authentication;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.InversionOfControl;
using vlko.BlogModule.Action;
using vlko.BlogModule.Action.CRUDModel;
using vlko.BlogModule.Action.ViewModel;
using vlko.BlogModule.Search;

namespace vlko.web.Areas.Admin.Controllers
{
	[AuthorizeRoles(Settings.AdminRole)]
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
			var data = RepositoryFactory.Command<ICommentData>().GetAllForAdmin()
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
			var item = RepositoryFactory.Command<ICommentCrud>().FindByPk(id);
			return ViewWithAjax(item);
		}

		/// <summary>
		/// URL: Admin/Comment/Delete
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult Delete(Guid id)
		{
			return ViewWithAjax(RepositoryFactory.Command<ICommentCrud>().FindByPk(id));
		}

		/// <summary>
		/// URL: Admin/Comment/Delete
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>Action result.</returns>
		[HttpPost]
		public ActionResult Delete(CommentCRUDModel model)
		{
			var item = RepositoryFactory.Command<ICommentCrud>().FindByPk(model.Id);
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				RepositoryFactory.Command<ICommentCrud>().Delete(item);
				tran.Commit();
				RepositoryFactory.Command<ISearchCommands>().DeleteFromIndex(tran, item.Id.ToString());
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
			return ViewWithAjax(RepositoryFactory.Command<ICommentCrud>().FindByPk(id));
		}

		[HttpPost]
		[AntiXss]
		public ActionResult Edit(CommentCRUDModel model)
		{
			if (ModelState.IsValid)
			{
				var crudOperations = RepositoryFactory.Command<ICommentCrud>();

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
						RepositoryFactory.Command<ISearchCommands>().DeleteFromIndex(tran, originalItem.Id.ToString());
						RepositoryFactory.Command<ISearchCommands>().IndexComment(tran, originalItem);
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