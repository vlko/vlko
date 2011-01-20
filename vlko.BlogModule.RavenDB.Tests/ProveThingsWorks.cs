using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client.Exceptions;
using vlko.BlogModule.RavenDB.Indexes;
using vlko.BlogModule.RavenDB.Indexes.ReduceModelView;
using vlko.BlogModule.RavenDB.Repository;
using vlko.BlogModule.Roots;
using vlko.core.InversionOfControl;
using vlko.core.Repository;

namespace vlko.BlogModule.RavenDB.Tests
{
	[TestClass]
	public class ProveThingsWorks : LocalClientTest
	{
		private RssFeed _testFeed;

		[TestInitialize]
		public void Init()
		{
			IoC.InitializeWith(new WindsorContainer());
			ApplicationInit.InitializeRepositories();
			base.SetUp();
			DBInit.RegisterDocumentStore(Store);
			DBInit.RegisterIndexes(Store);

			using (var tran = RepositoryFactory.StartTransaction())
			{
				_testFeed = new RssFeed
								{
									Id = Guid.NewGuid(),
									Name = "feed1",
									Url = "some_test_url1",
									AuthorRegex = "author_regex",
									DisplayFullContent = true,
									GetDirectContent = false,
									ContentParseRegex = "content_parser"
								};
				_testFeed.RssItems = new List<RssItem>
				                 	{
				                 		new RssItem
				                 			{
				                 				Id = Guid.NewGuid(),
				                 				RssFeed = _testFeed,
				                                Url = "feed_url1",
				                                Author = "unknown",
				                                Description = "Short_description",
				                                Text = "some_text",
				                                CreatedDate = DateTime.Now,
				                                Modified = DateTime.Now,
				                                PublishDate = DateTime.Now
				                            }
				                    };

				SessionFactory<RssFeed>.Store(_testFeed);
				foreach (var rssItem in _testFeed.RssItems)
				{
					SessionFactory<RssItem>.Store(rssItem);
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
		[ExpectedException(typeof(NonUniqueObjectException))]
		public void Same_id_stored_to_different_class()
		{
			Guid id = Guid.NewGuid();
			var classOne = new RssFeed
							{
								Id = id,
								Name = "test_feed",
								Url = "some_test_url1",
								AuthorRegex = "author_regex",
								DisplayFullContent = true,
								GetDirectContent = false,
								ContentParseRegex = "content_parser"
							};
			var classTwo = new RssItem
							{
								Id = id,
								Url = "test_url",
								Author = "unknown",
								Description = "Short_description",
								Text = "some_text",
								CreatedDate = DateTime.Now,
								Modified = DateTime.Now,
								PublishDate = DateTime.Now
							};

			using (var tran = RepositoryFactory.StartTransaction())
			{
				SessionFactory<RssFeed>.Store(classOne);

				// here it should fail !!!
				SessionFactory<RssItem>.Store(classTwo);
				tran.Commit();
			}
		}

		[TestMethod]
		public void Proxy_lazy_loading_of_reference_types()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				// get rssItem with proxy on RssItem.RssFeed
				var rssItem = SessionFactory<RssItem>.Load(_testFeed.RssItems[0].Id);

				// check if loaded ok
				Assert.IsNotNull(rssItem);
				Assert.AreEqual(_testFeed.RssItems[0].Id, rssItem.Id);
				Assert.AreEqual(_testFeed.RssItems[0].Url, rssItem.Url);

				// try to access id without request to server
				var startServerCalls = SessionFactory.Current.Advanced.NumberOfRequests;
				var rssFeedId = rssItem.RssFeed.Id;
				Assert.AreEqual(_testFeed.Id, rssFeedId);
				Assert.AreEqual(startServerCalls, SessionFactory.Current.Advanced.NumberOfRequests);

				// accessing other property should load data
				var rssFeedUrl = rssItem.RssFeed.Url;
				Assert.AreEqual(_testFeed.Url, rssFeedUrl);
				Assert.AreEqual(startServerCalls + 1, SessionFactory.Current.Advanced.NumberOfRequests);

				// every next access should not load data anymore
				var rssFeedName = rssItem.RssFeed.Name;
				Assert.AreEqual(_testFeed.Name, rssFeedName);
				Assert.AreEqual(startServerCalls + 1, SessionFactory.Current.Advanced.NumberOfRequests);
			}
		}

		[TestMethod]
		public void Save_class_with_proxy_reference()
		{
			const string newTitle = "new_title_change";
			using (var tran = RepositoryFactory.StartTransaction())
			{
				var rssItem = SessionFactory<RssItem>.Load(_testFeed.RssItems[0].Id);
				rssItem.Title = newTitle;
				SessionFactory<RssItem>.Store(rssItem);
				tran.Commit();
			}
			using (RepositoryFactory.StartUnitOfWork())
			{
				var rssItem = SessionFactory<RssItem>.Load(_testFeed.RssItems[0].Id);
				Assert.AreEqual(newTitle, rssItem.Title);
				Assert.AreEqual(_testFeed.Name, rssItem.RssFeed.Name);
			}
		}

		[TestMethod]
		public void Index_updated_up_and_down()
		{
			var itemToDeleteId = Guid.NewGuid();
			using (var tran = RepositoryFactory.StartTransaction())
			{
				SessionFactory<RssItem>.Store(new RssItem
				                              	{
													Id = itemToDeleteId,
													RssFeed = _testFeed,
				                              		Url = "new_item1",
				                              		Author = "unknown",
				                              		Description = "Short_description",
				                              		Text = "some_text",
				                              		CreatedDate = DateTime.Now,
				                              		Modified = DateTime.Now,
				                              		PublishDate = DateTime.Now
				                              	});
				SessionFactory<RssItem>.Store(new RssItem
				                              	{
				                              		Id = Guid.NewGuid(),
													RssFeed = _testFeed,
				                              		Url = "new_item2",
				                              		Author = "unknown",
				                              		Description = "Short_description",
				                              		Text = "some_text",
				                              		CreatedDate = DateTime.Now,
				                              		Modified = DateTime.Now,
				                              		PublishDate = DateTime.Now
				                              	});
				tran.Commit();
			}

			WaitForIndexing();

			using (RepositoryFactory.StartUnitOfWork())
			{
				var feedCount = SessionFactory<RssItem>.IndexQuery<RssFeedsWithItemsCount, RssFeedCount>().Where(item => item.FeedId == _testFeed.Id).FirstOrDefault();
				Assert.IsNotNull(feedCount);
				Assert.AreEqual(3, feedCount.Count);
			}

			using (var tran = RepositoryFactory.StartTransaction())
			{
				var itemToDelete = SessionFactory<RssItem>.Load(itemToDeleteId);
				SessionFactory<RssItem>.Delete(itemToDelete);
				tran.Commit();
			}
			WaitForIndexing();

			using (RepositoryFactory.StartUnitOfWork())
			{
				var feedCount = SessionFactory<RssItem>.IndexQuery<RssFeedsWithItemsCount, RssFeedCount>().Where(item => item.FeedId == _testFeed.Id).FirstOrDefault();
				Assert.IsNotNull(feedCount);
				Assert.AreEqual(2, feedCount.Count);
			}
		}
	}
}
