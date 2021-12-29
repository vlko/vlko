namespace vlko.core.Services
{
    public class MailTemplate
    {
        /// <summary>
        /// The subject template.
        /// </summary>
        public string SubjectTemplate { get; set; }

        /// <summary>
        /// The text template.
        /// </summary>
        public string TextTemplate { get; set; }

        /// <summary>
        /// A value indicating whether this instance is HTML mail.
        /// </summary>
        public bool IsHtml { get; set; }
    }
}