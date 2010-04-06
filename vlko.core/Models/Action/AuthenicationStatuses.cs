using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vlko.core.Models.Action
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
