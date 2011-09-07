using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vlko.BlogModule.Commands;
using vlko.BlogModule.Commands.ComplexHelpers.Twitter;
using vlko.core.Authentication;
using vlko.core.Base;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.core.Services;
using vlko.core.ValidationAtribute;
using vlko.BlogModule;

namespace vlko.web.Areas.Admin.Controllers
{
	[AuthorizeRoles(Settings.AdminRole)]
	[AreaCheck("Admin")]
	public class TwitterController : BaseController
	{
		/// <summary>
		/// URL: Twitter/Authorize
		/// </summary>
		/// <returns>Action result.</returns>
		public ActionResult Authorize()
		{
			try
			{
				var twitterConnection = RepositoryFactory.Command<ITwitterConnection>();

				if (!IsCurrentTokenValid(twitterConnection))
				{
					var registerUrl = twitterConnection.GetAuthorizeUrl(
						new ConsumerAppIdent
							{
								ConsumerKey = Settings.Twitter.ConsumerKey.Value,
								ConsumerSecret = Settings.Twitter.ConsumerSecret.Value
							},
						GetAuthorizeReturnUrl()
						);

					return Redirect(registerUrl);
				}
				return ViewWithAjax("AuthorizeResult", "Current twitter token is valid.");
			}
			catch (Exception e)
			{
				return ViewWithAjax("AuthorizeResult", "Twitter authorization failed." + e.Message);
			}
		}

		/// <summary>
		/// Determines whether [is current token valid] [the specified twitter connection].
		/// </summary>
		/// <param name="twitterConnection">The twitter connection.</param>
		/// <returns>
		/// 	<c>true</c> if [is current token valid] [the specified twitter connection]; otherwise, <c>false</c>.
		/// </returns>
		private bool IsCurrentTokenValid(ITwitterConnection twitterConnection)
		{
			return twitterConnection.IsTokenValid(new OAuthToken
			                           	{
			                           		ConsumerKey = Settings.Twitter.ConsumerKey.Value,
			                           		ConsumerSecret = Settings.Twitter.ConsumerSecret.Value,
			                           		Token = Settings.Twitter.Token.Value,
			                           		TokenSecret = Settings.Twitter.TokenSecret.Value
			                           	});

		}

		/// <summary>
		/// URL: Twitter/AuthorizeConfirmed
		/// </summary>
		/// <param name="oauth_token">The oauth_token.</param>
		/// <param name="oauth_verifier">The oauth_verifier.</param>
		/// <returns>Action result.</returns>
		public ActionResult AuthorizeConfirmed(string oauth_token, string oauth_verifier)
		{
			try
			{
				var twitterConnection = RepositoryFactory.Command<ITwitterConnection>();

				var token = twitterConnection.GetOAuthToken(
					new ConsumerAppIdent
					{
						ConsumerKey = Settings.Twitter.ConsumerKey.Value,
						ConsumerSecret = Settings.Twitter.ConsumerSecret.Value
					},
					oauth_token,
					oauth_verifier
					);

				Settings.Twitter.Token.SaveValue(token.Token);
				Settings.Twitter.TokenSecret.SaveValue(token.TokenSecret);

				return ViewWithAjax("AuthorizeResult", "Twitter authorized successfully.");
			}
			catch (Exception e)
			{
				return ViewWithAjax("AuthorizeResult", "Twitter authorization failed." + e.Message);
			}
		}
		/// <summary>
		/// Gets the authorize return URL.
		/// </summary>
		/// <returns></returns>
		private static string GetAuthorizeReturnUrl()
		{
			var rootUrl = IoC.Resolve<IAppInfoService>().RootUrl;
			return rootUrl + "/Admin/Twitter/AuthorizeConfirmed";
		}
	}
}