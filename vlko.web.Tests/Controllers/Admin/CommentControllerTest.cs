using System;
using System.Linq;
using System.Web.Mvc;
using MvcContrib.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core.Action;
using vlko.core.Components;
using vlko.core.Repository;
using vlko.BlogModule.Action;
using vlko.BlogModule.Action.CRUDModel;
using vlko.BlogModule.Action.ViewModel;
using vlko.web.Areas.Admin.Controllers;

namespace vlko.web.Tests.Controllers.Admin
{
	[TestClass]
	public class CommentControllerTest : BaseControllerTest
	{
		public static int NumberOfGeneratedItems = 100;

		/// <summary>
		/// Fills the db with data.
		/// </summary>
		protected override void FillDbWithData()
		{
			// fill db with data
			using (var tran = RepositoryFactory.StartTransaction())
			{

				RepositoryFactory.Command<IUserCommands>().CreateAdmin("vlko", "vlko@zilina.net", "test");
				var admin = RepositoryFactory.Command<IUserCommands>().GetByName("vlko");
				for (int i = 0; i < NumberOfGeneratedItems; i++)
				{
					var text = RepositoryFactory.Command<IStaticTextCrud>().Create(
						new StaticTextCRUDModel
						{
							AllowComments = false,
							Creator = admin,
							Title = "StaticPage" + i,
							FriendlyUrl = "staticpage" + (i == NumberOfGeneratedItems - 1 ? string.Empty : i.ToString()),
							ChangeDate = DateTime.Now,
							PublishDate = DateTime.Now.AddDays(-i),
							Text = "Static page" + i
						});
					RepositoryFactory.Command<ICommentCrud>().Create(
						new CommentCRUDModel()
						{
							AnonymousName = "User",
							ChangeDate = DateTime.Now.AddDays(-i),
							ClientIp = "127.0.0.1",
							ContentId = text.Id,
							Name = "Comment" + i,
							Text = "Static page" + i,
							UserAgent = "Mozzilla"
						});
				}
				tran.Commit();
			}
		}

		[TestMethod]
		public void Index()
		{
			// Arrange
			CommentController controller = new CommentController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);

			// Act
			ActionResult result = controller.Index(new PagedModel<CommentForAdminViewModel>());

			// Assert
			var model = result.AssertViewRendered()
				.WithViewData<PagedModel<CommentForAdminViewModel>>();

			Assert.AreEqual(NumberOfGeneratedItems, model.Count);

			int i = 0;
			foreach (var comment in model)
			{
				Assert.AreEqual("Comment" + i, comment.Name);
				Assert.AreEqual("Static page" + i, comment.Text);
				++i;
			}
			Assert.AreEqual(PagedModel<StaticTextViewModel>.DefaultPageItems, i);
		}

		[TestMethod]
		public void Details()
		{
			// Arrange
			CommentController controller = new CommentController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);

			var id = RepositoryFactory.Command<ICommentData>().GetAllForAdmin()
				.OrderBy(item => item.Name)
				.ToPage(1, 1).First().Id;

			// Act
			ActionResult result = controller.Details(id);

			// Assert
			var model = result.AssertViewRendered().WithViewData<CommentCRUDModel>();

			Assert.AreEqual(id, model.Id);
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			CommentController controller = new CommentController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);

			var id = RepositoryFactory.Command<ICommentData>().GetAllForAdmin()
				.OrderBy(item => item.Name)
				.ToPage(1, 1).First().Id;

			// Act
			ActionResult result = controller.Delete(id);

			// Assert
			var model = result.AssertViewRendered().WithViewData<CommentCRUDModel>();

			Assert.AreEqual(id, model.Id);
		}

		[TestMethod]
		public void Delete_post_success()
		{
			// Arrange
			CommentController controller = new CommentController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);

			var data = RepositoryFactory.Command<ICommentData>().GetAllForAdmin()
				.OrderBy(item => item.Name);
			var count = data.Count();
				
			var dataModel = RepositoryFactory.Command<ICommentCrud>().FindByPk(data.ToPage(1, 1).First().Id);

			// Act
			ActionResult result = controller.Delete(dataModel);

			// Assert
			result.AssertActionRedirect().ToAction("Index");

			Assert.AreEqual(count - 1, data.Count());
		}

		[TestMethod]
		public void Edit()
		{
			// Arrange
			CommentController controller = new CommentController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);

			var id = RepositoryFactory.Command<ICommentData>().GetAllForAdmin()
				.OrderBy(item => item.Name)
				.ToPage(1, 1).First().Id;

			// Act
			ActionResult result = controller.Edit(id);

			// Assert
			var model = result.AssertViewRendered().WithViewData<CommentCRUDModel>();

			Assert.AreEqual(id, model.Id);
		}

		[TestMethod]
		public void Edit_post_success()
		{
			// Arrange
			CommentController controller = new CommentController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);

			var id = RepositoryFactory.Command<ICommentData>().GetAllForAdmin()
				.OrderBy(item => item.Name)
				.ToPage(1, 1).First().Id;

			var dataModel = RepositoryFactory.Command<ICommentCrud>().FindByPk(id);

			dataModel.Name = "changed comment";
			dataModel.Text = "changed text";
			// Act
			ActionResult result = controller.Edit(dataModel);

			// Assert
			result.AssertActionRedirect().ToAction("Index");

			var changedItem = RepositoryFactory.Command<ICommentCrud>().FindByPk(id);
			Assert.IsNotNull(changedItem);
			Assert.AreEqual(dataModel.Id, changedItem.Id);
			Assert.AreEqual("changed comment", changedItem.Name);
			Assert.AreEqual("changed text", changedItem.Text);
		}

	}
}
