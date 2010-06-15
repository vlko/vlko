using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Castle.ActiveRecord.Testing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core;
using vlko.core.Authentication;
using vlko.core.Authentication.Implementation;
using vlko.core.Models.Action;
using vlko.core.Services;
using vlko.core.Services.Implementation;
using vlko.web;
using vlko.web.Controllers;
using vlko.web.ViewModel.Account;

namespace vlko.web.Tests.Controllers
{

	[TestClass]
	public class AccountControllerTest : InMemoryTest
	{
		public class LocalEmailSender : IEmailService
		{
			public bool Send(string toRecipient, string subject, string text, bool asHtmlEmail)
			{
				return true;
			}
		}

		[TestInitialize]
		public void Init()
		{
			model.IoC.IoC.InitializeWith(new WindsorContainer());
			ApplicationInit.InitializeRepositories();
			IWindsorContainer container = new WindsorContainer();
			container.Register(
				Component.For<IAppInfoService>().ImplementedBy<AppInfoService>(),
				Component.For<IEmailService>().ImplementedBy<LocalEmailSender>(),
				Component.For<IFormsAuthenticationService>().ImplementedBy<FormsAuthenticationService>(),
				Component.For<IUserAuthenticationService>().ImplementedBy<UserAuthenticationService>()
				);
			model.IoC.IoC.InitializeWith(container);
			base.SetUp();
		}

		[TestCleanup]
		public void Cleanup()
		{
			TearDown();
		}

		public override Type[] GetTypes()
		{
			return ApplicationInit.ListOfModelTypes();
		}

