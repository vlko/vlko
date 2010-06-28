using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Castle.Windsor;
using GenericRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Castle.ActiveRecord.Testing;
using vlko.core;
using vlko.core.Components;
using vlko.core.IoC;
using vlko.core.Models.Action;
using vlko.core.Models.Action.ActionModel;
using vlko.core.Models.Action.ViewModel;
using vlko.web.Areas.Admin.Controllers;

namespace vlko.web.Tests.Controllers.Admin
{
	[TestClass]
	public class CommentControllerTest : InMemoryTest
	{
		public static int NumberOfGeneratedItems = 100;
		private IUnitOfWork session;
		[TestInitialize]
		public void Init()
		{
			IoC.InitializeWith(new WindsorContainer());
			ApplicationInit.InitializeRepositories();
			base.SetUp();
			using (var tran = RepositoryFactory.StartTransaction())
			{

				IoC.Resolve<IUserAction>().CreateAdmin("vlko", "vlko@zilina.net", "test");
				var admin = IoC.Resolve<IUserAction>().GetByName("vlko");
				for (int i = 0; i < NumberOfGeneratedItems; i++)
				{
					var text = IoC.Resolve<IStaticTextCrud>().Create(
						new StaticTextActionModel
						{
							AllowComments = false,
							Creator = admin,
							Title = "StaticPage" + i,
							FriendlyUrl = "staticpage" + (i == NumberOfGeneratedItems - 1 ? string.Empty : i.ToString()),
							ChangeDate = DateTime.Now,
							PublishDate = DateTime.Now.AddDays(-i),
							Text = "Static page" + i
						});
					IoC.Resolve<ICommentCrud>().Create(
						new CommentActionModel()
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
			session = RepositoryFactory.StartUnitOfWork();
		}

		[TestCleanup]
		public void Cleanup()
		{
			session.Dispose();
			TearDown();
		}

		public override Type[] GetTypes()
		{
			return ApplicationInit.ListOfModelTypes();
		}

		[TestMethod]
		public void Index()
		{
			// Arrange
			CommentController controller = new CommentController();
			controller.MockRequest();
			// Act
			ViewResult result = controller.Index(new PagedModel<CommentForAdminViewModel>()) as ViewResult;

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));

			ViewResult viewResult = (ViewResult)result;
			var model = (PagedModel<CommentForAdminViewModel>)viewResult.ViewData.Model;

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
			controller.MockRequest();
			var id = IoC.Resolve<ICommentData>().GetAllForAdmin()
				.OrderBy(item => item.Name)
				.ToPage(1, 1).First().Id;

			// Act
			ActionResult result = controller.Details(id);

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));

			ViewResult viewResult = (ViewResult)result;
			var model = (CommentActionModel)viewResult.ViewData.Model;

			Assert.AreEqual(id, model.Id);
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			CommentController controller = new CommentController();
			controller.MockRequest();
			var id = IoC.Resolve<ICommentData>().GetAllForAdmin()
				.OrderBy(item => item.Name)
				.ToPage(1, 1).First().Id;

			// Act
			ActionResult result = controller.Delete(id);

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));

			ViewResult viewResult = (ViewResult)result;
			var model = (CommentActionModel)viewResult.ViewData.Model;

			Assert.AreEqual(id, model.Id);
		}

		[TestMethod]
		public void Delete_post_success()
		{
			// Arrange
			CommentController controller = new CommentController();
			controller.MockRequest();
			controller.MockValueProvider("Comment");

			var data = IoC.Resolve<ICommentData>().GetAllForAdmin()
				.OrderBy(item => item.Name);
			var count = data.Count();
				
			var dataModel = IoC.Resolve<ICommentCrud>().FindByPk(data.ToPage(1, 1).First().Id);

			// Act
			ActionResult result = controller.Delete(dataModel);

			// Assert
			Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
			RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
			Assert.AreEqual("Index", redirectResult.RouteValues["action"]);

			Assert.AreEqual(count - 1, data.Count());
		}

		[TestMethod]
		public void Edit()
		{
			// Arrange
			CommentController controller = new CommentController();
			controller.MockRequest();
			var id = IoC.Resolve<ICommentData>().GetAllForAdmin()
				.OrderBy(item => item.Name)
				.ToPage(1, 1).First().Id;

			// Act
			ActionResult result = controller.Edit(id);

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));

			ViewResult viewResult = (ViewResult)result;
			var model = (CommentActionModel)viewResult.ViewData.Model;

			Assert.AreEqual(id, model.Id);
		}

		[TestMethod]
		public void Edit_post_success()
		{
			// Arrange
			CommentController controller = new CommentController();
			controller.MockRequest();
			controller.MockValueProvider("Comment");

			var id = IoC.Resolve<ICommentData>().GetAllForAdmin()
				.OrderBy(item => item.Name)
				.ToPage(1, 1).First().Id;

			var dataModel = IoC.Resolve<ICommentCrud>().FindByPk(id);

			dataModel.Name = "changed comment";
			dataModel.Text = "changed text";
			// Act
			ActionResult result = controller.Edit(dataModel);

			// Assert
			Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
			RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
			Assert.AreEqual("Index", redirectResult.RouteValues["action"]);

			var changedItem = IoC.Resolve<ICommentCrud>().FindByPk(id);
			Assert.IsNotNull(changedItem);
			Assert.AreEqual(dataModel.Id, changedItem.Id);
			Assert.AreEqual("changed comment", changedItem.Name);
			Assert.AreEqual("changed text", changedItem.Text);
		}

	}
}
