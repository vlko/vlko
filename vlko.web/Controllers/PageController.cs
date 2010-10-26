using System;
using System.Collections.Generic;
using System.Web.Mvc;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.InversionOfControl;
using vlko.core.Tools;
using vlko.model;
using vlko.model.Action;
using vlko.model.Action.CRUDModel;
using vlko.model.Action.ViewModel;
using vlko.model.Repository;
using vlko.model.Search;
using vlko.model.ValidationAtribute;
using vlko.web.ViewModel.Page;

namespace vlko.web.Controllers
{
	public class PageController : BaseController
	{
		[HttpGet]
		public ActionResult Index(PagedModel<StaticTextViewModel> pageModel)
		{
			pageModel.LoadData(RepositoryFactory.Action<IStaticTextData>()
				.GetAll(DateTime.Now)
				.OrderByDescending(item => item.PublishDate));
			return ViewWithAjax(pageModel);
		}

		/// <summary>
		/// URL: Page/{friendlyUrl}
		/// </summary>
		/// <param name="friendlyUrl">The friendly URL.</param>
		/// <param name="commentsModel">The comments model.</param>
		/// <param name="sort">The sort.</param>
		/// <returns>Action result.</returns>
		public ActionResult ViewPage(string friendlyUrl, PagedModel<CommentViewModel> commentsModel, string sort)
		{
			var staticText = RepositoryFactory.Action<IStaticTextData>().Get(friendlyUrl, DateTime.Now);

			CommentViewTypeEnum sortType = ParseCommentViewType(sort);

			IEnumerable<CommentTreeViewModel> comments;
			LoadComments(sortType, out comments, commentsModel, staticText.Id);

			return ViewWithAjax(
				new PageViewModel
					{
						StaticText	= staticText,
						CommentViewType = sortType,
						FlatComments = commentsModel,
						TreeComments = comments,
						NewComment = new CommentCRUDModel
						             	{
											Name = staticText.Title,
											ContentId = staticText.Id,
						             		ChangeUser = UserInfo.User
						             	}
					});
		}


		/// <summary>
		/// URL: Page/{friendlyUrl}/Reply/{parentId}
		/// </summary>
		/// <param name="friendlyUrl">The friendly URL.</param>
		/// <param name="parentId">The parent id.</param>
		/// <param name="commentsModel">The comments model.</param>
		/// <param name="sort">The sort.</param>
		/// <returns>Action result.</returns>
		public ActionResult Reply(string friendlyUrl, Guid parentId, PagedModel<CommentViewModel> commentsModel, string sort)
		{
			var staticText = RepositoryFactory.Action<IStaticTextData>().Get(friendlyUrl, DateTime.Now);

			CommentViewTypeEnum sortType = ParseCommentViewType(sort);

			IEnumerable<CommentTreeViewModel> comments;
			LoadComments(sortType, out comments, commentsModel, staticText.Id);

			var parentComment = RepositoryFactory.Action<ICommentCrud>().FindByPk(parentId);

			return ViewWithAjax("ViewPage",
				new PageViewModel
				{
					StaticText = staticText,
					CommentViewType = sortType,
					FlatComments = commentsModel,
					TreeComments = comments,
					NewComment = new CommentCRUDModel
					{
						Name = "Re: " + parentComment.Name,
						ParentId = parentId,
						ContentId = staticText.Id,
						ChangeUser = UserInfo.User
					}
				});	
		}

		/// <summary>
		/// URL: Page/{friendlyUrl}/NewComment
		/// </summary>
		/// <param name="commentsModel">The comments model.</param>
		/// <param name="model">The model.</param>
		/// <returns>Action result.</returns>
		[HttpPost]
		[AntiXss]
		public ActionResult NewComment(PagedModel<CommentViewModel> commentsModel, CommentCRUDModel model, string sort)
		{
			var staticText = RepositoryFactory.Action<IStaticTextData>().Get(model.ContentId, DateTime.Now);
			model.ChangeUser = UserInfo.User;

			// check for anonymous name required if not logged user
			if (model.ChangeUser == null && string.IsNullOrEmpty(model.AnonymousName))
			{
				ModelState.AddModelError<CommentCRUDModel>(item => item.AnonymousName, ModelResources.AnonymousRequireError);
			}
			else if (ModelState.IsValid)
			{
				model.ChangeDate = DateTime.Now;
				model.ClientIp = Request.UserHostAddress;
				model.UserAgent = Request.UserAgent;

				// for anonymous user encode text
				if (model.ChangeUser == null)
				{
					model.Text = HtmlManipulation.RemoveTags(model.Text);
				}
				using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
				{
					RepositoryFactory.Action<ICommentCrud>().Create(model);
					tran.Commit();
					RepositoryFactory.Action<ISearchAction>().IndexComment(tran, model);
				}
				return RedirectToActionWithAjax(staticText.FriendlyUrl, routeValues: new {sort = sort});
			}

			CommentViewTypeEnum sortType = ParseCommentViewType(sort);

			IEnumerable<CommentTreeViewModel> comments;
			LoadComments(sortType, out comments, commentsModel, staticText.Id);

			return ViewWithAjax("ViewPage",
				new PageViewModel
				{
					StaticText = staticText,
					CommentViewType = sortType,
					FlatComments = commentsModel,
					TreeComments = comments,
					NewComment = model
				});
		}

		/// <summary>
		/// Parses the type of the comment view.
		/// </summary>
		/// <param name="sort">The sort.</param>
		/// <returns></returns>
		private static CommentViewTypeEnum ParseCommentViewType(string sort)
		{
			switch (sort)
			{
				case "tree":
					return CommentViewTypeEnum.Tree;
				case "desc":
					return CommentViewTypeEnum.FlatDesc;
				default:
					return CommentViewTypeEnum.Flat;
			}
		}


		/// <summary>
		/// Loads the comments.
		/// </summary>
		/// <param name="sort">The sort.</param>
		/// <param name="comments">The comments.</param>
		/// <param name="commentsModel">The comments model.</param>
		/// <param name="staticTextId">The static text id.</param>
		private static void LoadComments(CommentViewTypeEnum sort, out IEnumerable<CommentTreeViewModel> comments, PagedModel<CommentViewModel> commentsModel, Guid staticTextId)
		{
			comments = null;
			switch (sort)
			{
				case CommentViewTypeEnum.Flat:
					commentsModel.LoadData(RepositoryFactory.Action<ICommentData>().GetAllByDate(staticTextId));
					break;
				case CommentViewTypeEnum.FlatDesc:
					commentsModel.LoadData(RepositoryFactory.Action<ICommentData>().GetAllByDateDesc(staticTextId));
					break;
				case CommentViewTypeEnum.Tree:
					comments = RepositoryFactory.Action<ICommentData>().GetCommentTree(staticTextId);
					break;
			}
		}
	}
}