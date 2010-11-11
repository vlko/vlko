using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.core.Services;
using vlko.model.Action;
using vlko.model.Action.ViewModel;
using vlko.model.Implementation.NH.Action;
using vlko.web.Areas.Admin.Controllers;
using vlko.web.Areas.Admin.ViewModel.FileBrowser;

namespace vlko.web.Tests.Controllers.Admin
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
				Component.For<IFileBrowserAction>().ImplementedBy<FileBrowserAction>()
					.DynamicParameters((kernel, parameters) =>
					{
						var appInfo = IoC.Resolve<IAppInfoService>();
						parameters["rootUrl"] = appInfo.RootUrl;
						parameters["rootPath"] = appInfo.RootPath;
					}),
				Component.For<IAppInfoService>().ImplementedBy<AppInfoServiceMock>()
				);
		}

		[TestMethod]
		public void Index_empty()
		{
			// Arrange
			var controller = new FileBrowserController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);
			controller.MockUser("empty");

			// Act
			ActionResult result = controller.Index();

			// Assert
			var model = result.AssertViewRendered().WithViewData<FileBrowserViewModel>();

			Assert.IsNotNull(model.UserFiles);
			Assert.AreEqual(0, model.UserFiles.Count());
		}

		[TestMethod]
		public void Upload_ok()
		{
			// Arrange
			var controller = new FileBrowserController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);
			builder.Form["Ident"] = "some_name";
			controller.MockUser("upload_ok");

			// Act

			ActionResult result = controller.Upload(new HttpPostedFileMock("test.jpg", 40));

			// Assert
			var model = result.AssertViewRendered().WithViewData<FileBrowserViewModel>();

			Assert.IsTrue(controller.ModelState.IsValid);

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

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);
			builder.Form["Ident"] = "..\\some_name";
			controller.MockUser("..\\upload_ok");

			// Act
			ActionResult result = controller.Upload(new HttpPostedFileMock("..\\test.\\jpg", 40));

			// Assert
			var model = result.AssertViewRendered().WithViewData<FileBrowserViewModel>();

			Assert.IsTrue(controller.ModelState.IsValid);

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

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);
			builder.Form["Ident"] = "some_name";
			controller.MockUser("Upload_file_too_big");

			// Act
			ActionResult result = controller.Upload(new HttpPostedFileMock("test.jpg", FileBrowserViewModel.MaxFileSize + 1));

			// Assert
			var model = result.AssertViewRendered().WithViewData<FileBrowserViewModel>();

			Assert.IsFalse(controller.ModelState.IsValid);

			Assert.IsNotNull(model.UserFiles);
			Assert.AreEqual(0, model.UserFiles.Count());
		}

		[TestMethod]
		public void Upload_file_empty_ident()
		{
			// Arrange
			var controller = new FileBrowserController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);
			builder.Form["Ident"] = string.Empty;
			controller.MockUser("Upload_file_empty_ident");

			// Act
			ActionResult result = controller.Upload(new HttpPostedFileMock("test.jpg", 40));

			// Assert
			var model = result.AssertViewRendered().WithViewData<FileBrowserViewModel>();

			Assert.IsFalse(controller.ModelState.IsValid);

			Assert.IsNotNull(model.UserFiles);
			Assert.AreEqual(0, model.UserFiles.Count());
		}

		[TestMethod]
		public void Upload_ident_exists()
		{
			// Arrange
			var controller = new FileBrowserController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);
			builder.Form["Ident"] = "some_name";
			controller.MockUser("Upload_ident_exists");

			// Act
			ActionResult previousResult = controller.Upload(new HttpPostedFileMock("test.jpg", 40));
			// upload second time the same
			ActionResult result = controller.Upload(new HttpPostedFileMock("test.jpg", 40));

			// Assert
			var model = result.AssertViewRendered().WithViewData<FileBrowserViewModel>();

			Assert.IsFalse(controller.ModelState.IsValid);

			Assert.IsNotNull(model.UserFiles);
			Assert.AreEqual(1, model.UserFiles.Count());
		}

		[TestMethod]
		public void Delete()
		{
			// Arrange
			var controller = new FileBrowserController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);
			builder.Form["Ident"] = "some_name";
			controller.MockUser("delete");

			// Act
			ActionResult createResult = controller.Upload(new HttpPostedFileMock("test.jpg", 40));
			// get first
			var createModel = createResult.AssertViewRendered().WithViewData<FileBrowserViewModel>();
			// delete first
			ActionResult result = controller.Delete(createModel.UserFiles.First().Ident);

			// Assert
			var deleteModel = result.AssertViewRendered().WithViewData<FileViewModel>();

			Assert.IsNotNull(deleteModel);
			Assert.AreEqual("some_name.jpg", deleteModel.Ident);
			Assert.AreEqual(40, deleteModel.Size);
		}

		[TestMethod]
		public void Delete_post_ok()
		{
			// Arrange
			var controller = new FileBrowserController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);
			builder.Form["Ident"] = "some_name";
			controller.MockUser("delete_ok");

			// Act
			ActionResult createResult = controller.Upload(new HttpPostedFileMock("test.jpg", 40));
			// get first
			var createModel = createResult.AssertViewRendered().WithViewData<FileBrowserViewModel>();
			// delete first
			ActionResult result = controller.Delete(createModel.UserFiles.First());

			// Assert
			result.AssertActionRedirect().ToAction("Index");

			// test if user directory is empty
			ActionResult listResult = controller.Index();
			var model = listResult.AssertViewRendered().WithViewData<FileBrowserViewModel>();
			Assert.AreEqual(0, model.UserFiles.Count());
		}

		[TestMethod]
		public void Delete_post_failed()
		{
			// Arrange
			var controller = new FileBrowserController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);
			builder.Form["Ident"] = "some_name";
			controller.MockUser("delete_ok");

			// Act
			// Act
			ActionResult createResult = controller.Upload(new HttpPostedFileMock("test.jpg", 40));
			// get first
			var createModel = createResult.AssertViewRendered().WithViewData<FileBrowserViewModel>();
			var item = createModel.UserFiles.First();
			item.Ident += "_not_exists";
			// delete first
			ActionResult result = controller.Delete(item);

			// Assert
			result.AssertViewRendered();

			Assert.IsFalse(controller.ModelState.IsValid);
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