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
	public abstract class TimelinePagingBaseTest : LocalTest
	{
		private User _user;

        public TimelinePagingBaseTest(ITestProvider testProvider)
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
            }
		}

		public virtual void Cleanup()
		{
			TestProvider.TearDown();
		}

		public virtual void Get_half_half_items()
		{
            CreateTestData(5,5);
			using (RepositoryFactory.StartUnitOfWork())
			{
				var timeline = RepositoryFactory.Command<ITimeline>().GetAll(DateTime.Now.AddDays(1));
				Assert.AreEqual(10, timeline.Count());
				var data = timeline.ToPage(0, 4);
                AssertDataOrder(data, 4);
                data = timeline.ToPage(1, 4);
                AssertDataOrder(data, 4);
                data = timeline.ToPage(2, 4);
                AssertDataOrder(data, 2);
			}
		}

        public virtual void Get_only_first()
        {
            CreateTestData(10, 0);
            using (RepositoryFactory.StartUnitOfWork())
            {
                var timeline = RepositoryFactory.Command<ITimeline>().GetAll(DateTime.Now.AddDays(1));
                Assert.AreEqual(10, timeline.Count());
                var data = timeline.ToPage(0, 4);
                AssertDataOrder(data, 4, otherEndIndex:0);
                data = timeline.ToPage(1, 4);
                AssertDataOrder(data, 4, otherEndIndex: 0);
                data = timeline.ToPage(2, 4);
                AssertDataOrder(data, 2, otherEndIndex: 0);
            }
        }

        public virtual void Get_only_second()
        {
            CreateTestData(0, 10);
            using (RepositoryFactory.StartUnitOfWork())
            {
                var timeline = RepositoryFactory.Command<ITimeline>().GetAll(DateTime.Now.AddDays(1));
                Assert.AreEqual(10, timeline.Count());
                var data = timeline.ToPage(0, 4);
                AssertDataOrder(data, 4, firstEndIndex: 0);
                data = timeline.ToPage(1, 4);
                AssertDataOrder(data, 4, firstEndIndex: 0);
                data = timeline.ToPage(2, 4);
                AssertDataOrder(data, 2, firstEndIndex: 0);
            }
        }

        public virtual void Get_more_first()
        {
            CreateTestData(6, 4);
            using (RepositoryFactory.StartUnitOfWork())
            {
                var timeline = RepositoryFactory.Command<ITimeline>().GetAll(DateTime.Now.AddDays(1));
                Assert.AreEqual(10, timeline.Count());
                var data = timeline.ToPage(0, 4);
                AssertDataOrder(data, 4);
                data = timeline.ToPage(1, 4);
                AssertDataOrder(data, 4);
                data = timeline.ToPage(2, 4);
                AssertDataOrder(data, 2, otherEndIndex: 0);
            }
        }

        public virtual void Get_more_first_middle_page()
        {
            CreateTestData(7, 3);
            using (RepositoryFactory.StartUnitOfWork())
            {
                var timeline = RepositoryFactory.Command<ITimeline>().GetAll(DateTime.Now.AddDays(1));
                Assert.AreEqual(10, timeline.Count());
                var data = timeline.ToPage(0, 4);
                AssertDataOrder(data, 4);
                data = timeline.ToPage(1, 4);
                AssertDataOrder(data, 4, otherEndIndex: 1);
                data = timeline.ToPage(2, 4);
                AssertDataOrder(data, 2, otherEndIndex: 0);
            }
        }

        public virtual void Get_more_second()
        {
            CreateTestData(4, 6);
            using (RepositoryFactory.StartUnitOfWork())
            {
                var timeline = RepositoryFactory.Command<ITimeline>().GetAll(DateTime.Now.AddDays(1));
                Assert.AreEqual(10, timeline.Count());
                var data = timeline.ToPage(0, 4);
                AssertDataOrder(data, 4);
                data = timeline.ToPage(1, 4);
                AssertDataOrder(data, 4);
                data = timeline.ToPage(2, 4);
                AssertDataOrder(data, 2, firstEndIndex: 0);
            }
        }

        public virtual void Get_more_second_middle_page()
        {
            CreateTestData(3, 7);
            using (RepositoryFactory.StartUnitOfWork())
            {
                var timeline = RepositoryFactory.Command<ITimeline>().GetAll(DateTime.Now.AddDays(1));
                Assert.AreEqual(10, timeline.Count());
                var data = timeline.ToPage(0, 4);
                AssertDataOrder(data, 4);
                data = timeline.ToPage(1, 4);
                AssertDataOrder(data, 4, firstEndIndex: 1);
                data = timeline.ToPage(2, 4);
                AssertDataOrder(data, 2, firstEndIndex: 0);
            }
        }

        private void AssertDataOrder(object[] data, int expectedNumberOfItems, int? otherEndIndex = null, int? firstEndIndex = null)
        {
            Assert.AreEqual(expectedNumberOfItems, data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                bool otherItem = i%2 == 1;
                if (otherItem && otherEndIndex != null && otherEndIndex < i)
                {
                    otherItem = false;
                }
                if (!otherItem && firstEndIndex != null && firstEndIndex <= i)
                {
                    otherItem = true;
                }
                if (otherItem)
                {
                    Assert.IsInstanceOfType(data[i], typeof(StaticTextViewModel));
                }
                else
                {
                    Assert.IsInstanceOfType(data[i], typeof(TwitterStatus));
                }
            }
        }

		private void CreateTestData(int twitterItems, int otherItems)
		{
			using (var tran = RepositoryFactory.StartTransaction())
			{
				var startDate = DateTime.Now;
                for (int i = 0; i < otherItems; i++)
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
                }
			    startDate = DateTime.Now;
                for (int i = 0; i < twitterItems; i++)
				{
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
            TestProvider.WaitForIndexing();
		}
	}
}
