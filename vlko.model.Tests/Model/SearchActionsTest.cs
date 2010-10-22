using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Testing;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core;
using vlko.core.InversionOfControl;
using vlko.model.Action;
using vlko.model.Action.CRUDModel;
using vlko.model.Action.ViewModel;
using vlko.model.Repository;
using vlko.model.Search;

namespace vlko.model.Tests.Model
{
	[TestClass]
	public class SearchActionsTest : InMemoryTest
	{

		private User _user;

		[TestInitialize]
		public void Init()
		{
			IoC.InitializeWith(new WindsorContainer());
			ApplicationInit.InitializeRepositories();
			ApplicationInit.InitializeServices();
			base.SetUp();
			using (var tran = new TransactionScope())
			{
				RepositoryFactory.Action<IUserAction>().CreateAdmin("user", "user@user.sk", "test");
				tran.VoteCommit();
			}
			using (var session = new SessionScope())
			{
				_user = RepositoryFactory.Action<IUserAction>().GetByName("user");
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
		public void Test_index_static_text()
		{
			IoC.Resolve<ISearchProvider>().Initialize(Directory.GetCurrentDirectory());
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				RepositoryFactory.Action<ISearchAction>().IndexStaticText(tran, new StaticTextCRUDModel()
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

		[TestMethod]
		public void Test_index_comment()
		{
			IoC.Resolve<ISearchProvider>().Initialize(Directory.GetCurrentDirectory());
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				RepositoryFactory.Action<ISearchAction>().IndexComment(tran, new CommentCRUDModel()
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

		[TestMethod]
		public void Multithreading_test()
		{
			IoC.Resolve<ISearchProvider>().Initialize(Directory.GetCurrentDirectory());
			const int numberOfThreads = 20;

			int threadCount = 0;
			int threadToFinish = numberOfThreads;
			ManualResetEvent finished = new ManualResetEvent(false);

			// put one item just to get some result
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				RepositoryFactory.Action<ISearchAction>().IndexComment(tran, new CommentCRUDModel()
				                                                	{
				                                                		Id = Guid.NewGuid(),
				                                                		ChangeDate = DateTime.Now,
				                                                		Name = "test",
				                                                		Text = "very long test",
				                                                		ChangeUser = _user
				                                                	});
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
												RepositoryFactory.Action<ISearchAction>().IndexComment(tran, new CommentCRUDModel()
				                             						{
				                             							Id = Guid.NewGuid(),
				                             							ChangeDate = DateTime.Now,
				                             							Name = "test",
				                             							Text = "very long test",
				                             							ChangeUser = _user
				                             						});
												RepositoryFactory.Action<ISearchAction>().IndexStaticText(tran, new StaticTextCRUDModel()
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
												var searchResult = RepositoryFactory.Action<ISearchAction>().Search(session, "test");
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

		[TestMethod]
		public void Test_delete()
		{
			IoC.Resolve<ISearchProvider>().Initialize(Directory.GetCurrentDirectory());
			Guid idToDelete;
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				var startDate = DateTime.Now;
				var home = RepositoryFactory.Action<IStaticTextCrud>().Create(
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
				RepositoryFactory.Action<ISearchAction>().IndexStaticText(tran, home);
				tran.Commit();
			}
			using (var session = RepositoryFactory.StartUnitOfWork(IoC.Resolve<SearchContext>()))
			{
				// test search for user name
				var searchResult = RepositoryFactory.Action<ISearchAction>().Search(session, "delete");
				Assert.AreEqual(1, searchResult.Count());
			}
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				RepositoryFactory.Action<ISearchAction>().DeleteFromIndex(tran, idToDelete);
				tran.Commit();
			}
			using (var session = RepositoryFactory.StartUnitOfWork(IoC.Resolve<SearchContext>()))
			{
				// test search for user name
				var searchResult = RepositoryFactory.Action<ISearchAction>().Search(session, "delete");
				Assert.AreEqual(0, searchResult.Count());
			}
		}

		[TestMethod]
		public void Test_Update()
		{
			IoC.Resolve<ISearchProvider>().Initialize(Directory.GetCurrentDirectory());
			Guid idToUpdate;
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				var startDate = DateTime.Now;
				var home = RepositoryFactory.Action<IStaticTextCrud>().Create(
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
				RepositoryFactory.Action<ISearchAction>().IndexStaticText(tran, home);
				tran.Commit();
			}
			using (var session = RepositoryFactory.StartUnitOfWork(IoC.Resolve<SearchContext>()))
			{
				// test search for user name
				var searchResult = RepositoryFactory.Action<ISearchAction>().Search(session, "delete");
				Assert.AreEqual(1, searchResult.Count());
			}
			using (var tran = RepositoryFactory.StartTransaction(IoC.Resolve<SearchUpdateContext>()))
			{
				RepositoryFactory.Action<ISearchAction>().DeleteFromIndex(tran, idToUpdate);
				var home = RepositoryFactory.Action<IStaticTextCrud>().FindByPk(idToUpdate);
				home.Text = "nodelete me";
				RepositoryFactory.Action<IStaticTextCrud>().Update(home);
				RepositoryFactory.Action<ISearchAction>().IndexStaticText(tran, home);
				tran.Commit();
			}
			using (var session = RepositoryFactory.StartUnitOfWork(IoC.Resolve<SearchContext>()))
			{
				// test search for user name
				var searchResult = RepositoryFactory.Action<ISearchAction>().Search(session, "nodelete");
				Assert.AreEqual(1, searchResult.Count());
			}
		}

		[TestMethod]
		public void Test_search()
		{
			CreateTestData("search");

			using (var session = RepositoryFactory.StartUnitOfWork(IoC.Resolve<SearchContext>()))
			{
				// test search for user name
				var searchResult = RepositoryFactory.Action<ISearchAction>().Search(session, "user");
				Assert.AreEqual(231 - 2 /* two items are out of date range */, searchResult.Count());

				var data = searchResult.ToArray();
				Assert.AreEqual(SearchResult.MaximumRawResults, searchResult.ToArray().Length);

				// test search for text
				searchResult = RepositoryFactory.Action<ISearchAction>().Search(session, "home");
				Assert.AreEqual(31 , searchResult.Count());

				data = searchResult.ToPage(0, 2);
				Assert.AreEqual(2, data.Length);

				// test if static text was prioritized
				Assert.IsInstanceOfType(data[0], typeof(StaticTextViewModel));
				Assert.IsInstanceOfType(data[1], typeof(CommentViewModel));
			}
		}

		[TestMethod]
		public void Test_search_by_date()
		{
			CreateTestData("search_by_date");

			using (var session = RepositoryFactory.StartUnitOfWork(IoC.Resolve<SearchContext>()))
			{
				// test search for user name
				var searchResult = RepositoryFactory.Action<ISearchAction>().SearchByDate(session, "user");
				Assert.AreEqual(231 - 2 /* two items are out of date range */, searchResult.Count());

				var data = searchResult.ToArray();
				Assert.AreEqual(SearchResult.MaximumRawResults, searchResult.ToArray().Length);

				// test search for text
				searchResult = RepositoryFactory.Action<ISearchAction>().SearchByDate(session, "home");
				Assert.AreEqual(31, searchResult.Count());

				data = searchResult.ToArray();
				Assert.AreEqual(31, data.Length);

				// test if static text was prioritized
				Assert.IsInstanceOfType(data[0], typeof(CommentViewModel));
				Assert.IsInstanceOfType(data[1], typeof(CommentViewModel));
				Assert.IsInstanceOfType(data[2], typeof(StaticTextViewModel));
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
				var searchAction = RepositoryFactory.Action<ISearchAction>();
				var home = RepositoryFactory.Action<IStaticTextCrud>().Create(
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
					                          RepositoryFactory.Action<ICommentCrud>().Create(
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
								ChangeDate = startDate.AddDays(-i),
								PublishDate = startDate.AddDays(-i),
								Text = "Static page" + i,
								Description = "Static page" + i
							});
					searchAction.IndexStaticText(tran, text);
					searchAction.IndexComment(tran,
					                          RepositoryFactory.Action<ICommentCrud>().Create(
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
				}
				tran.Commit();
			}
		}
	}
}
