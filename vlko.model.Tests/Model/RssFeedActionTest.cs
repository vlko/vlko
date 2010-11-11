using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Testing;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.model.Action;
using vlko.model.Action.CRUDModel;
using vlko.model.Roots;

namespace vlko.model.Tests.Model
{
	[TestClass]
	public class RssFeedActionTest : InMemoryTest
	{
		private RssFeed[] _testData;

		[TestInitialize]
		public void Init()
		{
			var doc = new XmlDocument();
			doc.Load("log4net.config");
			log4net.Config.XmlConfigurator.Configure(doc.DocumentElement);

			IoC.InitializeWith(new WindsorContainer());
			ApplicationInit.InitializeRepositories();
			base.SetUp();
			using (var tran = new TransactionScope())
			{
				var feed1 = new RssFeed
				            	{
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
					Name = "feed2",
					Url = "some_test_url2",
					AuthorRegex = "author_regex",
					DisplayFullContent = true,
					GetDirectContent = false,
					ContentParseRegex = "content_parser"
				};

				var feed3 = new RssFeed
				{
					Name = "feed3",
					Url = "some_test_url3",
					AuthorRegex = "author_regex",
					DisplayFullContent = false,
					GetDirectContent = true,
					ContentParseRegex = "content_parser"
				};

				ActiveRecordMediator<RssFeed>.Create(feed1);
				ActiveRecordMediator<RssFeed>.Create(feed2);
				ActiveRecordMediator<RssFeed>.Create(feed3);
				_testData = new[] { feed1, feed2, feed3 };
				tran.VoteCommit();
			}
			
		}

		[TestCleanup]
		public void Cleanup()
		{
			TearDown();
		}

		public override Type[] GetTypes()
		{
			return ApplicationInit.ListOfModelTypes();
		}

		[TestMethod]
		public void Test_find_by_primary_key()
		{
			using (new SessionScope())
			{
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
				var initialCount = ActiveRecordMediator<RssFeed>.Count();

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


				Assert.AreEqual(initialCount + 1, ActiveRecordMediator<RssFeed>.Count());

				using (var tran = RepositoryFactory.StartTransaction())
				{
					crudActions.Delete(item);
					tran.Commit();
				}

				Assert.AreEqual(initialCount, ActiveRecordMediator<RssFeed>.Count());
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

				Assert.AreEqual(dataItems.Length, originalItems.Length);

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

	}
}
