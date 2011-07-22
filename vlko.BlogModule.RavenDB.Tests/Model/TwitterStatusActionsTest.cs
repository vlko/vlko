using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.RavenDB.Repository;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.BlogModule.Action;
using vlko.BlogModule.Roots;
using vlko.core.Roots;

namespace vlko.BlogModule.RavenDB.Tests.Model
{
	[TestClass]
	public class TwitterStatusActionsTest : LocalClientTest
	{
		private TwitterStatus[] _statuses;

		private User _user;

		[TestInitialize]
		public void Init()
		{
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule.RavenDB"));
			base.SetUp();

			using (var tran = RepositoryFactory.StartTransaction())
			{
				_statuses = new[]
				            	{
				            		new TwitterStatus
				            			{
											Id = Guid.NewGuid(),
				            				TwitterId = 1,
				            				Text = "Status1",
				            				User = "twit_user1",
				            				Hidden = false,
				            				CreatedDate = new DateTime(2010, 10, 10),
				            				Modified = new DateTime(2010, 10, 10),
				            				PublishDate = new DateTime(2010, 10, 10),
				            				Reply = false,
				            				RetweetUser = null,
				            				CreatedBy = null,
				            				AreCommentAllowed = false
				            			},
				            		new TwitterStatus
				            			{
											Id = Guid.NewGuid(),
				            				TwitterId = 2,
				            				Text = "Status2",
				            				User = "twit_user1",
				            				Hidden = false,
				            				CreatedDate = new DateTime(2010, 10, 11),
				            				Modified = new DateTime(2010, 10, 11),
				            				PublishDate = new DateTime(2010, 10, 11),
				            				Reply = false,
				            				RetweetUser = null,
				            				CreatedBy = null,
				            				AreCommentAllowed = false
				            			},
				            		new TwitterStatus
				            			{
											Id = Guid.NewGuid(),
				            				TwitterId = 3,
				            				Text = "Status3",
				            				User = "twit_user1",
				            				Hidden = false,
				            				CreatedDate = new DateTime(2010, 10, 11).AddMinutes(1),
				            				Modified = new DateTime(2010, 10, 11),
				            				PublishDate = new DateTime(2010, 10, 11),
				            				Reply = false,
				            				RetweetUser = null,
				            				CreatedBy = null,
				            				AreCommentAllowed = false
				            			}
				            	};
				foreach (var twitterStatus in _statuses)
				{
					SessionFactory<TwitterStatus>.Store(twitterStatus);
				}
				tran.Commit();
			}
			WaitForIndexing();
		}

		[TestCleanup]
		public void Cleanup()
		{
			TearDown();
		}

		[TestMethod]
		public void Test_create()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var item = new TwitterStatus()
				           	{
				           		TwitterId = 4,
				           		Text = "Status4",
				           		User = "twit_user2",
				           		Hidden = false,
				           		CreatedDate = new DateTime(2010, 10, 11),
				           		Modified = new DateTime(2010, 10, 11),
				           		PublishDate = new DateTime(2010, 10, 11),
				           		Reply = false,
				           		RetweetUser = null,
				           		CreatedBy = null,
				           		AreCommentAllowed = false
				           	};

				var action = RepositoryFactory.Action<ITwitterStatusAction>();

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = action.CreateStatus(item);
					tran.Commit();
				}

				var storedItem = SessionFactory<TwitterStatus>.Load(item.Id);

				Assert.AreEqual(item.Id, storedItem.Id);
				Assert.AreEqual(item.TwitterId, storedItem.TwitterId);
				Assert.AreEqual(item.Text, storedItem.Text);
				Assert.AreEqual(item.Hidden, storedItem.Hidden);
				Assert.AreEqual(item.CreatedDate, storedItem.CreatedDate);
				Assert.AreEqual(item.Modified, storedItem.Modified);
				Assert.AreEqual(item.PublishDate, storedItem.PublishDate);
				Assert.AreEqual(item.Reply, storedItem.Reply);
				Assert.AreEqual(item.RetweetUser, storedItem.RetweetUser);
			}
		}

		[TestMethod]
		public void Get_by_ids()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var action = RepositoryFactory.Action<ITwitterStatusAction>();
				var result = action.GetByIds(new[] {_statuses[0].Id, _statuses[1].Id})
								.ToArray();
				
				Assert.AreEqual(2, result.Length);
				Assert.AreEqual(_statuses[0].Id, result.First(item => item.Id == _statuses[0].Id).Id);
				Assert.AreEqual(_statuses[1].Id, result.First(item => item.Id == _statuses[1].Id).Id);

			}
		}

		[TestMethod]
		public void Get_by_twitter_ids()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var action = RepositoryFactory.Action<ITwitterStatusAction>();
				var result = action.GetByTwitterIds(new[] { _statuses[0].TwitterId, _statuses[1].TwitterId })
								.ToArray();

				Assert.AreEqual(2, result.Length);
				Assert.AreEqual(_statuses[0].Id, result.First(item => item.Id == _statuses[0].Id).Id);
				Assert.AreEqual(_statuses[1].Id, result.First(item => item.Id == _statuses[1].Id).Id);

			}
		}

		[TestMethod]
		public void Get_all()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var action = RepositoryFactory.Action<ITwitterStatusAction>();
				var result = action.GetAll().OrderBy(item => item.CreatedDate).ToArray();

				Assert.AreEqual(3, result.Length);
				Assert.AreEqual(_statuses[0].Id, result[0].Id);
				Assert.AreEqual(_statuses[1].Id, result[1].Id);
				Assert.AreEqual(_statuses[2].Id, result[2].Id);
			}
		}
	}

	
}