		[TestMethod]
		public void ChangePassword_Get_ReturnsView()
		{
			// Arrange
			AccountController controller = GetAccountController();

			// Act
			ActionResult result = controller.ChangePassword();

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));
			Assert.AreEqual(10, ((ViewResult)result).ViewData["PasswordLength"]);
		}

		[TestMethod]
		public void ChangePassword_Post_ReturnsRedirectOnSuccess()
		{
			// Arrange
			AccountController controller = GetAccountController();
			ChangePasswordModel model = new ChangePasswordModel()
			{
				OldPassword = "goodOldPassword",
				NewPassword = "goodNewPassword",
				ConfirmPassword = "goodNewPassword"
			};

			// Act
			ActionResult result = controller.ChangePassword(model);

			// Assert
			Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
			RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
			Assert.AreEqual("ChangePasswordSuccess", redirectResult.RouteValues["action"]);
		}

		[TestMethod]
		public void ChangePassword_Post_ReturnsViewIfChangePasswordFails()
		{
			// Arrange
			AccountController controller = GetAccountController();
			ChangePasswordModel model = new ChangePasswordModel()
			{
				OldPassword = "goodOldPassword",
				NewPassword = "badNewPassword",
				ConfirmPassword = "badNewPassword"
			};

			// Act
			ActionResult result = controller.ChangePassword(model);

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));
			ViewResult viewResult = (ViewResult)result;
			Assert.AreEqual(model, viewResult.ViewData.Model);
			Assert.AreEqual("The current password is incorrect or the new password is invalid.", controller.ModelState[""].Errors[0].ErrorMessage);
			Assert.AreEqual(10, viewResult.ViewData["PasswordLength"]);
		}

		[TestMethod]
		public void ChangePassword_Post_ReturnsViewIfModelStateIsInvalid()
		{
			// Arrange
			AccountController controller = GetAccountController();
			ChangePasswordModel model = new ChangePasswordModel()
			{
				OldPassword = "goodOldPassword",
				NewPassword = "goodNewPassword",
				ConfirmPassword = "goodNewPassword"
			};
			controller.ModelState.AddModelError("", "Dummy error message.");

			// Act
			ActionResult result = controller.ChangePassword(model);

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));
			ViewResult viewResult = (ViewResult)result;
			Assert.AreEqual(model, viewResult.ViewData.Model);
			Assert.AreEqual(10, viewResult.ViewData["PasswordLength"]);
		}

		[TestMethod]
		public void ChangePasswordSuccess_ReturnsView()
		{
			// Arrange
			AccountController controller = GetAccountController();

			// Act
			ActionResult result = controller.ChangePasswordSuccess();

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));
		}

		[TestMethod]
		public void LogOff_LogsOutAndRedirects()
		{
			// Arrange
			AccountController controller = GetAccountController();

			// Act
			ActionResult result = controller.LogOff();

			// Assert
			Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
			RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
			Assert.AreEqual("Home", redirectResult.RouteValues["controller"]);
			Assert.AreEqual("Index", redirectResult.RouteValues["action"]);
			Assert.IsTrue(((MockFormsAuthenticationService)controller.FormsService).SignOut_WasCalled);
		}

		[TestMethod]
		public void LogOn_Get_ReturnsView()
		{
			// Arrange
			AccountController controller = GetAccountController();
			var queryString = new NameValueCollection();
			controller.MockRequest(queryStringValues: queryString);

			// Act
			ActionResult result = controller.LogOn();

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));
		}

		[TestMethod]
		public void LogOn_Post_ReturnsRedirectOnSuccess_WithoutReturnUrl()
		{
			// Arrange
			AccountController controller = GetAccountController();
			LogOnModel model = new LogOnModel()
			{
				UserName = "someUser",
				Password = "goodPassword",
				RememberMe = false
			};

			// Act
			ActionResult result = controller.LogOn(model);

			// Assert
			Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
			RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
			Assert.AreEqual("Home", redirectResult.RouteValues["controller"]);
			Assert.AreEqual("Index", redirectResult.RouteValues["action"]);
			Assert.IsTrue(((MockFormsAuthenticationService)controller.FormsService).SignIn_WasCalled);
		}

		[TestMethod]
		public void LogOn_Post_ReturnsRedirectOnSuccess_WithReturnUrl()
		{
			// Arrange
			AccountController controller = GetAccountController();
			LogOnModel model = new LogOnModel()
			{
				UserName = "someUser",
				Password = "goodPassword",
				ReturnUrl = "/someUrl",
				RememberMe = false
			};

			// Act
			ActionResult result = controller.LogOn(model);

			// Assert
			Assert.IsInstanceOfType(result, typeof(RedirectResult));
			RedirectResult redirectResult = (RedirectResult)result;
			Assert.AreEqual("/someUrl", redirectResult.Url);
			Assert.IsTrue(((MockFormsAuthenticationService)controller.FormsService).SignIn_WasCalled);
		}

		[TestMethod]
		public void LogOn_Post_ReturnsViewIfModelStateIsInvalid()
		{
			// Arrange
			AccountController controller = GetAccountController();
			LogOnModel model = new LogOnModel()
			{
				UserName = "someUser",
				Password = "goodPassword",
				RememberMe = false
			};
			controller.ModelState.AddModelError("", "Dummy error message.");

			// Act
			ActionResult result = controller.LogOn(model);

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));
			ViewResult viewResult = (ViewResult)result;
			Assert.AreEqual(model, viewResult.ViewData.Model);
		}

		[TestMethod]
		public void LogOn_Post_ReturnsViewIfValidateUserFails()
		{
			// Arrange
			AccountController controller = GetAccountController();
			LogOnModel model = new LogOnModel()
			{
				UserName = "someUser",
				Password = "badPassword",
				RememberMe = false
			};

			// Act
			ActionResult result = controller.LogOn(model);

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));
			ViewResult viewResult = (ViewResult)result;
			Assert.AreEqual(model, viewResult.ViewData.Model);
			Assert.AreNotEqual(string.Empty, controller.ModelState[""].Errors[0].ErrorMessage);
		}

		[TestMethod]
		public void Register_Get_ReturnsView()
		{
			// Arrange
			AccountController controller = GetAccountController();

			// Act
			ActionResult result = controller.Register();

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));
			Assert.AreEqual(10, ((ViewResult)result).ViewData["PasswordLength"]);
		}

		[TestMethod]
		public void Register_Post_ReturnsRedirectOnSuccess()
		{
			// Arrange
			AccountController controller = GetAccountController();
			RegisterModel model = new RegisterModel()
			{
				UserName = "someUser",
				Email = "goodEmail",
				Password = "goodPassword",
				ConfirmPassword = "goodPassword"
			};

			// Act
			ActionResult result = controller.Register(model);

			// Assert
			Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
			RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
			Assert.AreEqual("Home", redirectResult.RouteValues["controller"]);
			Assert.AreEqual("Index", redirectResult.RouteValues["action"]);
		}

		[TestMethod]
		public void Register_Post_ReturnsViewIfRegistrationFails()
		{
			// Arrange
			AccountController controller = GetAccountController();
			RegisterModel model = new RegisterModel()
			{
				UserName = "duplicateUser",
				Email = "goodEmail",
				Password = "goodPassword",
				ConfirmPassword = "goodPassword"
			};

			// Act
			ActionResult result = controller.Register(model);

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));
			ViewResult viewResult = (ViewResult)result;
			Assert.AreEqual(model, viewResult.ViewData.Model);
			Assert.AreEqual("Username already exists. Please enter a different user name.", controller.ModelState[""].Errors[0].ErrorMessage);
			Assert.AreEqual(10, viewResult.ViewData["PasswordLength"]);
		}

		[TestMethod]
		public void Register_Post_ReturnsViewIfModelStateIsInvalid()
		{
			// Arrange
			AccountController controller = GetAccountController();
			RegisterModel model = new RegisterModel()
			{
				UserName = "someUser",
				Email = "goodEmail",
				Password = "goodPassword",
				ConfirmPassword = "goodPassword"
			};
			controller.ModelState.AddModelError("", "Dummy error message.");

			// Act
			ActionResult result = controller.Register(model);

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));
			ViewResult viewResult = (ViewResult)result;
			Assert.AreEqual(model, viewResult.ViewData.Model);
			Assert.AreEqual(10, viewResult.ViewData["PasswordLength"]);
		}

		[TestMethod]
		public void Register_confirm_registration_valid()
		{
			// Arrange
			AccountController controller = GetAccountController();

			// Act
			ActionResult result = controller.ConfirmRegistration("validToken");

			// Assert
			Assert.IsInstanceOfType(result, typeof (ViewResult));
			ViewResult viewResult = (ViewResult) result;
			Assert.AreEqual("someUser", viewResult.ViewData["user"]);
		}

		[TestMethod]
		public void Register_confirm_registration_failed()
		{
			// Arrange
			AccountController controller = GetAccountController();

			// Act
			ActionResult result = controller.ConfirmRegistration("not_valid_token");

			// Assert
			ViewResult viewResult = (ViewResult)result;
			Assert.AreEqual("NotValidToken", viewResult.ViewName);
		}

		[TestMethod]
		public void Reset_Password_Post_ReturnsRedirectOnSuccess()
		{
			// Arrange
			AccountController controller = GetAccountController();
			ResetPasswordModel model = new ResetPasswordModel()
			{
				Email = "goodEmail",
			};

			// Act
			ActionResult result = controller.ResetPassword(model);

			// Assert
			Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
			RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
			Assert.AreEqual("Home", redirectResult.RouteValues["controller"]);
			Assert.AreEqual("Index", redirectResult.RouteValues["action"]);
		}

		[TestMethod]
		public void Reset_Password_Post_ReturnsViewIfFails()
		{
			// Arrange
			AccountController controller = GetAccountController();
			ResetPasswordModel model = new ResetPasswordModel()
			{
				Email = "wrongEmail",
			};

			// Act
			ActionResult result = controller.ResetPassword(model);

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));
			ViewResult viewResult = (ViewResult)result;
			Assert.AreEqual(model, viewResult.ViewData.Model);
			Assert.AreEqual("Invalid email address. Please enter a different e-mail address.", controller.ModelState[""].Errors[0].ErrorMessage);
		}

		[TestMethod]
		public void Reset_Password_Post_ReturnsViewIfModelStateIsInvalid()
		{
			// Arrange
			AccountController controller = GetAccountController();
			ResetPasswordModel model = new ResetPasswordModel()
			{
				Email = "goodEmail",
			};
			controller.ModelState.AddModelError("", "Dummy error message.");

			// Act
			ActionResult result = controller.ResetPassword(model);

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));
			ViewResult viewResult = (ViewResult)result;
			Assert.AreEqual(model, viewResult.ViewData.Model);
		}

		[TestMethod]
		public void Confirm_Reset_Password_ReturnsView()
		{
			// Arrange
			AccountController controller = GetAccountController();

			// Act
			ActionResult result = controller.ConfirmResetPassword("validToken");

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));
			ConfirmResetPasswordModel viewModel = (ConfirmResetPasswordModel)((ViewResult)result).ViewData.Model;

			Assert.AreEqual("someUser", viewModel.Username);
			Assert.AreEqual("validToken", viewModel.VerifyToken);
		}

		[TestMethod]
		public void Confirm_Reset_Password_Post_ReturnsRedirectOnSuccess()
		{
			// Arrange
			AccountController controller = GetAccountController();
			ConfirmResetPasswordModel model = new ConfirmResetPasswordModel()
												  {
													  Username = "someUser",
													  VerifyToken = "validToken",
													  NewPassword = "goodNewPassword"
												  };

			// Act
			ActionResult result = controller.ConfirmResetPassword(model);

			// Assert
			Assert.IsInstanceOfType(result, typeof (RedirectToRouteResult));
			RedirectToRouteResult redirectResult = (RedirectToRouteResult) result;
			Assert.AreEqual("Home", redirectResult.RouteValues["controller"]);
			Assert.AreEqual("Index", redirectResult.RouteValues["action"]);
		}

		[TestMethod]
		public void Confirm_Reset_Password_Post_ReturnsViewIfFails()
		{
			// Arrange
			AccountController controller = GetAccountController();
			ConfirmResetPasswordModel model = new ConfirmResetPasswordModel()
			{
				Username = "invalidUser",
				VerifyToken = "validToken",
				NewPassword = "goodNewPassword"
			};

			// Act
			ActionResult result = controller.ConfirmResetPassword(model);

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));
			ViewResult viewResult = (ViewResult)result;
			Assert.AreEqual(model, viewResult.ViewData.Model);
			Assert.AreEqual("Unable to reset password. Please contact administrator.", controller.ModelState[""].Errors[0].ErrorMessage);
		}

		[TestMethod]
		public void Confirm_Reset_Password_Post_ReturnsViewIfModelStateIsInvalid()
		{
			// Arrange
			AccountController controller = GetAccountController();
			ConfirmResetPasswordModel model = new ConfirmResetPasswordModel()
			{
				Username = "invalidUser",
				VerifyToken = "validToken",
				NewPassword = "goodNewPassword"
			};
			controller.ModelState.AddModelError("", "Dummy error message.");

			// Act
			ActionResult result = controller.ConfirmResetPassword(model);

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));
			ViewResult viewResult = (ViewResult)result;
			Assert.AreEqual(model, viewResult.ViewData.Model);
		}

		private static AccountController GetAccountController()
		{
			AccountController controller = new AccountController()
			{
				FormsService = new MockFormsAuthenticationService(),
				UserAuthenticationService = new MockUserAuthenticationService()
			};
			controller.ControllerContext = new ControllerContext()
			{
				Controller = controller,
				RequestContext = new RequestContext(new MockHttpContext(), new RouteData())
			};
			return controller;
		}

		private class MockFormsAuthenticationService : IFormsAuthenticationService
		{
			public bool SignIn_WasCalled;
			public bool SignOut_WasCalled;

			public void SignIn(string userName, bool createPersistentCookie)
			{
				// verify that the arguments are what we expected
				Assert.AreEqual("someUser", userName);
				Assert.IsFalse(createPersistentCookie);

				SignIn_WasCalled = true;
			}

			public void SignOut()
			{
				SignOut_WasCalled = true;
			}
		}

		private class MockHttpContext : HttpContextBase
		{
			private readonly IPrincipal _user = new GenericPrincipal(new GenericIdentity("someUser"), null /* roles */);

			public override IPrincipal User
			{
				get
				{
					return _user;
				}
				set
				{
					base.User = value;
				}
			}
		}

		private class MockUserAuthenticationService : IUserAuthenticationService
		{
			public int MinPasswordLength
			{
				get { return 10; }
			}

			public ValidateUserStatus ValidateUser(string userName, string password)
			{
				return (userName == "someUser" && password == "goodPassword") ? ValidateUserStatus.Success : ValidateUserStatus.InvalidPassword;
			}

			public CreateUserStatus CreateUser(string userName, string password, string email)
			{
				if (userName == "duplicateUser")
				{
					return CreateUserStatus.DuplicateUserName;
				}

				// verify that values are what we expected
				Assert.AreEqual("goodPassword", password);
				Assert.AreEqual("goodEmail", email);

				return CreateUserStatus.Success;
			}

			public bool ChangePassword(string userName, string oldPassword, string newPassword)
			{
				return (userName == "someUser" && oldPassword == "goodOldPassword" && newPassword == "goodNewPassword");
			}

			public string ResolveToken(string token)
			{
				return (token == "validToken") ? "someUser" : null; ;
			}

			public bool ConfirmRegistration(string token)
			{
				return (token == "validToken");
			}

			public ResetUserPasswordStatus GetResetPasswordToken(string email)
			{
				if (email == "goodEmail")
				{
					return ResetUserPasswordStatus.Success;
				}
				return ResetUserPasswordStatus.EmailNotExist;
			}

			public bool ConfirmResetPassword(string username, string token, string newPassword)
			{
				return (username == "someUser" && token == "validToken" && newPassword == "goodNewPassword");
			}

			public bool IsUserInRole(string username, string role)
			{
				return true;
			}
		}

	}
}
