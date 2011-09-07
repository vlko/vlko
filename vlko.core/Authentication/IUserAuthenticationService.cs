using System.ComponentModel.Composition;
using vlko.core.Commands;

namespace vlko.core.Authentication
{
	[InheritedExport]
    public interface IUserAuthenticationService
    {
        /// <summary>
        /// Gets the length of the min password.
        /// </summary>
        /// <value>The length of the min password.</value>
        int MinPasswordLength { get; }

        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>validate user status.</returns>
        ValidateUserStatus ValidateUser(string userName, string password);

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="email">The email.</param>
        /// <returns>Create user status.</returns>
        CreateUserStatus CreateUser(string userName, string password, string email);

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if success otherwise false.</returns>
        bool ChangePassword(string userName, string oldPassword, string newPassword);

        /// <summary>
        /// Resolves the token to user name.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>User name or null if token not valid.</returns>
        string ResolveToken(string token);

        /// <summary>
        /// Confirms the registration.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>True if success otherwise false.</returns>
        bool ConfirmRegistration(string token);

        /// <summary>
        /// Gets the reset password token.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>Reset user password status.</returns>
        ResetUserPasswordStatus GetResetPasswordToken(string email);

        /// <summary>
        /// Confirms the reset password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="token">The token.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if success otherwise false.</returns>
        bool ConfirmResetPassword(string username, string token, string newPassword);
    }
}