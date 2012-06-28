using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.Commands;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.BlogModule.Roots;
using vlko.core.Roots;
using vlko.core.Testing;

namespace vlko.BlogModule.Tests.Model
{
	public abstract class TwitterStatusCommandsBaseTest : LocalTest
	{
		private TwitterStatus[] _statuses;

		private User _user;

		public TwitterStatusCommandsBaseTest(ITestProvider testProvider) : base(testProvider)
		{
		}

		public virtual void Init()
		{
			TestProvider.SetUp();

			using (var tran = RepositoryFactory.StartTransaction())
			{
				_statuses = new[]
				            	{
				            		new TwitterStatus
				            			{
											Id = Guid.NewGuid(),
				            				TwitterId = 1,
				            				Description = "Status1",
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
				            				Description = "Status2",
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
				            				Description = "Status3",
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
					TestProvider.Create(twitterStatus);
				}
				tran.Commit();
			}
			TestProvider.WaitForIndexing();
		}

		public virtual void Cleanup()
		{
			TestProvider.TearDown();
		}

		public virtual void Test_create()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var item = new TwitterStatus()
				           	{
				           		TwitterId = 4,
                                Description = "Status4",
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

				var command = RepositoryFactory.Command<ITwitterStatusCommands>();

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = command.CreateStatus(item);
					tran.Commit();
				}

				var storedItem = TestProvider.GetById<TwitterStatus>(item.Id);

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

		public virtual void Get_by_ids()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var command = RepositoryFactory.Command<ITwitterStatusCommands>();
				var result = command.GetByIds(new[] {_statuses[0].Id, _statuses[1].Id})
								.ToArray();
				
				Assert.AreEqual(2, result.Length);
				Assert.AreEqual(_statuses[0].Id, result.First(item => item.Id == _statuses[0].Id).Id);
				Assert.AreEqual(_statuses[1].Id, result.First(item => item.Id == _statuses[1].Id).Id);

			}
		}

		public virtual void Get_by_twitter_ids()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var command = RepositoryFactory.Command<ITwitterStatusCommands>();
				var result = command.GetByTwitterIds(new[] { _statuses[0].TwitterId, _statuses[1].TwitterId })
								.ToArray();

				Assert.AreEqual(2, result.Length);
				Assert.AreEqual(_statuses[0].Id, result.First(item => item.Id == _statuses[0].Id).Id);
				Assert.AreEqual(_statuses[1].Id, result.First(item => item.Id == _statuses[1].Id).Id);

			}
		}

		public virtual void Get_all()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var command = RepositoryFactory.Command<ITwitterStatusCommands>();
				var result = command.GetAll().OrderBy(item => item.CreatedDate).ToArray();

				Assert.AreEqual(3, result.Length);
				Assert.AreEqual(_statuses[0].Id, result[0].Id);
				Assert.AreEqual(_statuses[1].Id, result[1].Id);
				Assert.AreEqual(_statuses[2].Id, result[2].Id);
			}
		}
	}

	
}
