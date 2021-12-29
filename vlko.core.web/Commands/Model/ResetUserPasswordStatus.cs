namespace vlko.core.web.Commands.Model
{
    public enum ResetUserPasswordStatus
    {
        Success,
        WaitOneHour,
        UserNotVerified,
        EmailNotExist
    }
}