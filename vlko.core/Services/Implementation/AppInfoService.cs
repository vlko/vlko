using System;

namespace vlko.core.Services.Implementation
{
    public class AppInfoService : IAppInfoService
    {
        /// <summary>
        /// Gets the application name.
        /// </summary>
        /// <value>The application name.</value>
        public string Name
        {
            get { return "vlko.preweb.sk"; }
        }

        /// <summary>
        /// Gets the root URL.
        /// </summary>
        /// <value>The root URL.</value>
        public string RootUrl
        {
            get { return "http://vlko.preweb.sk"; }
        }

        /// <summary>
        /// Gets the registration mail template.
        /// </summary>
        /// <returns>Mail template.</returns>
        public MailTemplate GetRegistrationMailTemplate()
        {
            return new MailTemplate()
                       {
                           SubjectTemplate = "Registration for " + Name,
                           TextTemplate = "To finish registration process for " + Name + " go to following link\n{0}"
                       };
        }

        /// <summary>
        /// Gets the reset password mail template.
        /// </summary>
        /// <returns>Mail template.</returns>
        public MailTemplate GetResetPasswordMailTemplate()
        {
            return new MailTemplate()
            {
                SubjectTemplate = "Reset password for " + Name,
                TextTemplate = "To finish reset password process for " + Name + " go to following link\n{0}"
            };
        }
    }
}