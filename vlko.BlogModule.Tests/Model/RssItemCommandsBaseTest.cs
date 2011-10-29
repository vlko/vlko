using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.Commands;
using vlko.BlogModule.Commands.CRUDModel;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.BlogModule.Roots;
using vlko.core.Testing;

namespace vlko.BlogModule.Tests.Model
{
	public abstract class RssItemCommandsBaseTest : LocalTest
	{
		private RssItem[] _rssItems;

		private RssFeed _feed;


		public RssItemCommandsBaseTest(ITestProvider testProvider) : base(testProvider)
		{
		}

		public virtual void Init()
		{
			TestProvider.SetUp();

			using (var tran = RepositoryFactory.StartTransaction())
			{
				_feed = new RssFeed
				        	{
								Id = Guid.NewGuid(),
				        		Name = "feed",
				        		Url = "feed_url"
				        	};
				TestProvider.Create(_feed);
				_rssItems = new[]
				            	{
				            		new RssItem
				            			{
											Id = Guid.NewGuid(),
				            				FeedItemId = "1",
											Title = "title1",
				            				Text = "Text1",
											Description = "Description1",
				            				Author = "twit_user1",
											Url = "url1",
				            				Hidden = false,
				            				CreatedDate = new DateTime(2010, 10, 10),
				            				Modified = new DateTime(2010, 10, 10),
				            				PublishDate = new DateTime(2010, 10, 10),
				            				CreatedBy = null,
				            				AreCommentAllowed = false,
											RssFeed = _feed
				            			},
				            		new RssItem
				            			{
											Id = Guid.NewGuid(),
				            				FeedItemId = "2",
											Title = "title2",
				            				Text = "Text2",
											Description = "Description2",
				            				Author = "twit_user2",
											Url = "url2",
				            				Hidden = false,
				            				CreatedDate = new DateTime(2010, 10, 12),
				            				Modified = new DateTime(2010, 10, 12),
				            				PublishDate = new DateTime(2010, 10, 12),
				            				CreatedBy = null,
				            				AreCommentAllowed = false,
											RssFeed = _feed
				            			},
									new RssItem
										{
											Id = Guid.NewGuid(),
				            				FeedItemId = "3",
											Title = "title3",
				            				Text = "Text3",
											Description = "Description3",
				            				Author = "twit_user3",
											Url = "url3",
				            				Hidden = true,
				            				CreatedDate = new DateTime(2010, 10, 13),
				            				Modified = new DateTime(2010, 10, 13),
				            				PublishDate = new DateTime(2010, 10, 13),
				            				CreatedBy = null,
				            				AreCommentAllowed = false,
											RssFeed = _feed
				            			}
				            	};
				foreach (var rssItem in _rssItems)
				{
					TestProvider.Create(rssItem);
				}
				tran.Commit();
			}
			TestProvider.WaitForIndexing();
		}

		public virtual void Cleanup()
		{
			TestProvider.TearDown();
		}

		public virtual void Test_save()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var item = new RssItemCRUDModel()
				           	{
								FeedItemId = "new",
								Title = "title_new",
								Text = "Text_new",
								Description = "DescriptionNew",
								Author = "twit_usernew",
								Url = "url_new",
								FeedId = _feed.Id,
								Published = DateTime.Now
				           	};

				var command = RepositoryFactory.Command<IRssItemCommands>();

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = command.Save(item);
					tran.Commit();
				}

				TestProvider.WaitForIndexing();

				var storedItem = TestProvider.AsQueryable<RssItem>().First(rssItem => rssItem.FeedItemId == item.FeedItemId);

				Assert.AreEqual(item.FeedItemId, storedItem.FeedItemId);
				Assert.AreEqual(item.FeedId, storedItem.RssFeed.Id);
				Assert.AreEqual(item.Title, storedItem.Title);
				Assert.AreEqual(item.Text, storedItem.Text);
				Assert.AreEqual(item.Description, storedItem.Description);
				Assert.AreEqual(item.Published, storedItem.CreatedDate);
				Assert.AreEqual(item.Published, storedItem.Modified);
				Assert.AreEqual(item.Published, storedItem.PublishDate);
				Assert.AreEqual(item.Author, storedItem.Author);
				Assert.AreEqual(item.Url, storedItem.Url);
				Assert.AreEqual(false, storedItem.Hidden);
				Assert.AreEqual(false, storedItem.AreCommentAllowed);

