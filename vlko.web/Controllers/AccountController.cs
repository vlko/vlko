using System;
using System.Web.Mvc;
using System.Web.Routing;
using vlko.core.Action;
using vlko.core.Authentication;
using vlko.core.Base;
using vlko.core.InversionOfControl;
using vlko.BlogModule.Action;
using vlko.web.ViewModel.Account;
using ChangePasswordModel = vlko.web.ViewModel.Account.ChangePasswordModel;
using LogOnModel = vlko.web.ViewModel.Account.LogOnModel;
using RegisterModel = vlko.web.ViewModel.Account.RegisterModel;
using ResetPasswordModel = vlko.web.ViewModel.Account.ResetPasswordModel;


namespace vlko.web.Controllers
{

	[HandleError]
	public class AccountController : BaseController
	{
		public const string AllowAnonymousToRegisterUsersConst = "AllowAnonymousToRegisterUsers";

		public IFormsAuthenticationService FormsService { get; set; }
		public IUserAuthenticationService UserAuthenticationService { get; set; }

		/// <summary>
		/// Initializes data that might not be available when the constructor is called.
		/// </summary>
		/// <param name="requestContext">The HTTP context and route data.</param>
		protected override void Initialize(RequestContext requestContext)
		{
			if (FormsService == null) {
				FormsService = IoC.Resolve<IFormsAuthenticationService>();
			}
			if (UserAuthenticationService == null)
			{
				UserAuthenticationService = IoC.Resolve<IUserAuthenticationService>();
			}

			base.Initialize(requestContext);
		}

		/// <summary>
		/// URL: /Account/LogOn
		/// </summary>
		/// <returns>Action result.</returns>
		public ActionResult LogOn()
		{
			return View(new LogOnModel()
			            	{
			            		ReturnUrl = Request.QueryString["ReturnUrl"]
			            	});
		}

