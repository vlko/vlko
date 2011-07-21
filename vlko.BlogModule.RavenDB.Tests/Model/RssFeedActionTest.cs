using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.BlogModule.RavenDB.Repository;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.BlogModule.Action;
using vlko.BlogModule.Action.CRUDModel;
using vlko.BlogModule.Roots;

namespace vlko.BlogModule.RavenDB.Tests.Model
{
	[TestClass]
	public class RssFeedActionTest : LocalClientTest
	{
		private RssFeed[] _testData;

		[TestInitialize]
		public void Init()
		{
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule"));
			IoC.AddCatalogAssembly(Assembly.Load("vlko.BlogModule.RavenDB"));
			base.SetUp();

			using (var tran = RepositoryFactory.StartTransaction())
			{
				var feed1 = new RssFeed
				            	{
									Id = Guid.NewGuid(),
				            		Name = "feed1",
				            		Url = "some_test_url1",
				            		AuthorRegex = "author_regex",
				            		DisplayFullContent = true,
				            		GetDirectContent = false,
				            		ContentParseRegex = "content_parser"
				            	};
				feed1.RssItems = new List<RssItem>
				                 	{
				                 		new RssItem
				                 			{
				                 				Id = Guid.NewGuid(),
				                 				RssFeed = feed1,
				                                Url = "feed_url1",
				                                Author = "unknown",
				                                Description = "Short_description",
				                                Text = "some_text",
				                                CreatedDate = DateTime.Now,
				                                Modified = DateTime.Now,
				                                PublishDate = DateTime.Now
				                            }
				                    };
				var feed2 = new RssFeed
				{
					Id = Guid.NewGuid(),
					Name = "feed2",
					Url = "some_test_url2",
					AuthorRegex = "author_regex",
					DisplayFullContent = true,
					GetDirectContent = false,
					ContentParseRegex = "content_parser"
				};

				var feed3 = new RssFeed
				{
					Id = Guid.NewGuid(),
					Name = "feed3",
					Url = "some_test_url3",
					AuthorRegex = "author_regex",
					DisplayFullContent = false,
					GetDirectContent = true,
					ContentParseRegex = "content_parser"
				};

				SessionFactory<RssFeed>.Store(feed1);
				SessionFactory<RssFeed>.Store(feed2);
				SessionFactory<RssFeed>.Store(feed3);
				foreach (var rssItem in feed1.RssItems)
				{
					SessionFactory<RssItem>.Store(rssItem);
				}
				_testData = new[] { feed1, feed2, feed3 };
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
		public void Test_find_by_primary_key()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var items = SessionFactory<RssFeed>.FindAll();

				var crudActions = RepositoryFactory.Action<IRssFeedAction>();

				// get first
				var first = crudActions.FindByPk(_testData[0].Id);
				Assert.AreEqual(_testData[0].Id, first.Id);
				Assert.AreEqual(_testData[0].Name, first.Name);
				Assert.AreEqual(_testData[0].Url, first.Url);
				Assert.AreEqual(_testData[0].AuthorRegex, first.AuthorRegex);
				Assert.AreEqual(_testData[0].ContentParseRegex, first.ContentParseRegex);
				Assert.AreEqual(_testData[0].GetDirectContent, first.GetDirectContent);
				Assert.AreEqual(_testData[0].DisplayFullContent, first.DisplayFullContent);

				// get second
				var second = crudActions.FindByPk(_testData[1].Id);
				Assert.AreEqual(_testData[1].Id, second.Id);
				Assert.AreEqual(_testData[1].Name, second.Name);
				Assert.AreEqual(_testData[1].Url, second.Url);
				Assert.AreEqual(_testData[1].AuthorRegex, second.AuthorRegex);
				Assert.AreEqual(_testData[1].ContentParseRegex, second.ContentParseRegex);
				Assert.AreEqual(_testData[1].GetDirectContent, second.GetDirectContent);
				Assert.AreEqual(_testData[1].DisplayFullContent, second.DisplayFullContent);

				// get third
				var third = crudActions.FindByPk(_testData[2].Id);
				Assert.AreEqual(_testData[2].Id, third.Id);
				Assert.AreEqual(_testData[2].Name, third.Name);
				Assert.AreEqual(_testData[2].Url, third.Url);
				Assert.AreEqual(_testData[2].AuthorRegex, third.AuthorRegex);
				Assert.AreEqual(_testData[2].ContentParseRegex, third.ContentParseRegex);
				Assert.AreEqual(_testData[2].GetDirectContent, third.GetDirectContent);
				Assert.AreEqual(_testData[2].DisplayFullContent, third.DisplayFullContent);

			}
		}

		[TestMethod]
		public void Test_create()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var item = new RssFeedCRUDModel()
							   {
								   Name = "new_feed1",
								   Url = "new_feed_url",
								   AuthorRegex = "author_regex_new",
								   ContentParseRegex = "content_parse_regex",
								   GetDirectContent = true,
								   DisplayFullContent = false
							   };

				var crudActions = RepositoryFactory.Action<IRssFeedAction>();

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = crudActions.Create(item);
					tran.Commit();
				}
				WaitForIndexing();

