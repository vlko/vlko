using System;
using System.Diagnostics;
using System.IO;
using System.Web.Mvc;
using Castle.ActiveRecord.Testing;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using vlko.core;
using vlko.core.Action;
using vlko.core.Authentication;
using vlko.core.Components;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.model.Action;
using vlko.model.Action.CRUDModel;
using vlko.model.Action.ViewModel;
using vlko.model.Search;
using vlko.web.Areas.Admin.Controllers;

namespace vlko.web.Tests.Controllers.Admin
{
	[TestClass]
	public class StaticPageControllerTest : BaseControllerTest
	{
		public static int NumberOfGeneratedItems = 100;

		/// <summary>
		/// Fills the db with data.
		/// </summary>
		protected override void FillDbWithData()
		{
			using (var tran = RepositoryFactory.StartTransaction())
			{

				RepositoryFactory.Action<IUserAction>().CreateAdmin("vlko", "vlko@zilina.net", "test");
				var admin = RepositoryFactory.Action<IUserAction>().GetByName("vlko");
				for (int i = 0; i < NumberOfGeneratedItems; i++)
				{
					RepositoryFactory.Action<IStaticTextCrud>().Create(
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
				}
				tran.Commit();
			}
		}

		[TestMethod]
		public void Index()
		{
			// Arrange
			StaticPageController controller = new StaticPageController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);

			// Act
			ActionResult result = controller.Index(new PagedModel<StaticTextViewModel>());

			// Assert
			var model = result.AssertViewRendered().WithViewData<PagedModel<StaticTextViewModel>>();

			Assert.AreEqual(NumberOfGeneratedItems, model.Count);

			int i = 0;
			foreach (var staticText in model)
			{
				Assert.AreEqual("StaticPage" + i, staticText.Title);
				Assert.AreEqual("staticpage" + i, staticText.FriendlyUrl);
				++i;
			}
			Assert.AreEqual(PagedModel<StaticTextViewModel>.DefaultPageItems, i);
		}

		[TestMethod]
		public void Details()
		{
			// Arrange
			StaticPageController controller = new StaticPageController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);

			var id = RepositoryFactory.Action<IStaticTextData>().Get("staticpage0").Id;

			// Act
			ActionResult result = controller.Details(id);

			// Assert
			var model = result.AssertViewRendered().WithViewData<StaticTextCRUDModel>();

			Assert.AreEqual(id, model.Id);
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			StaticPageController controller = new StaticPageController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);

			var id = RepositoryFactory.Action<IStaticTextData>().Get("staticpage0").Id;

			// Act
			ActionResult result = controller.Delete(id);

			// Assert
			var model = result.AssertViewRendered().WithViewData<StaticTextCRUDModel>();

