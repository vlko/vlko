using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core.Action;
using vlko.core.InversionOfControl;
using vlko.core.Repository;
using vlko.core.Roots;
using vlko.BlogModule.Action;
using vlko.BlogModule.Action.CRUDModel;
using vlko.BlogModule.Action.ViewModel;
using vlko.BlogModule.Roots;
using vlko.BlogModule.Search;
using vlko.core.Testing;

namespace vlko.BlogModule.Tests.Model
{
	public abstract class SearchActionsBaseTest : LocalTest
	{

		private User _user;

		public SearchActionsBaseTest(ITestProvider testProvider) : base(testProvider)
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

		public virtual void Test_index_static_text()
		{
			IoC.Resolve<ISearchProvider>().Initialize(Directory.GetCurrentDirectory());
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				RepositoryFactory.Command<ISearchCommands>().IndexStaticText(tran, new StaticTextCRUDModel()
				{
					Id = Guid.NewGuid(),
					PublishDate = DateTime.Now,
					ChangeDate = DateTime.Now,
					Title = "test",
					Text = "very long test",
					Creator = _user
				});
				tran.Commit();
			}
		}

		public virtual void Test_index_comment()
		{
			IoC.Resolve<ISearchProvider>().Initialize(Directory.GetCurrentDirectory());
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				RepositoryFactory.Command<ISearchCommands>().IndexComment(tran, new CommentCRUDModel()
				{
					Id = Guid.NewGuid(),
					ChangeDate = DateTime.Now,
					Name = "test",
					Text = "very long test",
					ChangeUser = _user
				});
				tran.Commit();
			}
		}

		public virtual void Test_index_twitter_status()
		{
			IoC.Resolve<ISearchProvider>().Initialize(Directory.GetCurrentDirectory());
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				RepositoryFactory.Command<ISearchCommands>().IndexTwitterStatus(tran, new TwitterStatus()
				{
					Id = Guid.NewGuid(),
					CreatedDate = DateTime.Now,
					Text = "very long test",
					User = _user.Name
				});
				tran.Commit();
			}
		}

		public virtual void Test_index_rss_item()
		{
			IoC.Resolve<ISearchProvider>().Initialize(Directory.GetCurrentDirectory());
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				RepositoryFactory.Command<ISearchCommands>().IndexRssItem(tran, new RssItemCRUDModel()
				{
					FeedItemId = "feed_id",
					Title = "title",
					Author = "author",
					Text = "text",
					Published = DateTime.Now
				});
				tran.Commit();
			}
		}

		public virtual void Multithreading_test()
		{
			IoC.Resolve<ISearchProvider>().Initialize(Directory.GetCurrentDirectory());
			const int numberOfThreads = 20;

			int threadCount = 0;
			int threadToFinish = numberOfThreads;
			ManualResetEvent finished = new ManualResetEvent(false);

			// put one item just to get some result
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				RepositoryFactory.Command<ISearchCommands>().IndexComment(tran, new CommentCRUDModel()
				                                                	{
				                                                		Id = Guid.NewGuid(),
				                                                		ChangeDate = DateTime.Now,
				                                                		Name = "test",
				                                                		Text = "very long test",
				                                                		ChangeUser = _user
				                                                	});
				tran.Commit();
			}

			for (int i = 0; i < numberOfThreads; i++)
			{
				Interlocked.Increment(ref threadCount);
				ThreadPool.QueueUserWorkItem(delegate
				                {
				                    try
				                    {
				                    	try
				                    	{
											using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
											{
												RepositoryFactory.Command<ISearchCommands>().IndexComment(tran, new CommentCRUDModel()
				                             						{
				                             							Id = Guid.NewGuid(),
				                             							ChangeDate = DateTime.Now,
				                             							Name = "test",
				                             							Text = "very long test",
				                             							ChangeUser = _user
				                             						});
												RepositoryFactory.Command<ISearchCommands>().IndexStaticText(tran, new StaticTextCRUDModel()
				                             						{
				                             							Id = Guid.NewGuid(),
				                             							PublishDate = DateTime.Now,
				                             							ChangeDate = DateTime.Now,
				                             							Title = "test",
				                             							Text = "very long test",
				                             							Creator = _user
				                             						});
												tran.Commit();
				                        		
											}
											using (var session = RepositoryFactory.StartUnitOfWork(IoC.Resolve<SearchContext>()))
											{
												// test search for user name
												var searchResult = RepositoryFactory.Command<ISearchCommands>().Search(session, "test");
												Assert.AreNotEqual(0, searchResult.Count());
											}
											Interlocked.Decrement(ref threadToFinish);
				                    	}
										catch (Exception e)
										{
											Debug.Write(threadToFinish + ";" + e.ToString());
											throw;
										}
				                    }
				                    finally
				                    {
				                        if (Interlocked.Decrement(ref threadCount) == 0)
				                        {
				                            finished.Set();
				                        }
				                    }
				                });

			}
			finished.WaitOne();
			Assert.AreEqual(0, threadToFinish);
		}

		public virtual void Test_delete()
		{
			IoC.Resolve<ISearchProvider>().Initialize(Directory.GetCurrentDirectory());
			Guid idToDelete;
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				var startDate = DateTime.Now;
				var home = RepositoryFactory.Command<IStaticTextCrud>().Create(
					new StaticTextCRUDModel
						{
							AllowComments = false,
							Creator = _user,
							Title = "To Delete",
							FriendlyUrl = "Home",
							ChangeDate = startDate.AddDays(-2),
							PublishDate = startDate,
							Text = "delete me",
							Description = "delete me"
						});
				idToDelete = home.Id;
				RepositoryFactory.Command<ISearchCommands>().IndexStaticText(tran, home);
				tran.Commit();
			}
			using (var session = RepositoryFactory.StartUnitOfWork(IoC.Resolve<SearchContext>()))
			{
				// test search for user name
				var searchResult = RepositoryFactory.Command<ISearchCommands>().Search(session, "delete");
				Assert.AreEqual(1, searchResult.Count());
			}
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				RepositoryFactory.Command<ISearchCommands>().DeleteFromIndex(tran, idToDelete.ToString());
				tran.Commit();
			}
			using (var session = RepositoryFactory.StartUnitOfWork(IoC.Resolve<SearchContext>()))
			{
				// test search for user name
				var searchResult = RepositoryFactory.Command<ISearchCommands>().Search(session, "delete");
				Assert.AreEqual(0, searchResult.Count());
			}
		}

		public virtual void Test_Update()
		{
			IoC.Resolve<ISearchProvider>().Initialize(Directory.GetCurrentDirectory());
			Guid idToUpdate;
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				var startDate = DateTime.Now;
				var home = RepositoryFactory.Command<IStaticTextCrud>().Create(
					new StaticTextCRUDModel
					{
						AllowComments = false,
						Creator = _user,
						Title = "To Delete",
						FriendlyUrl = "Home",
						ChangeDate = startDate.AddDays(-2),
						PublishDate = startDate,
						Text = "delete me",
						Description = "delete me"
					});
				idToUpdate = home.Id;
				RepositoryFactory.Command<ISearchCommands>().IndexStaticText(tran, home);
				tran.Commit();
			}
			using (var session = RepositoryFactory.StartUnitOfWork(IoC.Resolve<SearchContext>()))
			{
				// test search for user name
				var searchResult = RepositoryFactory.Command<ISearchCommands>().Search(session, "delete");
				Assert.AreEqual(1, searchResult.Count());
			}
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				RepositoryFactory.Command<ISearchCommands>().DeleteFromIndex(tran, idToUpdate.ToString());
				var home = RepositoryFactory.Command<IStaticTextCrud>().FindByPk(idToUpdate);
				home.Text = "nodelete me";
				RepositoryFactory.Command<IStaticTextCrud>().Update(home);
				RepositoryFactory.Command<ISearchCommands>().IndexStaticText(tran, home);
				tran.Commit();
			}
			using (var session = RepositoryFactory.StartUnitOfWork(IoC.Resolve<SearchContext>()))
			{
				// test search for user name
				var searchResult = RepositoryFactory.Command<ISearchCommands>().Search(session, "nodelete");
				Assert.AreEqual(1, searchResult.Count());
			}
		}

		public virtual void Test_search()
		{
			CreateTestData("search");

			using (var session = RepositoryFactory.StartUnitOfWork(IoC.Resolve<SearchContext>()))
			{
				// test search for user name
				var searchResult = RepositoryFactory.Command<ISearchCommands>().Search(session, "user");
				Assert.AreEqual(332 - 2 /* two items are out of date range */, searchResult.Count());

				var data = searchResult.ToArray();
				Assert.IsTrue(SearchResult.MaximumRawResults >= searchResult.ToArray().Length);

				// test search for text
				searchResult = RepositoryFactory.Command<ISearchCommands>().Search(session, "home");
				Assert.AreEqual(33 , searchResult.Count());

				data = searchResult.ToPage(0, 4);
				Assert.AreEqual(4, data.Length);

				// test if static text was prioritized
				Assert.IsInstanceOfType(data[0], typeof(StaticTextViewModel));
				Assert.IsInstanceOfType(data[1], typeof(RssItemViewModel));
				Assert.IsInstanceOfType(data[2], typeof(TwitterStatus));
				Assert.IsInstanceOfType(data[3], typeof(CommentViewModel));
			}
		}

		public virtual void Test_search_by_date()
		{
			CreateTestData("search_by_date");

			using (var session = RepositoryFactory.StartUnitOfWork(IoC.Resolve<SearchContext>()))
			{
				// test search for user name
				var searchResult = RepositoryFactory.Command<ISearchCommands>().SearchByDate(session, "user");
				Assert.AreEqual(332 - 2 /* two items are out of date range */, searchResult.Count());

				var data = searchResult.ToArray();
				Assert.IsTrue(SearchResult.MaximumRawResults >= searchResult.ToArray().Length);

				// test search for text
				searchResult = RepositoryFactory.Command<ISearchCommands>().SearchByDate(session, "home");
				Assert.AreEqual(33, searchResult.Count());

				data = searchResult.ToArray();
				Assert.AreEqual(33, data.Length);

				// test if static text was prioritized
				Assert.IsInstanceOfType(data[0], typeof(CommentViewModel));
				Assert.IsInstanceOfType(data[1], typeof(CommentViewModel));
				Assert.IsInstanceOfType(data[2], typeof(RssItemViewModel));
				Assert.IsInstanceOfType(data[3], typeof(StaticTextViewModel));
				Assert.IsInstanceOfType(data[5], typeof(TwitterStatus));
			}
		}

		private void CreateTestData(string subFolderName)
		{
			string directory = Directory.GetCurrentDirectory() + "\\" + subFolderName;
			Directory.CreateDirectory(directory);
			IoC.Resolve<ISearchProvider>().Initialize(directory);
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				var startDate = DateTime.Now;
				var searchAction = RepositoryFactory.Command<ISearchCommands>();

				// add feed item
				var feed = RepositoryFactory.Command<IRssFeedCommands>().Create(new RssFeedCRUDModel()
				                                                             	{
				                                                             		Name = "feed",
				                                                             		Url = "url"
				                                                             	});
				var feedItem = RepositoryFactory.Command<IRssItemCommands>().Save(new RssItemCRUDModel()
				           	{
				           		FeedItemId = "new",
				           		Title = "home",
				           		Text = "text",
				           		Description = "description",
				           		Author = "user",
				           		Url = "url",
								Published = startDate.AddDays(-2),
				           		FeedId = feed.Id
				           	});
				searchAction.IndexRssItem(tran, feedItem);


				// add to index some static text
				var home = RepositoryFactory.Command<IStaticTextCrud>().Create(
					new StaticTextCRUDModel
						{
							AllowComments = false,
							Creator = _user,
							Title = "Home",
							FriendlyUrl = "Home",
							ChangeDate = startDate.AddDays(-2),
							PublishDate = startDate,
							Text = "Welcome to vlko",
							Description = "Welcome to vlko"
						});
				searchAction.IndexStaticText(tran, home);
				for (int i = 0; i < 30; i++)
				{
					searchAction.IndexComment(tran,
					                          RepositoryFactory.Command<ICommentCrud>().Create(
					                          	new CommentCRUDModel()
					                          		{
					                          			AnonymousName = "User",
					                          			ChangeDate = startDate.AddDays(-i),
					                          			ClientIp = "127.0.0.1",
					                          			ContentId = home.Id,
					                          			Name = "Comment" + i,
					                          			Text = "Home commment" + i,
					                          			UserAgent = "Mozzilla"
					                          		}));
				}
				searchAction.IndexTwitterStatus(tran,
													RepositoryFactory.Command<ITwitterStatusCommands>().CreateStatus(
														new TwitterStatus()
														{
															TwitterId = 0,
															CreatedDate = startDate.AddDays(-2),
															Modified = startDate.AddDays(-2),
															Text = "twitter status",
															User = _user.Name,
															AreCommentAllowed = false,
															RetweetUser = "Home"
														}));

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
								ChangeDate = startDate.AddDays(-i),
								PublishDate = startDate.AddDays(-i),
								Text = "Static page" + i,
								Description = "Static page" + i
							});
					searchAction.IndexStaticText(tran, text);
					// add some comments
					searchAction.IndexComment(tran,
					                          RepositoryFactory.Command<ICommentCrud>().Create(
					                          	new CommentCRUDModel()
					                          		{
					                          			ChangeDate = startDate.AddDays(-i),
					                          			ChangeUser = _user,
					                          			ClientIp = "127.0.0.1",
					                          			ContentId = text.Id,
					                          			Name = "Comment" + i,
					                          			Text = "Static page" + i,
					                          			UserAgent = "Mozzilla"
					                          		}));

					// add some twitter status
					searchAction.IndexTwitterStatus(tran,
													RepositoryFactory.Command<ITwitterStatusCommands>().CreateStatus(
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
														}));
				}

				tran.Commit();
			}
		}
	}
}
