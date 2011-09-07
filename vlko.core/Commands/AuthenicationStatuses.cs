namespace vlko.core.Commands
{
    public enum CreateUserStatus
    {
        Success,
        DuplicateUserName,
        DuplicateEmail
    }

    public enum ValidateUserStatus
    {
        Success,
        NotExists,
        InvalidPassword,
        NotVerified
    }

    public enum ResetUserPasswordStatus
    {
        Success,
        WaitOneHour,
        UserNotVerified,
        EmailNotExist
    }
}
