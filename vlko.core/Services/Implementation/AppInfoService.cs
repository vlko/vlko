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
			get
			{
				System.Web.HttpContext context = System.Web.HttpContext.Current;
				if (context != null)
				{
					string baseUrl = context.Request.Url.Scheme + "://" + context.Request.Url.Authority + context.Request.ApplicationPath.TrimEnd('/');
					return baseUrl;
				}
				return Settings.BaseUrl.Value;
			}
		}

		/// <summary>
		/// Gets the root path.
		/// </summary>
		/// <value>The root path.</value>
		public string RootPath
		{
			get
			{
				return System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
			}
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