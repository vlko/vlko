using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using vlko.BlogModule.Commands;
using vlko.BlogModule.Commands.CRUDModel;
using vlko.BlogModule.Commands.ViewModel;
using vlko.core.Authentication;
using vlko.core.Commands;
using vlko.core.Components;
using vlko.core.NH.Repository;
using vlko.core.Repository;
using vlko.core.Roots;
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

				RepositoryFactory.Command<IUserCommands>().CreateAdmin("vlko", "vlko@zilina.net", "test");
				var admin = RepositoryFactory.Command<IUserCommands>().GetByName("vlko");
				SessionFactory<User>.Create(new User
				                            	{
													Id = Guid.NewGuid(),
				                            		Name = "test",
													Email = "test"
				                            	});
				SessionFactory<User>.Create(new User
				                            	{
													Id = Guid.NewGuid(),
				                            		Name = "other",
													Email = "other"
				                            	});
				for (int i = 0; i < NumberOfGeneratedItems; i++)
				{
					RepositoryFactory.Command<IStaticTextCrud>().Create(
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

			var id = RepositoryFactory.Command<IStaticTextData>().Get("staticpage0").Id;

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

			var id = RepositoryFactory.Command<IStaticTextData>().Get("staticpage0").Id;

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

			var id = RepositoryFactory.Command<IStaticTextData>().Get("staticpage0").Id;
			var dataModel = RepositoryFactory.Command<IStaticTextCrud>().FindByPk(id);

			// Act
			ActionResult result = controller.Delete(dataModel);

			// Assert
			result.AssertActionRedirect().ToAction("Index");

			var deletedItems = RepositoryFactory.Command<IStaticTextData>().GetDeleted();
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

			var id = RepositoryFactory.Command<IStaticTextData>().Get("staticpage0").Id;
			var dataModel = RepositoryFactory.Command<IStaticTextCrud>().FindByPk(id);

			// Act
			ActionResult result = controller.Delete(dataModel);

			// Assert
			var model = result.AssertViewRendered().WithViewData<StaticTextCRUDModel>();
			Assert.AreEqual(id, model.Id);
			Assert.IsFalse(controller.ModelState.IsValid);

			var deletedItems = RepositoryFactory.Command<IStaticTextData>().GetDeleted();
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

			var newItemByUniqueFriendlyUrl = RepositoryFactory.Command<IStaticTextData>().Get("staticpage99");
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

			var id = RepositoryFactory.Command<IStaticTextData>().Get("staticpage0").Id;

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

			var id = RepositoryFactory.Command<IStaticTextData>().Get("staticpage0").Id;
			var dataModel = RepositoryFactory.Command<IStaticTextCrud>().FindByPk(id);
			dataModel.FriendlyUrl = "changed-friendly-url";
			dataModel.Title = "changed title";
			dataModel.Text = "<p>changed text</p>";

			// Act
			ActionResult result = controller.Edit(dataModel);

			// Assert
			result.AssertActionRedirect().ToAction("Index");

			var changedItemByUniqueFriendlyUrl = RepositoryFactory.Command<IStaticTextData>().Get("changed-friendly-url");
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

			var id = RepositoryFactory.Command<IStaticTextData>().Get("staticpage0").Id;
			var dataModel = RepositoryFactory.Command<IStaticTextCrud>().FindByPk(id);
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
			#endregion
		}
	}
}