using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using vlko.core.web.Commands.Model;

namespace vlko.core.web.Authentication
{
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
        Task<AuthWithRoles<ValidateUserStatus>> ValidateUser(string userName, string password);

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <param name="verified">if set to <c>true</c> [verified automatically without mail confirmation].</param>
        /// <returns>User creation status.</returns>
        Task<CreateUserStatus> CreateUser(IUrlHelper url, string email, string password, bool verified = false, string emailTemplate = null, string subject = null);

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if success otherwise false.</returns>
        Task<bool> ChangePassword(string userName, string oldPassword, string newPassword);

        /// <summary>
        /// Resolves the token to user name.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>User name or null if token not valid.</returns>
        Task<string> ResolveToken(string token);

        /// <summary>
        /// Confirms the registration.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>True if success otherwise false.</returns>
        Task<AuthWithRoles<bool>> ConfirmRegistration(string token);

        /// <summary>
        /// Gets the reset password token.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>Reset user password status.</returns>
        Task<ResetUserPasswordStatus> GetResetPasswordToken(IUrlHelper url, string email);

        /// <summary>
        /// Confirms the reset password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="token">The token.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if success otherwise false.</returns>
        Task<AuthWithRoles<bool>> ConfirmResetPassword(string username, string token, string newPassword);
    }
}