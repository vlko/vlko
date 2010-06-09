using System;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Castle.ActiveRecord.Testing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using GenericRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core;
using vlko.core.Authentication;
using vlko.core.Authentication.Implementation;
using vlko.core.Base;
using vlko.core.Components;
using vlko.core.Models.Action;
using vlko.core.Models.Action.ActionModel;
using vlko.core.Models.Action.ViewModel;
using vlko.core.Services;
using vlko.model.IoC;
using vlko.web.Controllers;
using vlko.web.ViewModel.FileBrowser;

namespace vlko.web.Tests.Controllers
{
    [TestClass]
    public class FileBrowserControllerTest
    {
        public static int NumberOfGeneratedItems = 100;
        private IUnitOfWork session;
        [TestInitialize]
        public void Init()
        {
            IoC.InitializeWith(new WindsorContainer());
            IoC.Container.Register(
                Component.For<IFileBrowserAction>().ImplementedBy<FileBrowserAction>(),
                Component.For<IAppInfoService>().ImplementedBy<AppInfoServiceMock>()
                );
        }

        [TestMethod]
        public void Index_empty()
        {
            // Arrange
            var controller = new FileBrowserController();
            controller.MockRequest();
            MockUser(controller, "empty");
            // Act
            ActionResult result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = (ViewResult)result;
            var model = (FileBrowserViewModel)viewResult.ViewData.Model;

            Assert.IsNotNull(model.UserFiles);
            Assert.AreEqual(0, model.UserFiles.Count());
        }

        [TestMethod]
        public void Upload_ok()
        {
            // Arrange
            var controller = new FileBrowserController();
            var form = new FormCollection();
            form.Add("Ident", "some_name");
            controller.MockRequest(form);
            MockUser(controller, "upload_ok");
            // Act

            ActionResult result = controller.Upload(new HttpPostedFileMock("test.jpg", 40));

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            Assert.IsTrue(controller.ModelState.IsValid);

            var viewResult = (ViewResult)result;
            var model = (FileBrowserViewModel)viewResult.ViewData.Model;

            Assert.IsNotNull(model.UserFiles);
            Assert.AreEqual(1, model.UserFiles.Count());

            FileViewModel savedFile = model.UserFiles.First();
            Assert.AreEqual("some_name.jpg", savedFile.Ident);
            Assert.AreEqual(40, savedFile.Size);
        }

        [TestMethod]
        public void Upload_with_dangerous_inputs()
        {
            // Arrange
            var controller = new FileBrowserController();
            var form = new FormCollection();
            form.Add("Ident", "..\\some_name");
            controller.MockRequest(form);
            MockUser(controller, "..\\upload_ok");
            // Act

            ActionResult result = controller.Upload(new HttpPostedFileMock("..\\test.\\jpg", 40));

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            Assert.IsTrue(controller.ModelState.IsValid);

            var viewResult = (ViewResult)result;
            var model = (FileBrowserViewModel)viewResult.ViewData.Model;

            Assert.IsNotNull(model.UserFiles);
            Assert.AreEqual(1, model.UserFiles.Count());

            FileViewModel savedFile = model.UserFiles.First();
            Assert.AreEqual("..-some_name", savedFile.Ident);
            Assert.AreEqual(40, savedFile.Size);          
        }

        [TestMethod]
        public void Upload_file_too_big()
        {
            // Arrange
            var controller = new FileBrowserController();
            var form = new FormCollection();
            form.Add("Ident", "some_name");
            controller.MockRequest(form);
            MockUser(controller, "Upload_file_too_big");
            // Act

            ActionResult result = controller.Upload(new HttpPostedFileMock("test.jpg", FileBrowserViewModel.MaxFileSize + 1));

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            Assert.IsFalse(controller.ModelState.IsValid);

            var viewResult = (ViewResult)result;
            var model = (FileBrowserViewModel)viewResult.ViewData.Model;

            Assert.IsNotNull(model.UserFiles);
            Assert.AreEqual(0, model.UserFiles.Count());
        }

        [TestMethod]
        public void Upload_file_empty_ident()
        {
            // Arrange
            var controller = new FileBrowserController();
            var form = new FormCollection();
            form.Add("Ident", string.Empty);
            controller.MockRequest(form);
            MockUser(controller, "Upload_file_empty_ident");
            // Act

            ActionResult result = controller.Upload(new HttpPostedFileMock("test.jpg", 40));

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            Assert.IsFalse(controller.ModelState.IsValid);

            var viewResult = (ViewResult)result;
            var model = (FileBrowserViewModel)viewResult.ViewData.Model;

            Assert.IsNotNull(model.UserFiles);
            Assert.AreEqual(0, model.UserFiles.Count());
        }

