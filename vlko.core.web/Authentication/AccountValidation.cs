using vlko.core.web.Commands.Model;

namespace vlko.core.web.Authentication
{

    public static class AccountValidation
    {
        /// <summary>
        /// Create user status error code to string.
        /// </summary>
        /// <param name="createStatus">The create status.</param>
        /// <returns>String representation of error code.</returns>
        public static string CreateUserErrorCodeToString(CreateUserStatus createStatus)
        {
            switch (createStatus)
            {
                case CreateUserStatus.DuplicateUserName:
                    return "Email je už zaregistrovaný.";

                case CreateUserStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        /// <summary>
        /// Validate user status error code to string.
        /// </summary>
        /// <param name="validateStatus">The validate status.</param>
        /// <returns>String representation of error code.</returns>
        public static string ValidateUserErrorCodeToString(ValidateUserStatus validateStatus)
        {
            switch (validateStatus)
            {
                case ValidateUserStatus.NotExists:
                    return "The user name provided is invalid. Please check the value and try again.";

                case ValidateUserStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case ValidateUserStatus.NotVerified:
                    return "Confirmation process not yet finished, please check email and finish it.";


                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        /// <summary>
        /// Resets the password error code to string.
        /// </summary>
        /// <param name="resetPasswordStatus">The reset password status.</param>
        /// <returns>String representation of error code.</returns>
        public static string ResetPasswordErrorCodeToString(ResetUserPasswordStatus resetPasswordStatus)
        {
            switch (resetPasswordStatus)
            {
                case ResetUserPasswordStatus.EmailNotExist:
                    return "Invalid email address. Please enter a different e-mail address.";
                case ResetUserPasswordStatus.UserNotVerified:
                    return "User was not yet verified, please check your mailbox or please contact administrator.";
                case ResetUserPasswordStatus.WaitOneHour:
                    return "Double reset request, you should wait at least one hour for next password reset request.";
                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}


