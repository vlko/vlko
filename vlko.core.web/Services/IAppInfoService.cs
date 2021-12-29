using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vlko.core.Services;

namespace vlko.core.web.Services
{
    public interface IAppInfoService
    {
        /// <summary>
        /// Gets the application name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Default "from email" used to sends email.
        /// </summary>
        string FromEmail { get; }

        /// <summary>
        /// The root URL without slash at the end.
        /// </summary>
        string RootUrl { get; }
        /*
        /// <summary>
        /// The root path.
        /// </summary>
        string RootPath { get; }*/

        /// <summary>
        /// The registration mail template.
        /// </summary>
        MailTemplate GetRegistrationMailTemplate();

        /// <summary>
        /// The welcome mail template.
        /// </summary>
        MailTemplate GetWelcomeTemplate(IUrlHelper url, string email, string emailTemplate = null, string subject = null);

        /// <summary>
        /// The reset password mail template.
        /// </summary>
        MailTemplate GetResetPasswordMailTemplate(IUrlHelper url);
    }
}