        [TestMethod]
        public void Upload_ident_exists()
        {
            // Arrange
            var controller = new FileBrowserController();
            var form = new FormCollection();
            form.Add("Ident", "some_name");
            controller.MockRequest(form);
            MockUser(controller, "Upload_ident_exists");
            // Act

            ActionResult result = controller.Upload(new HttpPostedFileMock("test.jpg", 40));
            result = controller.Upload(new HttpPostedFileMock("test.jpg", 40));

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            Assert.IsFalse(controller.ModelState.IsValid);

            var viewResult = (ViewResult)result;
            var model = (FileBrowserViewModel)viewResult.ViewData.Model;

            Assert.IsNotNull(model.UserFiles);
            Assert.AreEqual(1, model.UserFiles.Count());
        }

        [TestMethod]
        public void Delete()
        {
            // Arrange
            var controller = new FileBrowserController();
            var form = new FormCollection();
            form.Add("Ident", "some_name");
            controller.MockRequest(form);
            controller.MockValueProvider("FileBrowser");
            MockUser(controller, "delete");

            // Act
            ActionResult result = controller.Upload(new HttpPostedFileMock("test.jpg", 40));
            // get first
            var viewResult = (ViewResult)result;
            var model = (FileBrowserViewModel)viewResult.ViewData.Model;
            result = controller.Delete(model.UserFiles.First().Ident);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            viewResult = (ViewResult)result;
            var deleteModel = (FileViewModel)viewResult.ViewData.Model;

            Assert.IsNotNull(deleteModel);
            Assert.AreEqual("some_name.jpg", deleteModel.Ident);
            Assert.AreEqual(40, deleteModel.Size);
        }

        [TestMethod]
        public void Delete_post_ok()
        {
            // Arrange
            var controller = new FileBrowserController();
            var form = new FormCollection();
            form.Add("Ident", "some_name");
            controller.MockRequest(form);
            controller.MockValueProvider("FileBrowser");
            MockUser(controller, "delete_ok");

            // Act
            ActionResult result = controller.Upload(new HttpPostedFileMock("test.jpg", 40));
            // get first
            var viewResult = (ViewResult)result;
            var model = (FileBrowserViewModel)viewResult.ViewData.Model;
            result = controller.Delete(model.UserFiles.First());

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
            Assert.AreEqual("Index", redirectResult.RouteValues["action"]);

            // test if user directory is empty
            result = controller.Index();
            viewResult = (ViewResult)result;
            model = (FileBrowserViewModel)viewResult.ViewData.Model;
            Assert.AreEqual(0, model.UserFiles.Count());
        }

        [TestMethod]
        public void Delete_post_failed()
        {
            // Arrange
            var controller = new FileBrowserController();
            var form = new FormCollection();
            form.Add("Ident", "some_name");
            controller.MockRequest(form);
            controller.MockValueProvider("FileBrowser");
            MockUser(controller, "delete_ok");

            // Act
            ActionResult result = controller.Upload(new HttpPostedFileMock("test.jpg", 40));
            // get first
            var viewResult = (ViewResult)result;
            var model = (FileBrowserViewModel)viewResult.ViewData.Model;
            var item = model.UserFiles.First();
            item.Ident += "_not_exists";
            result = controller.Delete(item);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            Assert.IsFalse(controller.ModelState.IsValid);
        }
        private void MockUser(BaseController controller, string userName)
        {
            IWindsorContainer container = model.IoC.IoC.Container;
            container.Register(
                Component.For<IUserAuthenticationService>().ImplementedBy<StaticPageControllerTest.UserAuthenticationServiceMock>()
                );
            controller.UserInfo = new UserInfo(userName);
        }

        public class AppInfoServiceMock : IAppInfoService
        {
            #region Mock implementation

            public string Name
            {
                get { throw new NotImplementedException(); }
            }

            public string RootUrl
            {
                get { return string.Empty; }
            }

            public string RootPath
            {
                get { return System.Environment.CurrentDirectory; }
            }

            public MailTemplate GetRegistrationMailTemplate()
            {
                throw new NotImplementedException();
            }

            public MailTemplate GetResetPasswordMailTemplate()
            {
                throw new NotImplementedException();
            }
            #endregion
        }

        public class HttpPostedFileMock : HttpPostedFileBase
        {
            #region Mock implementation

            private readonly string _fileName;
            private readonly int _lenght;

            public HttpPostedFileMock(string fileName, int lenght)
            {
                _fileName = fileName;
                _lenght = lenght;
            }

            public override int ContentLength
            {
                get
                {
                    return _lenght;
                }
            }

            public override string FileName
            {
                get
                {
                    return _fileName;
                }
            }

            public override System.IO.Stream InputStream
            {
                get
                {
                    byte[] byteArray = Encoding.ASCII.GetBytes( string.Empty.PadLeft(_lenght) );
                    return new System.IO.MemoryStream( byteArray ); 
                }
            }
            #endregion
        }
    }
}