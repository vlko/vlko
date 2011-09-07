using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Twitterizer;
using vlko.BlogModule.Commands;
using vlko.BlogModule.Commands.ComplexHelpers.Twitter;
using vlko.BlogModule.Implementation.OtherTech.Commands;
using vlko.core.InversionOfControl;
using vlko.core.Repository;

namespace vlko.BlogModule.Tests.Model
{
	[TestClass]
	public class TwitteConnectionTest
	{
		private const string ConsumerKey = "46KP0cc900kUX6jVPf8whQ";
		private const string ConsumerSecret = "AB208n0msrrEPMqzDq6yq0w31LeQpFsCOX1YBkNQw";
		private const string Token = "put-here-valid-token";
		private const string TokenSecret = "put-here-valid-token-secret";
		private const string UserToCheck = "vlkodotnet";

		[TestInitialize]
		public void Init()
		{
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule"));
			IoC.AddRerouting<IRepositoryFactoryResolver>(() => new RepositoryFactoryResolver());
		}

		[TestMethod]
		public void Test_authorize_url()
		{
			const string returnUrl = "http://localhost/returnurl";

			var command = RepositoryFactory.Command<ITwitterConnection>();

			var authorizeUrl = command.GetAuthorizeUrl(new ConsumerAppIdent {ConsumerKey = ConsumerKey, ConsumerSecret = ConsumerSecret}, returnUrl);

			Assert.IsNotNull(authorizeUrl);
			Assert.IsTrue(authorizeUrl.StartsWith("https://twitter.com/oauth/authorize?oauth_token="));
		}

		[TestMethod]
		[ExpectedException(typeof(TwitterizerException))]
		public void Test_authorize_url_fail()
		{
			const string returnUrl = "http://localhost/returnurl";

			var command = RepositoryFactory.Command<ITwitterConnection>();

			var authorizeUrl = command.GetAuthorizeUrl(new ConsumerAppIdent { ConsumerKey = ConsumerKey, ConsumerSecret = ConsumerSecret + "_to_fail"}, returnUrl);
		}

		[TestMethod]
		public void Test_get_o_auth_token()
		{
			var command = RepositoryFactory.Command<ITwitterConnection>();

			var oAuthToken = command.GetOAuthToken(
					new ConsumerAppIdent { ConsumerKey = ConsumerKey, ConsumerSecret = ConsumerSecret },
					null, null);

			Assert.IsNotNull(oAuthToken);
		}

		[TestMethod]
		public void Test_is_token_valid()
		{
			var command = RepositoryFactory.Command<ITwitterConnection>();

			var valid = command.IsTokenValid(new OAuthToken
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
			var command = RepositoryFactory.Command<ITwitterConnection>();

			var valid = command.IsTokenValid(new OAuthToken
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
			var command = RepositoryFactory.Command<ITwitterConnection>();
			// with default 20 item
			var timeLine = command.GetStatusesForUser(
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
			var command = RepositoryFactory.Command<ITwitterConnection>();
			// with default 20 item
			var timeLine = command.GetStatusesForUser(
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

		public class RepositoryFactoryResolver : IRepositoryFactoryResolver
		{
			public IRepository<T> GetRepository<T>() where T : class
			{
				throw new System.NotImplementedException();
			}

			public T ResolveCommand<T>() where T : class, ICommandGroup
			{
				if (typeof(T) == typeof(ITwitterConnection))
				{
					return new TwitterConnection() as T;
				}
					throw new System.NotImplementedException();
			}

			public IUnitOfWork GetUnitOfWork()
			{
				throw new System.NotImplementedException();
			}

			public ITransaction GetTransaction()
			{
				throw new System.NotImplementedException();
			}
		}
	}
}