				item = new RssItemCRUDModel()
				{
					FeedItemId = "change",
					Title = "title_change",
					Text = "Text_change",
					Description = "Descriptionchange",
					Author = "twit_userchange",
					Url = "url_change",
					FeedId = _feed.Id,
					Published = DateTime.Now
				};

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = command.Save(item);
					tran.Commit();
				}

				TestProvider.WaitForIndexing();

				storedItem = TestProvider.AsQueryable<RssItem>().First(rssItem => rssItem.FeedItemId == item.FeedItemId);

				Assert.AreEqual(item.FeedItemId, storedItem.FeedItemId);
				Assert.AreEqual(item.FeedId, storedItem.RssFeed.Id);
				Assert.AreEqual(item.Title, storedItem.Title);
				Assert.AreEqual(item.Text, storedItem.Text);
				Assert.AreEqual(item.Description, storedItem.Description);
				Assert.AreEqual(item.Published, storedItem.CreatedDate);
				Assert.AreEqual(item.Published, storedItem.Modified);
				Assert.AreEqual(item.Published, storedItem.PublishDate);
				Assert.AreEqual(item.Author, storedItem.Author);
				Assert.AreEqual(item.Url, storedItem.Url);
				Assert.AreEqual(false, storedItem.Hidden);
				Assert.AreEqual(false, storedItem.AreCommentAllowed);
			}
			using (RepositoryFactory.StartUnitOfWork())
			{
				var storedItem = TestProvider.AsQueryable<RssItem>().First(rssItem => rssItem.FeedItemId == "change");
				var id = storedItem.RssFeed.Id;
			}
		}

		public virtual void Test_delete()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var item = new RssItemCRUDModel()
				           	{
				           		FeedItemId = "new",
				           		Title = "title_new",
				           		Text = "Text_new",
				           		Description = "DescriptionNew",
				           		Author = "twit_usernew",
				           		Url = "url_new",
				           		FeedId = _feed.Id,
								Published = DateTime.Now
				           	};

				var command = RepositoryFactory.Command<IRssItemCommands>();

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = command.Save(item);
					tran.Commit();
				}

				TestProvider.WaitForIndexing();

				Assert.IsNotNull(TestProvider.AsQueryable<RssItem>().FirstOrDefault(rssItem => rssItem.FeedItemId == item.FeedItemId));

				using (var tran = RepositoryFactory.StartTransaction())
				{
					command.Delete(item);
					tran.Commit();
				}

				TestProvider.WaitForIndexing();

				var items = command.GetAll().ToArray();
				Assert.AreEqual(2, items.Count());
				Assert.IsFalse(items.Any(rssItem => rssItem.FeedItemId == item.FeedItemId));
			}
		}

		public virtual void Get_by_ids()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var command = RepositoryFactory.Command<IRssItemCommands>();
				var result = command.GetByIds(new[] { _rssItems[0].FeedItemId, _rssItems[1].FeedItemId })
								.ToArray();

				Assert.AreEqual(2, result.Length);
				Assert.AreEqual(_rssItems[0].FeedItemId, result.First(item => item.FeedItemId == _rssItems[0].FeedItemId).FeedItemId);
				Assert.AreEqual(_rssItems[1].FeedItemId, result.First(item => item.FeedItemId == _rssItems[1].FeedItemId).FeedItemId);
			}
		}

		public virtual void Get_by_feed_ids()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var command = RepositoryFactory.Command<IRssItemCommands>();
				var result = command.GetByFeedIds(new[] { _rssItems[0].FeedItemId, _rssItems[1].FeedItemId })
								.ToArray();

				Assert.AreEqual(2, result.Length);
				Assert.AreEqual(_rssItems[0].FeedItemId, result.First(item => item.FeedItemId == _rssItems[0].FeedItemId).FeedItemId);
				Assert.AreEqual(_rssItems[1].FeedItemId, result.First(item => item.FeedItemId == _rssItems[1].FeedItemId).FeedItemId);
			}
		}

		public virtual void Get_all()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var command = RepositoryFactory.Command<IRssItemCommands>();
				var result = command.GetAll().OrderBy(item => item.Published).ToArray();

				Assert.AreEqual(2, result.Length);
				Assert.AreEqual(_rssItems[0].FeedItemId, result[0].FeedItemId);
				Assert.AreEqual(_rssItems[1].FeedItemId, result[1].FeedItemId);
			}
		}
	}


}
