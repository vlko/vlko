using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace vlko.core.Services
{
	[InheritedExport]
	public interface IAppInfoService
	{
		/// <summary>
		/// Gets the application name.
		/// </summary>
		/// <value>The application name.</value>
		string Name { get; }

		/// <summary>
		/// Gets the root URL without slash at the end.
		/// </summary>
		/// <value>The root URL without slash at the end.</value>
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
