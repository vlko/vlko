using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace vlko.core.Services
{
	public interface IEmailService
	{

        /// <summary>
        /// Sends email to the specified recipient.
        /// </summary>
        /// <param name="from">Sender fo message.</param>
        /// <param name="toRecipient">To recipient.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="text">The text.</param>
        /// <param name="asHtmlEmail">if set to <c>true</c> [as HTML email].</param>
        /// <param name="bcc">The BCC.</param>
        /// <param name="replyTo">The reply to.</param>
        /// <param name="attachments">Attachments.</param>
        /// <returns>True if success.</returns>
		bool Send(string from, IEnumerable<string> toRecipient, string subject, string text, bool asHtmlEmail, IEnumerable<string> bcc = null, IEnumerable<string> replyTo = null, IEnumerable<Attachment> attachments = null);
	}
}