		/// <summary>
		/// URL: /Account/LogOn
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="returnUrl">The return URL.</param>
		/// <returns>Action result.</returns>
		[HttpPost]
		public ActionResult LogOn(LogOnModel model)
		{
			if (ModelState.IsValid)
			{
				var verifyStatus = UserAuthenticationService.ValidateUser(model.UserName, model.Password);
				if (verifyStatus == ValidateUserStatus.Success)
				{
					FormsService.SignIn(model.UserName, model.RememberMe);
					if (!String.IsNullOrEmpty(model.ReturnUrl))
					{
						return Redirect(model.ReturnUrl);
					}
					else
					{
						return RedirectToAction("Index", "Home");
					}
				}
				else
				{
					ModelState.AddModelError(string.Empty, AccountValidation.ValidateUserErrorCodeToString(verifyStatus));
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		/// <summary>
		/// URL: /account/LogOff
		/// </summary>
		/// <returns>Action result.</returns>
		public ActionResult LogOff()
		{
			FormsService.SignOut();

			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// URL: /Account/Register
		/// </summary>
		/// <returns>Action result.</returns>
		[ConditionalAuthorize(AllowAnonymousToRegisterUsersConst)]
		public ActionResult Register()
		{
			ViewData["PasswordLength"] = UserAuthenticationService.MinPasswordLength;
			return View();
		}

		/// <summary>
		/// URL: /Account/Register
		/// </summary>
		/// <param name="model">The model</param>
		/// <returns>Action result.</returns>
		[HttpPost]
		[ConditionalAuthorize(AllowAnonymousToRegisterUsersConst)]
		public ActionResult Register(RegisterModel model)
		{
			if (ModelState.IsValid)
			{
				// Attempt to register the user
				CreateUserStatus createStatus = UserAuthenticationService.CreateUser(model.UserName, model.Password, model.Email);

				if (createStatus == CreateUserStatus.Success)
				{
					return RedirectToAction("Index", "Home");
				}
				else
				{
					ModelState.AddModelError(string.Empty, AccountValidation.CreateUserErrorCodeToString(createStatus));
				}
			}

			// If we got this far, something failed, redisplay form
			ViewData["PasswordLength"] = UserAuthenticationService.MinPasswordLength;
			return View(model);
		}

		/// <summary>
		/// URL: /Account/ConfirmRegistration
		/// </summary>
		/// <param name="verifyToken">The verify token</param>
		/// <returns>Action result.</returns>
		[ConditionalAuthorize(AllowAnonymousToRegisterUsersConst)]
		public ActionResult ConfirmRegistration(string verifyToken)
		{
			if (!string.IsNullOrEmpty(verifyToken))
			{
				var username = UserAuthenticationService.ResolveToken(verifyToken);
				if (UserAuthenticationService.ConfirmRegistration(verifyToken))
				{
					FormsService.SignIn(username, false /* createPersistentCookie */);
					ViewData["user"] = username;
					return View();
				}
			}
			return View("NotValidToken");
		}

		/// <summary>
		/// URL: /Account/ChangePassword
		/// </summary>
		/// <returns>Action result.</returns>
		[Authorize]
		public ActionResult ChangePassword()
		{
			ViewData["PasswordLength"] = UserAuthenticationService.MinPasswordLength;
			return View();
		}

		/// <summary>
		/// URL: /Account/ChangePassword
		/// </summary>
		/// <param name="model">The model</param>
		/// <returns>Action result.</returns>
		[Authorize]
		[HttpPost]
		public ActionResult ChangePassword(ChangePasswordModel model)
		{
			if (ModelState.IsValid)
			{
				if (UserAuthenticationService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
				{
					return RedirectToAction("ChangePasswordSuccess");
				}
				else
				{
					ModelState.AddModelError(string.Empty, "The current password is incorrect or the new password is invalid.");
				}
			}

			// If we got this far, something failed, redisplay form
			ViewData["PasswordLength"] = UserAuthenticationService.MinPasswordLength;
			return View(model);
		}

		/// <summary>
		/// URL: /Account/ChangePasswordSuccess
		/// </summary>
		/// <returns>Action result.</returns>
		public ActionResult ChangePasswordSuccess()
		{
			return View();
		}

		/// <summary>
		/// URL: /Account/ResetPassword
		/// </summary>
		/// <returns>Action result.</returns>
		public ActionResult ResetPassword()
		{
			ViewData["PasswordLength"] = UserAuthenticationService.MinPasswordLength;
			return View();
		}

		/// <summary>
		/// URL: /Account/ResetPassword
		/// </summary>
		/// <param name="model">The model</param>
		/// <returns>Action result.</returns>
		[HttpPost]
		public ActionResult ResetPassword(ResetPasswordModel model)
		{
			if (ModelState.IsValid)
			{
				// Attempt to reset password
				ResetUserPasswordStatus resetStatus = UserAuthenticationService.GetResetPasswordToken(model.Email);

				if (resetStatus == ResetUserPasswordStatus.Success)
				{
					return RedirectToAction("Index", "Home");
				}
				else
				{
					ModelState.AddModelError(string.Empty, AccountValidation.ResetPasswordErrorCodeToString(resetStatus));
				}
			}
			return View(model);
		}

		/// <summary>
		/// URL: /Account/ConfirmResetPassword
		/// </summary>
		/// <param name="verifyToken">The verify token</param>
		/// <returns>Action result.</returns>
		public ActionResult ConfirmResetPassword(string verifyToken)
		{
			if (!string.IsNullOrEmpty(verifyToken))
			{
				var username = UserAuthenticationService.ResolveToken(verifyToken);
				ViewData["PasswordLength"] = UserAuthenticationService.MinPasswordLength;

				return View(new ConfirmResetPasswordModel()
								{
									Username = username,
									VerifyToken = verifyToken
								});
			}
			return View("NotValidToken");
		}

		/// <summary>
		/// URL: /Account/ResetPassword
		/// </summary>
		/// <param name="model">The model</param>
		/// <returns>Action result.</returns>
		[HttpPost]
		public ActionResult ConfirmResetPassword(ConfirmResetPasswordModel model)
		{
			if (ModelState.IsValid)
			{
				// Attempt to reset password
				bool succeed = UserAuthenticationService.ConfirmResetPassword(model.Username, model.VerifyToken, model.NewPassword);

				if (succeed)
				{
					FormsService.SignIn(model.Username, false /* createPersistentCookie */);
					return RedirectToAction("Index", "Home");
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Unable to reset password. Please contact administrator.");
				}
			}
			return View(model);
		}

		/// <summary>
		/// URL: /Account/NotAuthorized
		/// </summary>
		/// <returns>Action result.</returns>
		public ActionResult NotAuthorized()
		{
			return View();
		}
	}
}