			Assert.AreEqual(id, model.Id);
		}

		[TestMethod]
		public void Delete_post_success()
		{
			// Arrange
			StaticPageController controller = new StaticPageController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);
			controller.MockUser("vlko");

			var id = RepositoryFactory.Action<IStaticTextData>().Get("staticpage0").Id;
			var dataModel = RepositoryFactory.Action<IStaticTextCrud>().FindByPk(id);

			// Act
			ActionResult result = controller.Delete(dataModel);

			// Assert
			result.AssertActionRedirect().ToAction("Index");

			var deletedItems = RepositoryFactory.Action<IStaticTextData>().GetDeleted();
			Assert.AreEqual(1, deletedItems.Count());
			Assert.AreEqual(id, deletedItems.ToArray()[0].Id);
		}

		[TestMethod]
		public void Delete_post_fail_for_not_owner()
		{
			// Arrange
			StaticPageController controller = new StaticPageController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);
			controller.MockUser("test");

			var id = RepositoryFactory.Action<IStaticTextData>().Get("staticpage0").Id;
			var dataModel = RepositoryFactory.Action<IStaticTextCrud>().FindByPk(id);

			// Act
			ActionResult result = controller.Delete(dataModel);

			// Assert
			var model = result.AssertViewRendered().WithViewData<StaticTextCRUDModel>();
			Assert.AreEqual(id, model.Id);
			Assert.IsFalse(controller.ModelState.IsValid);

			var deletedItems = RepositoryFactory.Action<IStaticTextData>().GetDeleted();
			Assert.AreEqual(0, deletedItems.Count());
		}

		[TestMethod]
		public void Create()
		{
			// Arrange
			StaticPageController controller = new StaticPageController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);

			// Act
			ActionResult result = controller.Create();

			// Assert
			var model = result.AssertViewRendered().WithViewData<StaticTextCRUDModel>();

			Assert.AreEqual(DateTime.Now.Date, model.PublishDate.Date);
		}

		[TestMethod]
		public void Create_post_success()
		{
			// Arrange
			StaticPageController controller = new StaticPageController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);
			controller.MockUser("vlko");

			var dataModel = new StaticTextCRUDModel
							{
								AllowComments = false,
								Title = "CreateTest",
								FriendlyUrl = "StaticPage",
								PublishDate = DateTime.Now,
								Text = "<p>Create test</p>"
							};
			// Act
			ActionResult result = controller.Create(dataModel);

			// Assert
			result.AssertActionRedirect().ToAction("Index");

			var newItemByUniqueFriendlyUrl = RepositoryFactory.Action<IStaticTextData>().Get("staticpage99");
			Assert.IsNotNull(newItemByUniqueFriendlyUrl);
			Assert.AreEqual("vlko", newItemByUniqueFriendlyUrl.Creator.Name);
			Assert.AreEqual(DateTime.Now.Date, newItemByUniqueFriendlyUrl.ChangeDate.Date);
			Assert.AreEqual("Create test", newItemByUniqueFriendlyUrl.Description);
		}


		[TestMethod]
		public void Create_post_failed()
		{
			// Arrange
			StaticPageController controller = new StaticPageController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);

			var emptyDataModel = controller.BindModel<StaticTextCRUDModel>(new FormCollection());

			// Act
			ActionResult result = controller.Create(emptyDataModel);

			// Assert
			var model = result.AssertViewRendered().WithViewData<StaticTextCRUDModel>();
		}

		[TestMethod]
		public void Edit()
		{
			// Arrange
			StaticPageController controller = new StaticPageController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);

			var id = RepositoryFactory.Action<IStaticTextData>().Get("staticpage0").Id;

			// Act
			ActionResult result = controller.Edit(id);

			// Assert
			var model = result.AssertViewRendered().WithViewData<StaticTextCRUDModel>();

			Assert.AreEqual(id, model.Id);
		}

		[TestMethod]
		public void Edit_post_success()
		{
			// Arrange
			StaticPageController controller = new StaticPageController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);
			controller.MockUser("vlko");

			var id = RepositoryFactory.Action<IStaticTextData>().Get("staticpage0").Id;
			var dataModel = RepositoryFactory.Action<IStaticTextCrud>().FindByPk(id);
			dataModel.FriendlyUrl = "changed-friendly-url";
			dataModel.Title = "changed title";
			dataModel.Text = "<p>changed text</p>";

			// Act
			ActionResult result = controller.Edit(dataModel);

			// Assert
			result.AssertActionRedirect().ToAction("Index");

			var changedItemByUniqueFriendlyUrl = RepositoryFactory.Action<IStaticTextData>().Get("changed-friendly-url");
			Assert.IsNotNull(changedItemByUniqueFriendlyUrl);
			Assert.AreEqual(dataModel.Id, changedItemByUniqueFriendlyUrl.Id);
			Assert.AreEqual("changed title", changedItemByUniqueFriendlyUrl.Title);
			Assert.AreEqual("changed text", changedItemByUniqueFriendlyUrl.Description);
			Assert.AreEqual(DateTime.Now.Date, changedItemByUniqueFriendlyUrl.ChangeDate.Date);
		}

		[TestMethod]
		public void Edit_post_failed_not_owner()
		{
			// Arrange
			StaticPageController controller = new StaticPageController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);
			controller.MockUser("other");

			var id = RepositoryFactory.Action<IStaticTextData>().Get("staticpage0").Id;
			var dataModel = RepositoryFactory.Action<IStaticTextCrud>().FindByPk(id);
			dataModel.FriendlyUrl = "changed-friendly-url";

			// Act
			ActionResult result = controller.Edit(dataModel);

			// Assert
			var model = result.AssertViewRendered().WithViewData<StaticTextCRUDModel>();

			Assert.IsFalse(controller.ModelState.IsValid);
			Assert.AreEqual(id, model.Id);
		}

		[TestMethod]
		public void Edit_post_failed_not_valid_model()
		{
			// Arrange
			StaticPageController controller = new StaticPageController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);

			var emptyDataModel = controller.BindModel<StaticTextCRUDModel>(new FormCollection());
			// Act
			ActionResult result = controller.Edit(emptyDataModel);

			// Assert
			// Assert
			var model = result.AssertViewRendered().WithViewData<StaticTextCRUDModel>();

			Assert.IsFalse(controller.ModelState.IsValid);
			Assert.AreEqual(Guid.Empty, model.Id);
		}


		

		public class UserAuthenticationServiceMock : IUserAuthenticationService
		{
			#region Mock implementation
			public int MinPasswordLength
			{
				get { throw new NotImplementedException(); }
			}

			public ValidateUserStatus ValidateUser(string userName, string password)
			{
				throw new NotImplementedException();
			}

			public CreateUserStatus CreateUser(string userName, string password, string email)
			{
				throw new NotImplementedException();
			}

			public bool ChangePassword(string userName, string oldPassword, string newPassword)
			{
				throw new NotImplementedException();
			}

			public string ResolveToken(string token)
			{
				throw new NotImplementedException();
			}

			public bool ConfirmRegistration(string token)
			{
				throw new NotImplementedException();
			}

			public ResetUserPasswordStatus GetResetPasswordToken(string email)
			{
				throw new NotImplementedException();
			}

			public bool ConfirmResetPassword(string username, string token, string newPassword)
			{
				throw new NotImplementedException();
			}

			public bool IsUserInRole(string username, string role)
			{
				return false;
			}
			#endregion
		}
	}
}