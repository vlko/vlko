using System.Threading.Tasks;
using vlko.core.DBAccess;
using vlko.core.web.Commands.Model;

namespace vlko.core.web.Commands
{
    public interface IUserAuthentication : ICommands
    {
        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <param name="verifyToken">The verify token used for mailing user to verify.</param>
        /// <param name="verified">if set to <c>true</c> [verified].</param>
        /// <returns>Create user status.</returns>
        Task<TokenResult<CreateUserStatus>> CreateUser(string email, string password, bool verified = false);

        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>User validation status.</returns>
        Task<AuthWithRoles<ValidateUserStatus>> ValidateUser(string username, string password);

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if old password valid and new was set. Otherwise false.</returns>
        Task<bool> ChangePassword(string username, string oldPassword, string newPassword);

        /// <summary>
        /// Verifies the token to user.
        /// </summary>
        /// <param name="verifyToken">The verify token.</param>
        /// <returns>User name if token valid; otherwise null;</returns>
        Task<string> VerifyTokenToUser(string verifyToken);

        /// <summary>
        /// Confirms the registration.
        /// </summary>
        /// <param name="verifyToken">The verify token.</param>
        /// <returns>True if registration process succeed otherwise false;</returns>
        Task<AuthWithRoles<bool>> ConfirmRegistration(string verifyToken);

        /// <summary>
        /// Gets the reset password token.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="verifyToken">The verify token.</param>
        /// <returns>Reset user password status.</returns>
        Task<TokenResult<ResetUserPasswordStatus>> GetResetPasswordToken(string email);

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="verifyToken">The verify token.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if password changed.</returns>
        Task<AuthWithRoles<bool>> ResetPassword(string username, string verifyToken, string newPassword);
    }
}
