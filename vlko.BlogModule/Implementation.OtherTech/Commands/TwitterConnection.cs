using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Security.Application;
using TweetSharp;
using vlko.BlogModule.Commands;
using vlko.BlogModule.Commands.ComplexHelpers.Twitter;
using vlko.core.Repository;
using TwitterStatus = vlko.BlogModule.Roots.TwitterStatus;

namespace vlko.BlogModule.Implementation.OtherTech.Commands
{
	public class TwitterConnection : CommandGroup<TwitterStatus>, ITwitterConnection
	{
		/// <summary>
		/// Gets the authorize URL.
		/// </summary>
		/// <param name="consumerAppIdent">The consumer app ident.</param>
		/// <param name="returnUrl">The return URL.</param>
		/// <returns>Authorize URL.</returns>
		public string GetAuthorizeUrl(ConsumerAppIdent consumerAppIdent, string returnUrl)
		{
            var service = new TwitterService(consumerAppIdent.ConsumerKey, consumerAppIdent.ConsumerSecret);

            var requestToken = service.GetRequestToken(returnUrl);

            return service.GetAuthorizationUri(requestToken).AbsoluteUri;
		}

		/// <summary>
		/// Gets the OAuth token.
		/// </summary>
		/// <param name="consumerAppIdent">The consumer app ident.</param>
		/// <param name="requestToken">The request token.</param>
		/// <param name="requestVerifier">The request verifier.</param>
		/// <returns>OAuth token.</returns>
		public OAuthToken GetOAuthToken(ConsumerAppIdent consumerAppIdent, string requestToken, string requestVerifier)
		{
            var service = new TwitterService(consumerAppIdent.ConsumerKey, consumerAppIdent.ConsumerSecret);
            var accessToken = service.GetAccessToken(new OAuthRequestToken {Token = requestToken}, requestVerifier);

			return new OAuthToken
					{
						ConsumerKey = consumerAppIdent.ConsumerKey,
						ConsumerSecret = consumerAppIdent.ConsumerSecret,
                        Token = accessToken.Token,
                        TokenSecret = accessToken.TokenSecret
					};
		}

		/// <summary>
		/// Determines whether [is token valid] [the specified consumer app ident].
		/// </summary>
		/// <param name="token">The token.</param>
		/// <returns>
		/// 	<c>true</c> if [is token valid] [the specified consumer app ident]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsTokenValid(OAuthToken token)
		{
			try
			{
                var service = new TwitterService(token.ConsumerKey, token.ConsumerSecret, token.Token, token.TokenSecret);
			    var user = service.VerifyCredentials();
			    return user != null;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the statuses for user.
		/// </summary>
		/// <param name="token">The token.</param>
		/// <param name="userName">Name of the user.</param>
		/// <param name="page">The page.</param>
		/// <param name="pageCount">The page count.</param>
		/// <returns>
		/// List of twitter status for specified user.
		/// </returns>
		public TwitterStatus[] GetStatusesForUser(OAuthToken token, string userName, int page = 0, int pageCount = 50)
		{
            var service = new TwitterService(token.ConsumerKey, token.ConsumerSecret, token.Token, token.TokenSecret);
            var timelineResult = service.ListTweetsOnUserTimeline(page, pageCount);

            var result = from status in timelineResult
						 select new TwitterStatus
									{
										TwitterId = Convert.ToInt64(status.Id),
										CreatedDate = status.CreatedDate.ToLocalTime(),
										User = status.User.ScreenName,
										Description = status.RetweetedStatus == null 
											? ParseStatusTextToHtml(status.Text) 
											: ParseStatusTextToHtml(status.RetweetedStatus.Text),
										Reply = status.InReplyToStatusId.HasValue,
										RetweetUser = status.RetweetedStatus != null ? status.RetweetedStatus.User.ScreenName : null,
										Hidden = status.InReplyToStatusId.HasValue
									};
			return result.ToArray();			
		}

        /// <summary>
        /// Gets the retweets for user.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageCount">The page count.</param>
        /// <returns>List of retweets of current user.</returns>
        public TwitterStatus[] GetRetweetsForUser(OAuthToken token, string userName, int page = 0, int pageCount = 50)
        {
            var service = new TwitterService(token.ConsumerKey, token.ConsumerSecret, token.Token, token.TokenSecret);
            var timelineResult = service.ListRetweetsByMe(page, pageCount);

            var result = from status in timelineResult
                         select new TwitterStatus
                         {
                             TwitterId = Convert.ToInt64(status.Id),
                             CreatedDate = status.CreatedDate.ToLocalTime(),
                             User = status.User.ScreenName,
                             Description = status.RetweetedStatus == null
                                 ? ParseStatusTextToHtml(status.Text)
                                 : ParseStatusTextToHtml(status.RetweetedStatus.Text),
                             Reply = status.InReplyToStatusId.HasValue,
                             RetweetUser = status.RetweetedStatus != null ? status.RetweetedStatus.User.ScreenName : null,
                             Hidden = status.InReplyToStatusId.HasValue
                         };
            return result.ToArray();
        }

		/********************************************
		* following code reused from TweetSharp project http://tweetsharp.codeplex.com/SourceControl/changeset/view/10ab65e64a56#src%2fvs2010%2fnet40%2fTweetSharp%2fCore%2fExtensions%2fStringExtensions.cs
		********************************************/

		/// <summary>
		///  Jon Gruber's URL Regex: http://daringfireball.net/2009/11/liberal_regex_for_matching_urls
		/// </summary>
		private static readonly Regex ParseUrls = new Regex(@"\b(([\w-]+://?|www[.])[^\s()<>]+(?:\([\w\d]+\)|([^\p{P}\s]|/)))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <summary>
		///  Diego Sevilla's @ Regex: http://stackoverflow.com/questions/529965/how-could-i-combine-these-regex-rules
		/// </summary>
		private static readonly Regex ParseMentions = new Regex(@"(^|\W)@([A-Za-z0-9_]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <summary>
		///  Simon Whatley's # Regex: http://www.simonwhatley.co.uk/parsing-twitter-usernames-hashtags-and-urls-with-javascript
		/// </summary>
		private static readonly Regex ParseHashtags = new Regex("[#]+[A-Za-z0-9-_]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		/// <summary>
		/// Parses the status text to HTML.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>Twitter status text reformated to html form.</returns>
		private static string ParseStatusTextToHtml(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return input;
			}

			// parse urls
			foreach (Match match in ParseUrls.Matches(input))
			{
				input = input.Replace(match.Value,
				                      String.Format(CultureInfo.InvariantCulture,
				                                    "<a href=\"{0}\" target=\"_blank\">{0}</a>",
				                                    match.Value));
			}

			// parse mentions
			foreach (Match match in ParseMentions.Matches(input))
			{
				if (match.Groups.Count != 3)
				{
					continue;
				}

				var screenName = match.Groups[2].Value;
				var mention = "@" + screenName;

				input = input.Replace(mention,
				                      String.Format(CultureInfo.InvariantCulture,
				                                    "<a href=\"http://twitter.com/{0}\" target=\"_blank\">{1}</a>",
				                                    screenName, mention));
			}

			// parse hash tags
			foreach (Match match in ParseHashtags.Matches(input))
			{
				var hashtag = Uri.EscapeDataString(match.Value);
				input = input.Replace(match.Value,
				                      String.Format(CultureInfo.InvariantCulture,
				                                    "<a href=\"http://search.twitter.com/search?q={0}\" target=\"_blank\">{1}</a>",
				                                    hashtag, match.Value));
			}

			return Sanitizer.GetSafeHtmlFragment(input);
		}
	}
}