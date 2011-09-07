using System.ComponentModel.Composition;
using vlko.core.Repository;
using vlko.core.Roots;

namespace vlko.core.Action
{
	[InheritedExport]
	public interface IUserAuthentication : ICommandGroup<User>
    {
        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="email">The email.</param>
        /// <param name="verifyToken">The verify token used for mailing user to verify.</param>
        /// <returns>Create user status.</returns>
        CreateUserStatus CreateUser(string username, string password, string email, out string verifyToken);

        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>User validation status.</returns>
        ValidateUserStatus ValidateUser(string username, string password);

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if old password valid and new was set. Otherwise false.</returns>
        bool ChangePassword(string username, string oldPassword, string newPassword);

        /// <summary>
        /// Verifies the token to user.
        /// </summary>
        /// <param name="verifyToken">The verify token.</param>
        /// <returns>User name if token valid; otherwise null;</returns>
        string VerifyTokenToUser(string verifyToken);

        /// <summary>
        /// Confirms the registration.
        /// </summary>
        /// <param name="verifyToken">The verify token.</param>
        /// <returns>True if registration process succeed otherwise false;</returns>
        bool ConfirmRegistration(string verifyToken);

        /// <summary>
        /// Gets the reset password token.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="verifyToken">The verify token.</param>
        /// <returns>Reset user password status.</returns>
        ResetUserPasswordStatus GetResetPasswordToken(string email, out string verifyToken);

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="verifyToken">The verify token.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if password changed.</returns>
        bool ResetPassword(string username, string verifyToken, string newPassword);
    }
}
