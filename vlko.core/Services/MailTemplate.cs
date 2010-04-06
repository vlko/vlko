using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vlko.core.Services
{
    public class MailTemplate
    {
        /// <summary>
        /// Gets or sets the subject template.
        /// </summary>
        /// <value>The subject template.</value>
        public string SubjectTemplate { get; set; }

        /// <summary>
        /// Gets or sets the text template.
        /// </summary>
        /// <value>The text template.</value>
        public string TextTemplate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is HTML mail.
        /// </summary>
        /// <value><c>true</c> if this instance is HTML mail; otherwise, <c>false</c>.</value>
        public bool IsHtml { get; set; }
    }
}
