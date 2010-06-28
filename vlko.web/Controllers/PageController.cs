using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using GenericRepository;
using vlko.core;
using vlko.core.Authentication;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.IoC;
using vlko.core.Models.Action;
using vlko.core.Models.Action.ActionModel;
using vlko.core.Models.Action.ViewModel;
using vlko.core.ValidationAtribute;
using vlko.web.ViewModel.Page;

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
		/// <param name="commentsModel">The comments model.</param>
		/// <returns>Action result.</returns>
		public ActionResult ViewPage(string friendlyUrl, PagedModel<CommentViewModel> commentsModel)
		{
			var staticText = IoC.Resolve<IStaticTextData>().Get(friendlyUrl, DateTime.Now);
			var comments = IoC.Resolve<ICommentData>().GetAllByDate(staticText.Id);
			return ViewWithAjax(
				new PageViewModel
					{
						StaticText	= staticText,
						CommentViewType = CommentViewTypeEnum.Flat,
						FlatComments = commentsModel.LoadData(comments),
						NewComment = new CommentActionModel
						             	{
											Name = staticText.Title,
											ContentId = staticText.Id,
						             		ChangeUser = UserInfo.User
						             	}
					});
		}

		/// <summary>
		/// URL: Page/NewComment
		/// </summary>
		/// <param name="commentsModel">The comments model.</param>
		/// <param name="model">The model.</param>
		/// <returns>Action result.</returns>
		[HttpPost]
		[AntiXss]
		public ActionResult NewComment(PagedModel<CommentViewModel> commentsModel, CommentActionModel model)
		{
			var staticText = IoC.Resolve<IStaticTextData>().Get(model.ContentId, DateTime.Now);
			model.ChangeUser = UserInfo.User;

			// check for anonymous name required if not logged user
			if (model.ChangeUser == null && string.IsNullOrEmpty(model.AnonymousName))
			{
				ModelState.AddModelError("AnonymousName", ModelResources.AnonymousRequireError);
			}
			else if (ModelState.IsValid)
			{
				model.ChangeDate = DateTime.Now;
				model.ClientIp = Request.UserHostAddress;
				model.UserAgent = Request.UserAgent;

				// for anonymous user encode text
				if (model.ChangeUser == null)
				{
					model.Text = Regex.Replace(model.Text, @"<(.|\n)*?>", string.Empty);
				}
				using (var tran = RepositoryFactory.StartTransaction())
				{
					IoC.Resolve<ICommentCrud>().Create(model);
					tran.Commit();
				}
				return RedirectToActionWithAjax(staticText.FriendlyUrl);
			}
			var comments = IoC.Resolve<ICommentData>().GetAllByDate(staticText.Id);
			return ViewWithAjax("ViewPage",
				new PageViewModel
				{
					StaticText = staticText,
					CommentViewType = CommentViewTypeEnum.Flat,
					FlatComments = commentsModel.LoadData(comments),
					NewComment = model
				});
		}
	}
}