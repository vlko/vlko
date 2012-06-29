using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.Commands;
using vlko.BlogModule.Commands.CRUDModel;
using vlko.BlogModule.Commands.ViewModel;
using vlko.core.Commands;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.core.Roots;
using vlko.BlogModule.Roots;
using vlko.core.Testing;

namespace vlko.BlogModule.Tests.Model
{
	public abstract class TimelineBaseTest : LocalTest
	{
		private User _user;

		public TimelineBaseTest(ITestProvider testProvider)
			: base(testProvider)
		{
		}

		public virtual void Init()
		{
			TestProvider.SetUp();

			using (var tran = RepositoryFactory.StartTransaction())
			{
				RepositoryFactory.Command<IUserCommands>().CreateAdmin("user", "user@user.sk", "test");
				tran.Commit();
			}
			TestProvider.WaitForIndexing();
			using (RepositoryFactory.StartUnitOfWork())
			{
				_user = RepositoryFactory.Command<IUserCommands>().GetByName("user");
				CreateTestData();
			}
			TestProvider.WaitForIndexing();
		}

		public virtual void Cleanup()
		{
			TestProvider.TearDown();
		}

		public virtual void Get_timeline_all()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var timeline = RepositoryFactory.Command<ITimeline>().GetAll(DateTime.Now);
				Assert.AreEqual(203 - 2 /* two items are out of date range */, timeline.Count());
				var data = timeline.ToArray();
				Assert.AreNotEqual(timeline.Count(), data.Length);
                Assert.IsInstanceOfType(data[0], typeof(TwitterStatus));
                Assert.IsInstanceOfType(data[1], typeof(StaticTextViewModel));
                Assert.IsInstanceOfType(data[2], typeof(TwitterStatus));
                Assert.IsInstanceOfType(data[3], typeof(StaticTextViewModel));
                Assert.IsInstanceOfType(data[4], typeof(TwitterStatus));
				Assert.IsInstanceOfType(data[5], typeof(RssItemViewModelWithId));
			}
		}

		public virtual void Get_timeline_page()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var timeline = RepositoryFactory.Command<ITimeline>().GetAll(DateTime.Now);
				Assert.AreEqual(203 - 2 /* two items are out of date range */, timeline.Count());
				var data = timeline.ToPage(0, 20);
				Assert.AreEqual(20, data.Length);
                Assert.IsInstanceOfType(timeline.ToPage(0, 2)[0], typeof(TwitterStatus));
                Assert.IsInstanceOfType(timeline.ToPage(1, 2)[1], typeof(StaticTextViewModel));
				Assert.IsInstanceOfType(timeline.ToPage(2, 2)[1], typeof (RssItemViewModel));
			}
		}

		public virtual void Get_hierarchical_content()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var feeds = RepositoryFactory.Command<IRssFeedCommands>().GetAll();
				Assert.AreEqual(1, feeds.Count());

				var feedItems = RepositoryFactory.Command<IRssItemCommands>().GetAll();
				Assert.AreEqual(1, feedItems.Count());

				var staticTexts = RepositoryFactory.Command<IStaticTextData>().GetAll();
				Assert.AreEqual(101, staticTexts.Count());

				var twitterStatuses = RepositoryFactory.Command<ITwitterStatusCommands>().GetAll();
				Assert.AreEqual(101, staticTexts.Count());
			}
		}

		private void CreateTestData()
		{
			using (var tran = RepositoryFactory.StartTransaction())
			{
				var startDate = DateTime.Now;

				// add feed item
				var feed = RepositoryFactory.Command<IRssFeedCommands>().Create(new RssFeedCRUDModel()
				                                                             	{
				                                                             		Name = "feed",
				                                                             		Url = "url",
				                                                             	});
				var feedItem = RepositoryFactory.Command<IRssItemCommands>().Save(new RssItemCRUDModel()
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
				var home = RepositoryFactory.Command<IStaticTextCrud>().Create(
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

				RepositoryFactory.Command<ITwitterStatusCommands>().CreateStatus(
					new TwitterStatus()
						{
							TwitterId = 0,
							CreatedDate = startDate.AddDays(-2).AddMinutes(2),
							Modified = startDate.AddDays(-2).AddMinutes(2),
                            Description = "twitter status",
							User = _user.Name,
							AreCommentAllowed = false,
							RetweetUser = "Home",
						});

				startDate = startDate.AddDays(1);
				for (int i = 0; i < 100; i++)
				{
					var text = RepositoryFactory.Command<IStaticTextCrud>().Create(
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
					RepositoryFactory.Command<ITwitterStatusCommands>().CreateStatus(
						new TwitterStatus()
							{
								TwitterId = i + 1,
								CreatedDate = startDate.AddDays(-i),
                                Description = "twitter status",
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
