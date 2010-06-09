using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vlko.core.Services
{
	public interface IEmailService
	{

		/// <summary>
		/// Sends email to the specified reciptient.
		/// </summary>
		/// <param name="toRecipient">To recipient.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="text">The text.</param>
		/// <param name="asHtmlEmail">if set to <c>true</c> [as HTML email].</param>
		/// <returns></returns>
		bool Send(string toRecipient, string subject, string text, bool asHtmlEmail);
	}
}
