using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using vlko.core.Action;
using vlko.core.Authentication;
using vlko.core.InversionOfControl;
using vlko.core.Services;
using vlko.web.Controllers;
using vlko.web.ViewModel.Account;

namespace vlko.web.Tests.Controllers
{

	[TestClass]
	public class AccountControllerTest : BaseControllerTest
	{
		public class LocalEmailSender : IEmailService
		{
			public bool Send(string toRecipient, string subject, string text, bool asHtmlEmail)
			{
				return true;
			}
		}

		protected override void FillDbWithData()
		{
			IoC.AddRerouting<IEmailService>(() => new LocalEmailSender());
		}

		[TestMethod]
		public void Change_password_get_returns_view()
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
		public void Change_password_post_returns_redirect_on_success()
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
		public void Change_password_post_returns_view_if_change_password_fails()
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
		public void Change_password_post_returns_view_if_model_state_is_invalid()
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
		public void Change_password_success_returns_view()
		{
			// Arrange
			AccountController controller = GetAccountController();

			// Act
			ActionResult result = controller.ChangePasswordSuccess();

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));
		}

		[TestMethod]
		public void Log_off_logs_out_and_redirects()
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
		public void Log_on_get_returns_view()
		{
			// Arrange
			AccountController controller = GetAccountController();

			TestControllerBuilder builder = new TestControllerBuilder();
			builder.InitializeController(controller);

			// Act
			ActionResult result = controller.LogOn();

			// Assert
			result.AssertViewRendered();
		}

		[TestMethod]
		public void Log_on_post_returns_redirect_on_success_without_return_url()
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
			result.AssertActionRedirect()
				.ToController("Home")
				.ToAction("Index");

			Assert.IsTrue(((MockFormsAuthenticationService)controller.FormsService).SignIn_WasCalled);
		}

		[TestMethod]
		public void Log_on_post_returns_redirect_on_success_with_return_url()
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
			result.AssertHttpRedirect().ToUrl("/someUrl");

			Assert.IsTrue(((MockFormsAuthenticationService)controller.FormsService).SignIn_WasCalled);
		}

		[TestMethod]
		public void Log_on_post_returns_view_if_model_state_is_invalid()
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
			var logonModel = result.AssertViewRendered().WithViewData<LogOnModel>();
			Assert.AreEqual(model, logonModel);
		}

		[TestMethod]
		public void Log_on_post_returns_view_if_validate_user_fails()
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
			var logonModel = result.AssertViewRendered().WithViewData<LogOnModel>();
			Assert.AreEqual(model, logonModel);

			Assert.AreNotEqual(string.Empty, controller.ModelState[""].Errors[0].ErrorMessage);
		}

		[TestMethod]
		public void Register_get_returns_view()
		{
			// Arrange
			AccountController controller = GetAccountController();

			// Act
			ActionResult result = controller.Register();

			// Assert
			var viewData = result.AssertViewRendered().ViewData;
			Assert.AreEqual(10, viewData["PasswordLength"]);
		}

		[TestMethod]
		public void Register_post_returns_redirect_on_success()
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
			result.AssertActionRedirect()
				.ToController("Home")
				.ToAction("Index");
		}

		[TestMethod]
		public void Register_post_returns_view_if_registration_fails()
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
			var registerModel = result.AssertViewRendered().WithViewData<RegisterModel>();

			Assert.AreEqual(model, registerModel);
			Assert.AreEqual("Username already exists. Please enter a different user name.", controller.ModelState[""].Errors[0].ErrorMessage);
			Assert.AreEqual(10, result.AssertViewRendered().ViewData["PasswordLength"]);
		}

		[TestMethod]
		public void Register_post_returns_view_if_model_state_is_invalid()
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
			var registerModel = result.AssertViewRendered().WithViewData<RegisterModel>();

			Assert.AreEqual(model, registerModel);
			Assert.AreEqual(10, result.AssertViewRendered().ViewData["PasswordLength"]);
		}

		[TestMethod]
		public void Register_confirm_registration_valid()
		{
			// Arrange
			AccountController controller = GetAccountController();

			// Act
			ActionResult result = controller.ConfirmRegistration("validToken");

			// Assert
			var viewData = result.AssertViewRendered().ViewData;
			Assert.AreEqual("someUser", viewData["user"]);
		}

		[TestMethod]
		public void Register_confirm_registration_failed()
		{
			// Arrange
			AccountController controller = GetAccountController();

			// Act
			ActionResult result = controller.ConfirmRegistration("not_valid_token");

			// Assert
			string viewName = result.AssertViewRendered().ViewName;
			Assert.AreEqual("NotValidToken", viewName);
		}

		[TestMethod]
		public void Reset_password_post_returns_redirect_on_success()
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
			result.AssertActionRedirect()
				.ToController("Home")
				.ToAction("Index");
		}

		[TestMethod]
		public void Reset_password_post_returns_view_if_fails()
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
			var resetModel = result.AssertViewRendered().WithViewData<ResetPasswordModel>();
			Assert.AreEqual(model, resetModel);
			Assert.AreEqual("Invalid email address. Please enter a different e-mail address.", controller.ModelState[""].Errors[0].ErrorMessage);
		}

		[TestMethod]
		public void Reset_password_post_returns_view_if_model_state_is_invalid()
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
			var resetModel = result.AssertViewRendered().WithViewData<ResetPasswordModel>();
			Assert.AreEqual(model, resetModel);
		}

		[TestMethod]
		public void Confirm_reset_password_returns_view()
		{
			// Arrange
			AccountController controller = GetAccountController();

			// Act
			ActionResult result = controller.ConfirmResetPassword("validToken");

			// Assert
			var viewModel = result.AssertViewRendered().WithViewData<ConfirmResetPasswordModel>();

			Assert.AreEqual("someUser", viewModel.Username);
			Assert.AreEqual("validToken", viewModel.VerifyToken);
		}

		[TestMethod]
		public void Confirm_reset_password_post_returns_redirect_on_success()
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
			result.AssertActionRedirect()
				.ToController("Home")
				.ToAction("Index");
		}

		[TestMethod]
		public void Confirm_reset_password_post_returns_view_if_fails()
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
			var viewModel = result.AssertViewRendered().WithViewData<ConfirmResetPasswordModel>();

			Assert.AreEqual(model, viewModel);
			Assert.AreEqual("Unable to reset password. Please contact administrator.", controller.ModelState[""].Errors[0].ErrorMessage);
		}

		[TestMethod]
		public void Confirm_reset_password_post_returns_view_if_model_state_is_invalid()
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
			var viewModel = result.AssertViewRendered().WithViewData<ConfirmResetPasswordModel>();

			Assert.AreEqual(model, viewModel);
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
		}

	}
}
