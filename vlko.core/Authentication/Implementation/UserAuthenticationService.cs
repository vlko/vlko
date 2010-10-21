using System;
using vlko.core.Services;
using vlko.model;
using vlko.model.Action;

namespace vlko.core.Authentication.Implementation
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
                return User.MinRequiredPasswordLength;
            }
        }

        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public ValidateUserStatus ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

            return _userAuthentication.ValidateUser(userName, password);
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="email">The email.</param>
        /// <returns>User creation status.</returns>
        public CreateUserStatus CreateUser(string userName, string password, string email)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("Value cannot be null or empty.", "email");

            string token;
            CreateUserStatus status =_userAuthentication.CreateUser(userName, password, email, out token);
            if (status == CreateUserStatus.Success)
            {
                var registerMessage = _appInfoService.GetRegistrationMailTemplate();
                _emailService.Send(email,
                                   registerMessage.SubjectTemplate,
                                   string.Format(
                                       registerMessage.TextTemplate,
                                       _appInfoService.RootUrl + "/Account/ConfirmRegistration/" + token
                                       ),
                                   registerMessage.IsHtml);
            }
            return status;
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if success; otherwise false</returns>
        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            return _userAuthentication.ChangePassword(userName, oldPassword, newPassword);
        }

        /// <summary>
        /// Resolves the token to user name.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>User name or null if token not valid.</returns>
        public string ResolveToken(string token)
        {
            if (String.IsNullOrEmpty(token)) throw new ArgumentException("Token cannot be null or empty.", "token");

            return _userAuthentication.VerifyTokenToUser(token);
        }

        /// <summary>
        /// Confirms the registration.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>True if success otherwise false.</returns>
        public bool ConfirmRegistration(string token)
        {
            if (String.IsNullOrEmpty(token)) throw new ArgumentException("Token cannot be null or empty.", "token");

            return _userAuthentication.ConfirmRegistration(token);
        }

        /// <summary>
        /// Gets the reset password token.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>Reset user password status.</returns>
        public ResetUserPasswordStatus GetResetPasswordToken(string email)
        {
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("Email cannot be null or empty.", "email");

            string token;
            ResetUserPasswordStatus status = _userAuthentication.GetResetPasswordToken(email, out token);
            if (status == ResetUserPasswordStatus.Success)
            {
                var resetPasswordMessage = _appInfoService.GetResetPasswordMailTemplate();
                _emailService.Send(email,
                                   resetPasswordMessage.SubjectTemplate,
                                   string.Format(
                                       resetPasswordMessage.TextTemplate,
                                       _appInfoService.RootUrl + "/Account/ConfirmResetPassword/" + token
                                       ),
                                   resetPasswordMessage.IsHtml);
            }
            return status;
        }

        /// <summary>
        /// Confirms the reset password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="token">The token.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if success otherwise false.</returns>
        public bool ConfirmResetPassword(string username, string token, string newPassword)
        {
            if (String.IsNullOrEmpty(username)) throw new ArgumentException("User name cannot be null or empty.", "username");
            if (String.IsNullOrEmpty(token)) throw new ArgumentException("Token cannot be null or empty.", "token");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Password cannot be null or empty.", "newPassword");

            return _userAuthentication.ResetPassword(username, token, newPassword);
        }

        /// <summary>
        /// Determines whether [is user in role] [the specified username].
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="role">The role.</param>
        /// <returns>
        /// 	<c>true</c> if [is user in role] [the specified username]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsUserInRole(string username, string role)
        {
            if (String.IsNullOrEmpty(username)) throw new ArgumentException("User name cannot be null or empty.", "username");
            if (String.IsNullOrEmpty(role)) throw new ArgumentException("Role cannot be null or empty.", "role");

            return _userAuthentication.IsUserInRole(username, role);
        }
    }
}