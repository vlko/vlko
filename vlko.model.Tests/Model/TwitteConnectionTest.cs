using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twitterizer;
using vlko.core;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.model.Action;
using vlko.model.Action.ComplexHelpers.Twitter;

namespace vlko.model.Tests.Model
{
	[TestClass]
	public class TwitteConnectionTest
	{
		private const string ConsumerKey = "46KP0cc900kUX6jVPf8whQ";
		private const string ConsumerSecret = "AB208n0msrrEPMqzDq6yq0w31LeQpFsCOX1YBkNQw";
		private const string Token = "52357639-USalMlPWAUBU9zcCt2cIafaeKasKUfZZsExnpEHV3";
		private const string TokenSecret = "MkVuuscDzRdquoZwdvUlssPbd3mPcRPGbW74g";
		private const string UserToCheck = "vlkodotnet";

		[TestInitialize]
		public void Init()
		{
			IoC.InitializeWith(new WindsorContainer());
			ApplicationInit.InitializeRepositories();
		}

		[TestMethod]
		public void Test_authorize_url()
		{
			const string returnUrl = "http://localhost/returnurl";

			var action = RepositoryFactory.Action<ITwitterConnection>();

			var authorizeUrl = action.GetAuthorizeUrl(new ConsumerAppIdent {ConsumerKey = ConsumerKey, ConsumerSecret = ConsumerSecret}, returnUrl);

			Assert.IsNotNull(authorizeUrl);
			Assert.IsTrue(authorizeUrl.StartsWith("https://twitter.com/oauth/authorize?oauth_token="));
		}

		[TestMethod]
		[ExpectedException(typeof(TwitterizerException))]
		public void Test_authorize_url_fail()
		{
			const string returnUrl = "http://localhost/returnurl";

			var action = RepositoryFactory.Action<ITwitterConnection>();

			var authorizeUrl = action.GetAuthorizeUrl(new ConsumerAppIdent { ConsumerKey = ConsumerKey, ConsumerSecret = ConsumerSecret + "_to_fail"}, returnUrl);
		}

		// unable to reproduce this as user request is necessary
		//[TestMethod]
		//public void Test_get_o_auth_token()
		//{
		//    var action = RepositoryFactory.Action<ITwitterConnection>();

		//    var oAuthToken = action.GetOAuthToken(
		//            new ConsumerAppIdent { ConsumerKey = ConsumerKey, ConsumerSecret = ConsumerSecret }, 
		//            null, null);

		//    Assert.IsNotNull(oAuthToken);
		//}

		[TestMethod]
		public void Test_is_token_valid()
		{
			var action = RepositoryFactory.Action<ITwitterConnection>();

			var valid = action.IsTokenValid(new OAuthToken
			                    	{
			                    		ConsumerKey = ConsumerKey,
			                    		ConsumerSecret = ConsumerSecret,
			                    		Token = Token,
			                    		TokenSecret = TokenSecret
			                    	});

			Assert.IsTrue(valid);
		}

		[TestMethod]
		public void Test_is_token_not_valid()
		{
			var action = RepositoryFactory.Action<ITwitterConnection>();

			var valid = action.IsTokenValid(new OAuthToken
			{
				ConsumerKey = ConsumerKey,
				ConsumerSecret = ConsumerSecret,
				Token = Token,
				TokenSecret = TokenSecret + "something"
			});

			Assert.IsFalse(valid);
		}

		[TestMethod]
		public void Test_get_user_data()
		{
			var action = RepositoryFactory.Action<ITwitterConnection>();
			// with default 20 item
			var timeLine = action.GetStatusesForUser(
				new OAuthToken
					{
						ConsumerKey = ConsumerKey,
						ConsumerSecret = ConsumerSecret,
						Token = Token,
						TokenSecret = TokenSecret
					},
				UserToCheck,
				0, 25);

			Assert.IsNotNull(timeLine);
			Assert.AreEqual(25, timeLine.Length);
		}

		[TestMethod]
		public void Test_get_user_data_paging()
		{
			var action = RepositoryFactory.Action<ITwitterConnection>();
			// with default 20 item
			var timeLine = action.GetStatusesForUser(
				new OAuthToken
				{
					ConsumerKey = ConsumerKey,
					ConsumerSecret = ConsumerSecret,
					Token = Token,
					TokenSecret = TokenSecret
				},
				UserToCheck,
				20, 25);

			Assert.IsNotNull(timeLine);
			Assert.AreEqual(25, timeLine.Length);
		}
	}
}
