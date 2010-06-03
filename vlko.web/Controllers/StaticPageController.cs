using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using GenericRepository;
using vlko.core.Authentication;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.Models.Action;
using vlko.core.Models.Action.ActionModel;
using vlko.core.Models.Action.ViewModel;
using vlko.core.Tools;
using vlko.core.ValidationAtribute;
using vlko.model.IoC;

namespace vlko.web.Controllers
{
    [Authorize]
    public class StaticPageController : BaseController
    {
        /// <summary>
        /// URL: StaticPage/Index
        /// </summary>
        /// <param name="pageModel">The page model.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        public ActionResult Index(PagedModel<StaticTextViewModel> pageModel)
        {
            pageModel.LoadData(IoC.Resolve<IStaticTextData>().GetAll().OrderByDescending(item => item.PublishDate));
            return ViewWithAjax(pageModel);
        }

        /// <summary>
        /// URL: StaticPage/Deleted
        /// </summary>
        /// <param name="pageModel">The page model.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        [AuthorizeRoles(AccountValidation.AdminRole)]
        public ActionResult Deleted(PagedModel<StaticTextViewModel> pageModel)
        {
            pageModel.LoadData(IoC.Resolve<IStaticTextData>().GetDeleted().OrderByDescending(item => item.PublishDate));
            return ViewWithAjax(pageModel);
        }

        /// <summary>
        /// URL: StaticPage/Details
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Action result.</returns>
        public ActionResult Details(Guid id)
        {
            var item = IoC.Resolve<IStaticTextCrud>().FindByPk(id);
            return ViewWithAjax(item);
        }

        /// <summary>
        /// URL: StaticPage/Delete
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Action result.</returns>
        public ActionResult Delete(Guid id)
        {
            return ViewWithAjax(IoC.Resolve<IStaticTextCrud>().FindByPk(id));
        }

        /// <summary>
        /// URL: StaticPage/Delete
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult Delete(StaticTextActionModel model)
        {
            var item = IoC.Resolve<IStaticTextCrud>().FindByPk(model.Id);
            if (item.Creator.Name == UserInfo.Name
                || UserInfo.IsAdmin())
            {
                using (var tran = RepositoryFactory.StartTransaction())
                {
                    IoC.Resolve<IStaticTextCrud>().Delete(model);
                    tran.Commit();
                }
                return RedirectToActionWithAjax("Index");
            }
            ModelState.AddModelError(string.Empty, "You are not allowed to delete this item.");

            return ViewWithAjax(model);
        }

        /// <summary>
        /// URL: StaticPage/Edit
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Action result.</returns>
        public ActionResult Edit(Guid id)
        {
            return ViewWithAjax(IoC.Resolve<IStaticTextCrud>().FindByPk(id));
        }

        /// <summary>
        /// URL: StaticPage/Edit
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [AntiXss]
        public ActionResult Edit(StaticTextActionModel model)
        {
            if (ModelState.IsValid)
            {
                var crudOperations = IoC.Resolve<IStaticTextCrud>();
                
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

                        model.FriendlyUrl = GenerateUniqueFriendlyUrl(model.FriendlyUrl, Guid.Empty);
                        model.ChangeDate = DateTime.Now;
                        model.Creator = IoC.Resolve<IUserAction>().GetByName(UserInfo.Name);

                        using (var tran = RepositoryFactory.StartTransaction())
                        {
                            IoC.Resolve<IStaticTextCrud>().Update(model);
                            tran.Commit();
                        }
                        return RedirectToActionWithAjax("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "You are not allowed to change this item.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Item not exists.");
                }
            }
            return ViewWithAjax(model);
        }

        /// <summary>
        /// URL: StaticPage/Create
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult Create()
        {
            return ViewWithAjax(new StaticTextActionModel
                            {
                                PublishDate = DateTime.Now
                            });
        }

        /// <summary>
        /// URL: StaticPage/Create
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [AntiXss]
        public ActionResult Create(StaticTextActionModel model)
        {
            if (ModelState.IsValid)
            {
                var crudOperations = IoC.Resolve<IStaticTextCrud>();

                if (string.IsNullOrWhiteSpace(model.FriendlyUrl))
                {
                    model.FriendlyUrl = model.Title;
                }

                model.FriendlyUrl = GenerateUniqueFriendlyUrl(model.FriendlyUrl, Guid.Empty);
                model.ChangeDate = DateTime.Now;
                model.Creator = IoC.Resolve<IUserAction>().GetByName(UserInfo.Name);

                using (var tran = RepositoryFactory.StartTransaction())
                {
                    IoC.Resolve<IStaticTextCrud>().Create(model);
                    tran.Commit();
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
            var dataOperations = IoC.Resolve<IStaticTextData>();
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
    }
}
