using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vlko.core.Services
{
    public interface IAppInfoService
    {
        /// <summary>
        /// Gets the application name.
        /// </summary>
        /// <value>The application name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the root URL.
        /// </summary>
        /// <value>The root URL.</value>
        string RootUrl { get; }

        /// <summary>
        /// Gets the root path.
        /// </summary>
        /// <value>The root path.</value>
        string RootPath { get; }

        /// <summary>
        /// Gets the registration mail template.
        /// </summary>
        /// <returns>Mail template.</returns>
        MailTemplate GetRegistrationMailTemplate();

        /// <summary>
        /// Gets the reset password mail template.
        /// </summary>
        /// <returns>Mail template.</returns>
        MailTemplate GetResetPasswordMailTemplate();
    }
}
