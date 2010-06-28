using System;
using System.Web.Mvc;
using Castle.ActiveRecord.Testing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using GenericRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core;
using vlko.core.Authentication;
using vlko.core.Components;
using vlko.core.IoC;
using vlko.core.Models.Action;
using vlko.core.Models.Action.ActionModel;
using vlko.core.Models.Action.ViewModel;
using vlko.web.Areas.Admin.Controllers;

namespace vlko.web.Tests.Controllers.Admin
{
    [TestClass]
    public class StaticPageControllerTest : InMemoryTest
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
                    IoC.Resolve<IStaticTextCrud>().Create(
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
            StaticPageController controller = new StaticPageController();
            controller.MockRequest();
            // Act
            ViewResult result = controller.Index(new PagedModel<StaticTextViewModel>()) as ViewResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;
            var model = (PagedModel<StaticTextViewModel>)viewResult.ViewData.Model;

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
            controller.MockRequest();
            var id = IoC.Resolve<IStaticTextData>().Get("staticpage0").Id;

            // Act
            ActionResult result = controller.Details(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;
            var model = (StaticTextActionModel)viewResult.ViewData.Model;

            Assert.AreEqual(id, model.Id);
        }

        [TestMethod]
        public void Delete()
        {
            // Arrange
            StaticPageController controller = new StaticPageController();
            controller.MockRequest();
            var id = IoC.Resolve<IStaticTextData>().Get("staticpage0").Id;

            // Act
            ActionResult result = controller.Delete(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;
            var model = (StaticTextActionModel)viewResult.ViewData.Model;

            Assert.AreEqual(id, model.Id);
        }

        [TestMethod]
        public void Delete_post_success()
        {
            // Arrange
            StaticPageController controller = new StaticPageController();
            controller.MockRequest();
            controller.MockValueProvider("StaticPage");

            MockUser(controller, "vlko");

            var id = IoC.Resolve<IStaticTextData>().Get("staticpage0").Id;
            var dataModel = IoC.Resolve<IStaticTextCrud>().FindByPk(id);

            // Act
            ActionResult result = controller.Delete(dataModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
            Assert.AreEqual("Index", redirectResult.RouteValues["action"]);

            var deletedItems = IoC.Resolve<IStaticTextData>().GetDeleted();
            Assert.AreEqual(1, deletedItems.Count());
            Assert.AreEqual(id, deletedItems.ToArray()[0].Id);
        }

        [TestMethod]
        public void Delete_post_failed()
        {
            // Arrange
            StaticPageController controller = new StaticPageController();
            controller.MockRequest();
            controller.MockValueProvider("StaticPage");

            MockUser(controller, "vlko");

            var id = IoC.Resolve<IStaticTextData>().Get("staticpage0").Id;
            var dataModel = IoC.Resolve<IStaticTextCrud>().FindByPk(id);

            // Act
            ActionResult result = controller.Delete(dataModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
            Assert.AreEqual("Index", redirectResult.RouteValues["action"]);

            var deletedItems = IoC.Resolve<IStaticTextData>().GetDeleted();
            Assert.AreEqual(1, deletedItems.Count());
            Assert.AreEqual(id, deletedItems.ToArray()[0].Id);
        }

        [TestMethod]
        public void Delete_post_fail_for_not_owner()
        {
            // Arrange
            StaticPageController controller = new StaticPageController();
            controller.MockRequest();
            controller.MockValueProvider("StaticPage");

            MockUser(controller, "other");

            var id = IoC.Resolve<IStaticTextData>().Get("staticpage0").Id;
            var dataModel = IoC.Resolve<IStaticTextCrud>().FindByPk(id);

            // Act
            ActionResult result = controller.Delete(dataModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;
            var model = (StaticTextActionModel)viewResult.ViewData.Model;

            Assert.AreEqual(id, model.Id);
        }

        [TestMethod]
        public void Create()
        {
            // Arrange
            StaticPageController controller = new StaticPageController();
            controller.MockRequest();

            // Act
            ActionResult result = controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;
            var model = (StaticTextActionModel)viewResult.ViewData.Model;

            Assert.AreEqual(DateTime.Now.Date, model.PublishDate.Date);
        }

        [TestMethod]
        public void Create_post_success()
        {
            // Arrange
            StaticPageController controller = new StaticPageController();
            controller.MockRequest();
            controller.MockValueProvider("StaticPage");

            MockUser(controller, "vlko");

            var dataModel = new StaticTextActionModel
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
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
            Assert.AreEqual("Index", redirectResult.RouteValues["action"]);

            var newItemByUniqueFriendlyUrl = IoC.Resolve<IStaticTextData>().Get("staticpage99");
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
            controller.MockRequest();

            var form = new FormCollection();
            var dataModel = controller.BindModel<StaticTextActionModel>(form);
            // Act
            ActionResult result = controller.Create(dataModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(StaticTextActionModel));
        }

        [TestMethod]
        public void Edit()
        {
            // Arrange
            StaticPageController controller = new StaticPageController();
            controller.MockRequest();
            var id = IoC.Resolve<IStaticTextData>().Get("staticpage0").Id;

            // Act
            ActionResult result = controller.Edit(id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;
            var model = (StaticTextActionModel)viewResult.ViewData.Model;

            Assert.AreEqual(id, model.Id);
        }

        [TestMethod]
        public void Edit_post_success()
        {
            // Arrange
            StaticPageController controller = new StaticPageController();
            controller.MockRequest();
            MockUser(controller, "vlko");
            controller.MockValueProvider("StaticPage");

            var id = IoC.Resolve<IStaticTextData>().Get("staticpage0").Id;
            var dataModel = IoC.Resolve<IStaticTextCrud>().FindByPk(id);
            dataModel.FriendlyUrl = "changed-friendly-url";
            dataModel.Title = "changed title";
            dataModel.Text = "<p>changed text</p>";
            // Act
            ActionResult result = controller.Edit(dataModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
            Assert.AreEqual("Index", redirectResult.RouteValues["action"]);

            var changedItemByUniqueFriendlyUrl = IoC.Resolve<IStaticTextData>().Get("changed-friendly-url");
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
            controller.MockRequest();
            MockUser(controller, "other");
            controller.MockValueProvider("StaticPage");

            var id = IoC.Resolve<IStaticTextData>().Get("staticpage0").Id;
            var dataModel = IoC.Resolve<IStaticTextCrud>().FindByPk(id);
            dataModel.FriendlyUrl = "changed-friendly-url";
            // Act
            ActionResult result = controller.Edit(dataModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(StaticTextActionModel));
        }

        [TestMethod]
        public void Edit_post_failed_not_valid_model()
        {
            // Arrange
            StaticPageController controller = new StaticPageController();
            controller.MockRequest();

            var form = new FormCollection();
            var dataModel = controller.BindModel<StaticTextActionModel>(form);
            // Act
            ActionResult result = controller.Edit(dataModel);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(StaticTextActionModel));
        }


        private void MockUser(StaticPageController controller, string userName)
        {
            IWindsorContainer container = IoC.Container;
            container.Register(
                Component.For<IUserAuthenticationService>().ImplementedBy<UserAuthenticationServiceMock>()
                );
            controller.UserInfo = new UserInfo(userName);
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