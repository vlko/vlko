using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.NH;
using vlko.core.InversionOfControl;
using vlko.core.NH.Testing;
using vlko.core.Repository;
using vlko.core.Roots;
using vlko.BlogModule.Action;
using vlko.BlogModule.Action.CRUDModel;
using vlko.BlogModule.Action.ViewModel;
using vlko.BlogModule.NH.Action;
using vlko.BlogModule.Roots;
using vlko.core.Action;

namespace vlko.BlogModule.Tests.Model
{
	[TestClass]
	public class TimelineTest : InMemoryTest
	{
		private User _user;
		[TestInitialize]
		public void Init()
		{
			//var doc = new XmlDocument();
			//doc.Load("log4net.config");
			//log4net.Config.XmlConfigurator.Configure(doc.DocumentElement);

			IoC.AddCatalogAssembly(Assembly.Load("vlko.Core.NH"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule.NH"));
			base.SetUp();

			using (var tran = RepositoryFactory.StartTransaction())
			{
				RepositoryFactory.Action<IUserAction>().CreateAdmin("user", "user@user.sk", "test");
				tran.Commit();
			}
			using (RepositoryFactory.StartUnitOfWork())
			{
				_user = RepositoryFactory.Action<IUserAction>().GetByName("user");
				CreateTestData();
			}
		}

		[TestCleanup]
		public void Cleanup()
		{
			TearDown();
		}

		[TestMethod]
		public void Get_timeline_all()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var timeline = RepositoryFactory.Action<ITimeline>().GetAll(DateTime.Now);
				Assert.AreEqual(203 - 2 /* two items are out of date range */, timeline.Count());
				var data = timeline.ToArray();
				Assert.AreEqual(TimelineResult.MaximumResults, data.Length);
				Assert.IsInstanceOfType(data[4], typeof(RssItemViewModel));
				Assert.IsInstanceOfType(data[3], typeof(StaticTextViewModel));
				Assert.IsInstanceOfType(data[0], typeof(TwitterStatus));
			}
		}

		[TestMethod]
		public void Get_timeline_page()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var timeline = RepositoryFactory.Action<ITimeline>().GetAll(DateTime.Now);
				Assert.AreEqual(203 - 2 /* two items are out of date range */, timeline.Count());
				var data = timeline.ToPage(0, 20);
				Assert.AreEqual(20, data.Length);
				Assert.IsInstanceOfType(timeline.ToPage(4, 1)[0], typeof (RssItemViewModel));
				Assert.IsInstanceOfType(timeline.ToPage(1, 1)[0], typeof (StaticTextViewModel));
				Assert.IsInstanceOfType(timeline.ToPage(0, 1)[0], typeof (TwitterStatus));
			}
		}

		private void CreateTestData()
		{
			using (var tran = RepositoryFactory.StartTransaction())
			{
				var startDate = DateTime.Now;

				// add feed item
				var feed = RepositoryFactory.Action<IRssFeedAction>().Create(new RssFeedCRUDModel()
				                                                             	{
				                                                             		Name = "feed",
				                                                             		Url = "url",
				                                                             	});
				var feedItem = RepositoryFactory.Action<IRssItemAction>().Save(new RssItemCRUDModel()
				                                                               	{
				                                                               		FeedItemId = "new",
				                                                               		Title = "home",
				                                                               		Text = "text",
				                                                               		Description = "description",
				                                                               		Author = "user",
				                                                               		Url = "url",
				                                                               		Published = startDate.AddDays(-2).AddMinutes(4),
				                                                               		FeedId = feed.Id
				                                                               	});


				// add to index some static text
				var home = RepositoryFactory.Action<IStaticTextCrud>().Create(
					new StaticTextCRUDModel
						{
							AllowComments = false,
							Creator = _user,
							Title = "Home",
							FriendlyUrl = "Home",
							ChangeDate = startDate.AddDays(-2).AddMinutes(3),
							PublishDate = startDate.AddDays(-2).AddMinutes(3),
							Text = "Welcome to vlko",
							Description = "Welcome to vlko",
						});

				RepositoryFactory.Action<ITwitterStatusAction>().CreateStatus(
					new TwitterStatus()
						{
							TwitterId = 0,
							CreatedDate = startDate.AddDays(-2).AddMinutes(2),
							Modified = startDate.AddDays(-2).AddMinutes(2),
							Text = "twitter status",
							User = _user.Name,
							AreCommentAllowed = false,
							RetweetUser = "Home",
						});

				startDate = startDate.AddDays(1);
				for (int i = 0; i < 100; i++)
				{
					var text = RepositoryFactory.Action<IStaticTextCrud>().Create(
						new StaticTextCRUDModel
							{
								AllowComments = true,
								Creator = _user,
								Title = "StaticPage" + i,
								FriendlyUrl = "StaticPage" + i,
								ChangeDate = startDate.AddDays(-i).AddMinutes(-1),
								PublishDate = startDate.AddDays(-i).AddMinutes(-1),
								Text = "Static page" + i,
								Description = "Static page" + i
							});

					// add some twitter status
					RepositoryFactory.Action<ITwitterStatusAction>().CreateStatus(
						new TwitterStatus()
							{
								TwitterId = i + 1,
								CreatedDate = startDate.AddDays(-i),
								Text = "twitter status",
								User = _user.Name,
								AreCommentAllowed = false,
								Hidden = false,
								Modified = startDate.AddDays(-i),
								Reply = false
							});
				}
				tran.Commit();
			}
		}
	}
}
