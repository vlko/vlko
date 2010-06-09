using System;
using System.Net.Mail;
using NLog;

namespace vlko.core.Services.Implementation
{
	public class EmailService : IEmailService
	{
		private static readonly Logger Logger;

		/// <summary>
		/// Initializes the <see cref="EmailService"/> class.
		/// </summary>
		static EmailService()
		{
			Logger = NLog.LogManager.GetLogger("EmailService");
		}

		/// <summary>
		/// Sends email to the specified reciptient.
		/// </summary>
		/// <param name="toRecipient">To recipient.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="text">The text.</param>
		/// <param name="asHtmlEmail">if set to <c>true</c> [as HTML email].</param>
		/// <returns>True if send succed; otherwise false;</returns>
		public bool Send(string toRecipient, string subject, string text, bool asHtmlEmail)
		{
			var email = new MailMessage();
			email.To.Add(toRecipient);
			email.Subject = subject;
			email.Body = text;
			email.IsBodyHtml = asHtmlEmail;

			try
			{
				new SmtpClient().Send(email);
				Logger.Debug(
					string.Format("Email to {0} with subject {1} sent.", email.To, email.Subject));
				return true;
			}
			catch (Exception e)
			{
				Logger.DebugException(
					string.Format("Send email to {0} with subject {1} failed.", email.To, email.Subject), e);
			}

			return false;
		}
	}
}