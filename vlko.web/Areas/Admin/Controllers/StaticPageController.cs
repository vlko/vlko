using System;
using System.Web.Mvc;
using vlko.core.Authentication;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.InversionOfControl;
using vlko.core.Tools;
using vlko.model;
using vlko.model.Action.CRUDModel;
using vlko.model.Action.ViewModel;
using vlko.model.ValidationAtribute;
using vlko.model.Action;
using vlko.model.Repository;
using vlko.model.Search;

namespace vlko.web.Areas.Admin.Controllers
{
	[Authorize]
	[AreaCheck("Admin")]
	public class StaticPageController : BaseController
	{
		/// <summary>
		/// URL: Admin/StaticPage/Index
		/// </summary>
		/// <param name="pageModel">The page model.</param>
		/// <returns>Action result.</returns>
		[HttpGet]
		public ActionResult Index(PagedModel<StaticTextViewModel> pageModel)
		{
			pageModel.LoadData(RepositoryFactory.Action<IStaticTextData>().GetAll().OrderByDescending(item => item.PublishDate));
			return ViewWithAjax(pageModel);
		}

		/// <summary>
		/// URL: Admin/StaticPage/Deleted
		/// </summary>
		/// <param name="pageModel">The page model.</param>
		/// <returns>Action result.</returns>
		[HttpGet]
		[AuthorizeRoles(vlko.model.User.AdminRole)]
		public ActionResult Deleted(PagedModel<StaticTextViewModel> pageModel)
		{
			pageModel.LoadData(RepositoryFactory.Action<IStaticTextData>().GetDeleted().OrderByDescending(item => item.PublishDate));
			return ViewWithAjax(pageModel);
		}

		/// <summary>
		/// URL: Admin/StaticPage/Details
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult Details(Guid id)
		{
			var item = RepositoryFactory.Action<IStaticTextCrud>().FindByPk(id);
			return ViewWithAjax(item);
		}

		/// <summary>
		/// URL: Admin/StaticPage/Delete
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult Delete(Guid id)
		{
			return ViewWithAjax(RepositoryFactory.Action<IStaticTextCrud>().FindByPk(id));
		}

		/// <summary>
		/// URL: Admin/StaticPage/Delete
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>Action result.</returns>
		[HttpPost]
		public ActionResult Delete(StaticTextCRUDModel model)
		{
			var item = RepositoryFactory.Action<IStaticTextCrud>().FindByPk(model.Id);
			if (item.Creator.Name == UserInfo.Name
				|| UserInfo.IsAdmin())
			{
				using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
				{
					RepositoryFactory.Action<IStaticTextCrud>().Delete(model);
					tran.Commit();
					RepositoryFactory.Action<ISearchAction>().DeleteFromIndex(tran, model.Id);
				}
				return RedirectToActionWithAjax("Index");
			}
			ModelState.AddModelError(string.Empty, ModelResources.NotAllowedToDeleteError);

			return ViewWithAjax(model);
		}

		/// <summary>
		/// URL: StaticPage/Edit
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns>Action result.</returns>
		public ActionResult Edit(Guid id)
		{
			return ViewWithAjax(RepositoryFactory.Action<IStaticTextCrud>().FindByPk(id));
		}

		/// <summary>
		/// URL: Admin/StaticPage/Edit
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>Action result.</returns>
		[HttpPost]
		[AntiXss]
		public ActionResult Edit(StaticTextCRUDModel model)
		{
			if (ModelState.IsValid)
			{
				var crudOperations = RepositoryFactory.Action<IStaticTextCrud>();
				
				// get original item to test change permissions
				var originalItem = crudOperations.FindByPk(model.Id);

				if (originalItem != null)
				{
					if (originalItem.Creator.Name == UserInfo.Name
						|| UserInfo.IsAdmin())
					{

						if (string.IsNullOrWhiteSpace(model.FriendlyUrl))
						{
							model.FriendlyUrl = model.Title;
						}

						model.FriendlyUrl = GenerateUniqueFriendlyUrl(model.FriendlyUrl, model.Id);
						model.ChangeDate = DateTime.Now;
						model.Creator = RepositoryFactory.Action<IUserAction>().GetByName(UserInfo.Name);
						model.Description = GenerateDescription(model.Text);

						using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
						{
							RepositoryFactory.Action<IStaticTextCrud>().Update(model);
							tran.Commit();
							RepositoryFactory.Action<ISearchAction>().DeleteFromIndex(tran, model.Id);
							RepositoryFactory.Action<ISearchAction>().IndexStaticText(tran, model);
						}
						return RedirectToActionWithAjax("Index");
					}
					else
					{
						ModelState.AddModelError(string.Empty, ModelResources.NotAllowedToChangeError);
					}
				}
				else
				{
					ModelState.AddModelError(string.Empty, ModelResources.ItemNotExistsError);
				}
			}
			return ViewWithAjax(model);
		}

		/// <summary>
		/// URL: Admin/StaticPage/Create
		/// </summary>
		/// <returns>Action result.</returns>
		public ActionResult Create()
		{
			return ViewWithAjax(new StaticTextCRUDModel
							{
								PublishDate = DateTime.Now
							});
		}

		/// <summary>
		/// URL: Admin/StaticPage/Create
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns>Action result.</returns>
		[HttpPost]
		[AntiXss]
		public ActionResult Create(StaticTextCRUDModel model)
		{
			if (ModelState.IsValid)
			{
				var crudOperations = RepositoryFactory.Action<IStaticTextCrud>();

				if (string.IsNullOrWhiteSpace(model.FriendlyUrl))
				{
					model.FriendlyUrl = model.Title;
				}

				model.FriendlyUrl = GenerateUniqueFriendlyUrl(model.FriendlyUrl, Guid.Empty);
				model.ChangeDate = DateTime.Now;
				model.Creator = RepositoryFactory.Action<IUserAction>().GetByName(UserInfo.Name);
				model.Description = GenerateDescription(model.Text);

				using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
				{
					RepositoryFactory.Action<IStaticTextCrud>().Create(model);
					tran.Commit();
					RepositoryFactory.Action<ISearchAction>().IndexStaticText(tran, model);
				}
				return RedirectToActionWithAjax("Index");

			}
			return ViewWithAjax(model);
		}

		/// <summary>
		/// Makes the friendly URL unique.
		/// </summary>
		/// <param name="friendlyUrl">The friendly URL.</param>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		private static string GenerateUniqueFriendlyUrl(string friendlyUrl, Guid id)
		{
			friendlyUrl = FriendlyUrlGenerator.Generate(friendlyUrl);
			var dataOperations = RepositoryFactory.Action<IStaticTextData>();
			int i = 1;
			string currentFriendlyUrl = friendlyUrl;
			StaticTextViewModel equalItem = null;
			do
			{
				equalItem = dataOperations.Get(currentFriendlyUrl);
				if (equalItem != null && equalItem.Id != id)
				{
					currentFriendlyUrl = friendlyUrl + i;
					++i;
				}
			} while (equalItem != null && equalItem.Id != id);

			return currentFriendlyUrl;
		}


		/// <summary>
		/// Generates the description.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>Description text.</returns>
		private static string GenerateDescription(string text)
		{
			return text
				.RemoveTags()
				.Shorten(ModelConstants.DescriptionMaxLenghtConst);
		}
	}
}