				var storedItem = crudActions.FindByPk(item.Id);

				Assert.AreEqual(item.Id, storedItem.Id);
				Assert.AreEqual(item.Name, storedItem.Name);
				Assert.AreEqual(item.Url, storedItem.Url);
				Assert.AreEqual(item.AuthorRegex, storedItem.AuthorRegex);
				Assert.AreEqual(item.ContentParseRegex, storedItem.ContentParseRegex);
				Assert.AreEqual(item.GetDirectContent, storedItem.GetDirectContent);
				Assert.AreEqual(item.DisplayFullContent, storedItem.DisplayFullContent);
			}
		}

		[TestMethod]
		public void Test_update()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var item = new RssFeedCRUDModel()
				{
					Name = "new_feed1",
					Url = "new_feed_url",
					AuthorRegex = "author_regex_new",
					ContentParseRegex = "content_parse_regex",
					GetDirectContent = true,
					DisplayFullContent = false
				};

				var crudActions = RepositoryFactory.Action<IRssFeedAction>();

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = crudActions.Create(item);
					tran.Commit();
				}
				WaitForIndexing();

				// check created item
				var storedItem = crudActions.FindByPk(item.Id);

				Assert.AreEqual(item.Id, storedItem.Id);
				Assert.AreEqual(item.Name, storedItem.Name);
				Assert.AreEqual(item.Url, storedItem.Url);
				Assert.AreEqual(item.AuthorRegex, storedItem.AuthorRegex);
				Assert.AreEqual(item.ContentParseRegex, storedItem.ContentParseRegex);
				Assert.AreEqual(item.GetDirectContent, storedItem.GetDirectContent);
				Assert.AreEqual(item.DisplayFullContent, storedItem.DisplayFullContent);

				// change item
				item.Name = item.Name + "change";
				item.Url = item.Url + "change";
				item.AuthorRegex = item.AuthorRegex + "change";
				item.ContentParseRegex = item.ContentParseRegex + "change";
				item.GetDirectContent = !item.GetDirectContent;
				item.DisplayFullContent = !item.DisplayFullContent;

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = crudActions.Update(item);
					tran.Commit();
				}

				// check changed item
				storedItem = crudActions.FindByPk(item.Id);

				Assert.AreEqual(item.Id, storedItem.Id);
				Assert.AreEqual(item.Name, storedItem.Name);
				Assert.AreEqual(item.Url, storedItem.Url);
				Assert.AreEqual(item.AuthorRegex, storedItem.AuthorRegex);
				Assert.AreEqual(item.ContentParseRegex, storedItem.ContentParseRegex);
				Assert.AreEqual(item.GetDirectContent, storedItem.GetDirectContent);
				Assert.AreEqual(item.DisplayFullContent, storedItem.DisplayFullContent);
			}
		}

		[TestMethod]
		public void Test_delete()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var initialCount = SessionFactory<RssFeed>.Count();

				var item = new RssFeedCRUDModel()
				{
					Name = "new_feed1",
					Url = "new_feed_url",
					AuthorRegex = "author_regex_new",
					ContentParseRegex = "content_parse_regex",
					GetDirectContent = true,
					DisplayFullContent = false
				};

				var crudActions = RepositoryFactory.Action<IRssFeedAction>();

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = crudActions.Create(item);
					tran.Commit();
				}
				WaitForIndexing();

				Assert.AreEqual(initialCount + 1, SessionFactory<RssFeed>.Count());

				using (var tran = RepositoryFactory.StartTransaction())
				{
					crudActions.Delete(item);
					tran.Commit();
				}
				WaitForIndexing();

				Assert.AreEqual(initialCount, SessionFactory<RssFeed>.Count());
			}
		}

		[TestMethod]
		public void Data_get_all()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var dataActions = RepositoryFactory.Action<IRssFeedAction>();

				var dataItems = dataActions.GetAll().OrderBy(item => item.Name).ToArray();
				var originalItems = _testData.OrderBy(item => item.Name).ToArray();

				Assert.AreEqual(originalItems.Length, dataItems.Length);

				for (int i = 0; i < _testData.Length; i++)
				{
					var dataItem = dataItems[i];
					var originalItem = originalItems[i];

					Assert.AreEqual((object) originalItem.Id, dataItem.Id);
					Assert.AreEqual((object)originalItem.Name, dataItem.Name);
					Assert.AreEqual(originalItem.RssItems == null ? 0 : originalItem.RssItems.Count, dataItem.FeedItemCount);
				}
		   }
		}

		[TestMethod]
		public void Data_get_feed_to_process()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var dataActions = RepositoryFactory.Action<IRssFeedAction>();

				var dataItems = dataActions.GetFeedToProcess().OrderBy(item => item.Name).ToArray();
				var originalItems = _testData.OrderBy(item => item.Name).ToArray();

				Assert.AreEqual(dataItems.Length, originalItems.Length);

				for (int i = 0; i < _testData.Length; i++)
				{
					var dataItem = dataItems[i];
					var originalItem = originalItems[i];

					Assert.AreEqual((object)originalItem.Id, dataItem.Id);
					Assert.AreEqual((object)originalItem.Name, dataItem.Name);
				}
			}
		}

	}
}
