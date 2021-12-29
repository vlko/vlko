using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using vlko.core.Roots;
using vlko.core.Services;
using vlko.core.web.Commands;
using vlko.core.web.Commands.Model;
using vlko.core.web.Services;

namespace vlko.core.web.Authentication.Implementation
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly IUserAuthentication _userAuthentication;
        private readonly IEmailService _emailService;
        private readonly IAppInfoService _appInfoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAuthenticationService"/> class.
        /// </summary>
        /// <param name="userAuthentication">The user authentication.</param>
        /// <param name="emailService">The email service.</param>
        /// <param name="appInfoService">The app info service.</param>
        public UserAuthenticationService(IUserAuthentication userAuthentication, IEmailService emailService, IAppInfoService appInfoService)
        {
            _userAuthentication = userAuthentication;
            _emailService = emailService;
            _appInfoService = appInfoService;
        }

        /// <summary>
        /// Gets the length of the min password.
        /// </summary>
        /// <value>The length of the min password.</value>
        public int MinPasswordLength
        {
            get
            {
                return WebSettings.MinRequiredPasswordLength;
            }
        }

        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public async Task<AuthWithRoles<ValidateUserStatus>> ValidateUser(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

            return await _userAuthentication.ValidateUser(userName, password);
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <param name="verified">if set to <c>true</c> [verified automatically without mail confirmation].</param>
        /// <returns>
        /// User creation status.
        /// </returns>
        /// <exception cref="ArgumentException">Value cannot be null or empty.;password
        /// or
        /// Value cannot be null or empty.;email</exception>
        public async Task<CreateUserStatus> CreateUser(IUrlHelper url, string email, string password, bool verified = false, string emailTemplate = null, string subject = null)
        {
            email = email.Trim();
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");
            if (string.IsNullOrEmpty(email)) throw new ArgumentException("Value cannot be null or empty.", "email");

            var createResult = await _userAuthentication.CreateUser(email, password, verified);
            if (createResult.Status == CreateUserStatus.Success && subject != "none")
            {
                if (verified)
                {
                    var welcomeTemplate = _appInfoService.GetWelcomeTemplate(url, email, emailTemplate, subject);
                    _emailService.Send(
                        _appInfoService.FromEmail,
                        new[] { email },
                        welcomeTemplate.SubjectTemplate,
                        welcomeTemplate.TextTemplate,
                        welcomeTemplate.IsHtml,
                        bcc: new[] { WebSettings.AdditionalBCCEmailForRegistration.Value });
                }
                else
                {
                    var registerMessage = _appInfoService.GetRegistrationMailTemplate();
                    _emailService.Send(
                        _appInfoService.FromEmail,
                        new[] { email },
                        registerMessage.SubjectTemplate,
                        string.Format(
                            registerMessage.TextTemplate,
                            _appInfoService.RootUrl + "/Account/ConfirmRegistration/" + createResult.VerifyToken
                            ),
                        registerMessage.IsHtml,
                        bcc: new[] { WebSettings.AdditionalBCCEmailForRegistration.Value });
                }
            }
            return createResult.Status;
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if success; otherwise false</returns>
        public async Task<bool> ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (string.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
            if (string.IsNullOrEmpty(newPassword)) throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            return await _userAuthentication.ChangePassword(userName, oldPassword, newPassword);
        }

        /// <summary>
        /// Resolves the token to user name.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>User name or null if token not valid.</returns>
        public async Task<string> ResolveToken(string token)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentException("Token cannot be null or empty.", "token");

            return await _userAuthentication.VerifyTokenToUser(token);
        }

        /// <summary>
        /// Confirms the registration.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>True if success otherwise false.</returns>
        public async Task<AuthWithRoles<bool>> ConfirmRegistration(string token)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentException("Token cannot be null or empty.", "token");

            return await _userAuthentication.ConfirmRegistration(token);
        }

        /// <summary>
        /// Gets the reset password token.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>Reset user password status.</returns>
        public async Task<ResetUserPasswordStatus> GetResetPasswordToken(IUrlHelper url, string email)
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentException("Email cannot be null or empty.", "email");

            var resetResult = await _userAuthentication.GetResetPasswordToken(email);
            if (resetResult.Status == ResetUserPasswordStatus.Success)
            {
                var resetPasswordMessage = _appInfoService.GetResetPasswordMailTemplate(url);
                _emailService.Send(
                        _appInfoService.FromEmail,
                        new[] { email },
                        resetPasswordMessage.SubjectTemplate,
                        string.Format(
                            resetPasswordMessage.TextTemplate,
                            _appInfoService.RootUrl + "/Account/ConfirmResetPassword/" + resetResult.VerifyToken
                            ),
                        resetPasswordMessage.IsHtml);
            }
            return resetResult.Status;
        }

        /// <summary>
        /// Confirms the reset password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="token">The token.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if success otherwise false.</returns>
        public async Task<AuthWithRoles<bool>> ConfirmResetPassword(string username, string token, string newPassword)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("User name cannot be null or empty.", "username");
            if (string.IsNullOrEmpty(token)) throw new ArgumentException("Token cannot be null or empty.", "token");
            if (string.IsNullOrEmpty(newPassword)) throw new ArgumentException("Password cannot be null or empty.", "newPassword");

            return await _userAuthentication.ResetPassword(username, token, newPassword);
        }
    }
}