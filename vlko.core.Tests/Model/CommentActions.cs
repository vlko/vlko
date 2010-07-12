using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Testing;
using Castle.Windsor;
using GenericRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vlko.core.Models;
using vlko.core.Models.Action;
using vlko.core.Models.Action.ActionModel;

namespace vlko.core.Tests.Model
{
	[TestClass]
	public class CommentActions : InMemoryTest
	{
		private StaticText _testText;

		private User _user;

		[TestInitialize]
		public void Init()
		{
			InversionOfControl.IoC.InitializeWith(new WindsorContainer());
			ApplicationInit.InitializeRepositories();
			base.SetUp();
			using (var tran = new TransactionScope())
			{
				_user = new User()
							{
								Name = "user",
								Email = "user@user.sk"
							};
				_testText = new StaticText
								{
									Title = "text3",
									CreatedBy = _user,
									CreatedDate = DateTime.Now,
									ActualVersion = 0,
									PublishDate = DateTime.Now,
									AreCommentAllowed = true,
									StaticTextVersions = new List<StaticTextVersion>
															 {
																 new StaticTextVersion
																	 {
																		 CreatedBy = _user,
																		 Text = "only one version",
																		 Version = 0,
																		 CreatedDate = new DateTime(2002, 1, 1)
																	 },
															 }
								};
				_testText.Comments = new List<Comment>
										 {
											 new Comment
												 {
													 Name = "first_comment",
													 Content = _testText,
													 Owner = _user,
													 AnonymousName = null,
													 CreatedDate = new DateTime(2002, 1, 1),
													 ActualVersion = 0,
													 CommentVersions = new List<CommentVersion>()
																		   {
																			   new CommentVersion
																				   {
																					   CreatedDate =
																						   new DateTime(2002,
																										1, 1),
																					   CreatedBy = _user,
																					   ClientIp =
																						   "192.168.1.1",
																					   UserAgent = "mozilla",
																					   Text =
																						   "this is unique commen",
																					   Version = 0
																				   }
																		   }
												 },
											 new Comment
												 {
													 Name = "second_comment",
													 Content = _testText,
													 Owner = _user,
													 AnonymousName = null,
													 CreatedDate = new DateTime(2002, 1, 1),
													 ActualVersion = 1,
													 CommentVersions = new List<CommentVersion>()
																		   {
																			   new CommentVersion
																				   {
																					   CreatedDate =
																						   new DateTime(2002,
																										1, 1),
																					   CreatedBy = _user,
																					   ClientIp =
																						   "192.168.1.1",
																					   UserAgent = "mozilla",
																					   Text =
																						   "this is unique commen",
																					   Version = 0
																				   },
																			   new CommentVersion
																				   {
																					   CreatedDate =
																						   new DateTime(2002,
																										2, 1),
																					   CreatedBy = _user,
																					   ClientIp =
																						   "192.168.2.1",
																					   UserAgent = "mozilla",
																					   Text =
																						   "this is unique commen - change",
																					   Version = 1
																				   }
																		   }
												 }

										 };

				_testText.Comments[0].TopComment = _testText.Comments[0];
				_testText.Comments[1].TopComment = _testText.Comments[1];
				ActiveRecordMediator<User>.Create(_user);
				ActiveRecordMediator<StaticText>.Create(_testText);
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
				var crudActions = RepositoryFactory.GetRepository<Comment>().GetAction<ICommentCrud>();

				// get first
				var first = crudActions.FindByPk(_testText.Comments[0].Id);
				Assert.AreEqual(_testText.Comments[0].Id, first.Id);
				Assert.AreEqual(_testText.Comments[0].Name, first.Name);
				Assert.AreEqual(_testText.Comments[0].CommentVersions[0].CreatedDate, first.ChangeDate);
				Assert.AreEqual(_testText.Comments[0].CommentVersions[0].Text, first.Text);
				Assert.AreEqual(_testText.Comments[0].CommentVersions[0].CreatedBy.Id, first.ChangeUser.Id);
				Assert.AreEqual(_testText.Id, first.ContentId);

				// get second
				var second = crudActions.FindByPk(_testText.Comments[1].Id);
				Assert.AreEqual(_testText.Comments[1].Id, second.Id);
				Assert.AreEqual(_testText.Comments[1].Name, second.Name);
				Assert.AreEqual(_testText.Comments[1].CommentVersions[1].CreatedDate, second.ChangeDate);
				Assert.AreEqual(_testText.Comments[1].CommentVersions[1].Text, second.Text);
				Assert.AreEqual(_testText.Comments[1].CommentVersions[1].CreatedBy.Id, second.ChangeUser.Id);
				Assert.AreEqual(_testText.Id, second.ContentId);
			}
		}

		[TestMethod]
		public void Test_create()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var item = new CommentActionModel()
							   {
								   Name = "test",
								   ParentId = null,
								   ChangeUser = _user,
								   AnonymousName = null,
								   ClientIp = "127.0.0.1",
								   ContentId = _testText.Id,
								   Text = "test",
								   ChangeDate = new DateTime(2002, 1, 1),
								   UserAgent = "ie6"
							   };

				var crudActions = RepositoryFactory.GetRepository<Comment>().GetAction<ICommentCrud>();

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = crudActions.Create(item);
					tran.Commit();
				}

				var storedItem = crudActions.FindByPk(item.Id);

				Assert.AreEqual(item.Id, storedItem.Id);
				Assert.AreEqual(item.ChangeUser.Id, storedItem.ChangeUser.Id);
				Assert.AreEqual(item.ChangeDate, storedItem.ChangeDate);
				Assert.AreEqual(item.AnonymousName, storedItem.AnonymousName);
				Assert.AreEqual(item.UserAgent, storedItem.UserAgent);
				Assert.AreEqual(item.ClientIp, storedItem.ClientIp);
				Assert.AreEqual(item.Name, storedItem.Name);
				Assert.AreEqual(item.Text, storedItem.Text);
				Assert.AreEqual(item.ContentId, storedItem.ContentId);

				var subItem = new CommentActionModel()
								  {
									  Name = "subtest",
									  ParentId = item.Id,
									  ChangeUser = _user,
									  AnonymousName = null,
									  ClientIp = "127.0.0.1",
									  ContentId = _testText.Id,
									  Text = "subtest",
									  ChangeDate = new DateTime(2002, 1, 1),
									  UserAgent = "ie6"
								  };

				using (var tran = RepositoryFactory.StartTransaction())
				{
					subItem = crudActions.Create(subItem);
					tran.Commit();
				}

				storedItem = crudActions.FindByPk(subItem.Id);
				Assert.AreEqual(subItem.Id, storedItem.Id);
				Assert.AreEqual(subItem.ChangeUser.Id, storedItem.ChangeUser.Id);
				Assert.AreEqual(subItem.ChangeDate, storedItem.ChangeDate);
				Assert.AreEqual(subItem.AnonymousName, storedItem.AnonymousName);
				Assert.AreEqual(subItem.UserAgent, storedItem.UserAgent);
				Assert.AreEqual(subItem.ClientIp, storedItem.ClientIp);
				Assert.AreEqual(subItem.Name, storedItem.Name);
				Assert.AreEqual(subItem.Text, storedItem.Text);
				Assert.AreEqual(subItem.ContentId, storedItem.ContentId);
				Assert.AreEqual(subItem.ParentId, item.Id);

				var realItem = ActiveRecordMediator<Comment>.FindByPrimaryKey(item.Id);
				Assert.AreEqual(item.Id, realItem.TopComment.Id);
				var realSubItem = ActiveRecordMediator<Comment>.FindByPrimaryKey(item.Id);
				Assert.AreEqual(item.Id, realSubItem.TopComment.Id);
			}
		}

		[TestMethod]
		public void Test_update()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var item = new CommentActionModel()
				{
					Name = "test_update",
					ParentId = null,
					ChangeUser = _user,
					AnonymousName = null,
					ClientIp = "127.0.0.1",
					ContentId = _testText.Id,
					Text = "test_update",
					ChangeDate = new DateTime(2002, 1, 1),
					UserAgent = "ie6"
				};

				var crudActions = RepositoryFactory.GetRepository<Comment>().GetAction<ICommentCrud>();

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = crudActions.Create(item);
					tran.Commit();
				}

				var storedItem = crudActions.FindByPk(item.Id);

				Assert.AreEqual(item.Id, storedItem.Id);
				Assert.AreEqual(item.ChangeUser.Id, storedItem.ChangeUser.Id);
				Assert.AreEqual(item.ChangeDate, storedItem.ChangeDate);
				Assert.AreEqual(item.AnonymousName, storedItem.AnonymousName);
				Assert.AreEqual(item.UserAgent, storedItem.UserAgent);
				Assert.AreEqual(item.ClientIp, storedItem.ClientIp);
				Assert.AreEqual(item.Name, storedItem.Name);
				Assert.AreEqual(item.Text, storedItem.Text);
				Assert.AreEqual(item.ContentId, storedItem.ContentId);

				item.Text = "test_update_changed";
				item.ChangeDate = new DateTime(2002, 1, 1);
				item.ChangeUser = null;

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = crudActions.Create(item);
					tran.Commit();
				}

				storedItem = crudActions.FindByPk(item.Id);

				Assert.AreEqual(item.Id, storedItem.Id);
				Assert.AreEqual(null, storedItem.ChangeUser);
				Assert.AreEqual(item.ChangeDate, storedItem.ChangeDate);
				Assert.AreEqual(item.AnonymousName, storedItem.AnonymousName);
				Assert.AreEqual(item.UserAgent, storedItem.UserAgent);
				Assert.AreEqual(item.ClientIp, storedItem.ClientIp);
				Assert.AreEqual(item.Name, storedItem.Name);
				Assert.AreEqual(item.Text, storedItem.Text);
				Assert.AreEqual(item.ContentId, storedItem.ContentId);
			}
		}

		[TestMethod]
		public void Test_delete()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				var initialCommentCount = ActiveRecordMediator<Comment>.Count();
				var initialCommentVersionCount = ActiveRecordMediator<CommentVersion>.Count();

				var item = new CommentActionModel()
				{
					Name = "test_delete",
					ParentId = null,
					ChangeUser = _user,
					AnonymousName = null,
					ClientIp = "127.0.0.1",
					ContentId = _testText.Id,
					Text = "test_delete",
					ChangeDate = new DateTime(2002, 1, 1),
					UserAgent = "ie6"
				};

				var crudActions = RepositoryFactory.GetRepository<Comment>().GetAction<ICommentCrud>();

				using (var tran = RepositoryFactory.StartTransaction())
				{
					item = crudActions.Create(item);
					tran.Commit();
				}

				// check created item
				var storedItem = crudActions.FindByPk(item.Id);

				Assert.AreEqual(item.Id, storedItem.Id);
				Assert.AreEqual(item.ChangeUser.Id, storedItem.ChangeUser.Id);
				Assert.AreEqual(item.ChangeDate, storedItem.ChangeDate);
				Assert.AreEqual(item.AnonymousName, storedItem.AnonymousName);
				Assert.AreEqual(item.UserAgent, storedItem.UserAgent);
				Assert.AreEqual(item.ClientIp, storedItem.ClientIp);
				Assert.AreEqual(item.Name, storedItem.Name);
				Assert.AreEqual(item.Text, storedItem.Text);
				Assert.AreEqual(item.ContentId, storedItem.ContentId);

				Assert.AreNotEqual(initialCommentCount, ActiveRecordMediator<Comment>.Count());
				Assert.AreNotEqual(initialCommentVersionCount, ActiveRecordMediator<CommentVersion>.Count());

				using (var tran = RepositoryFactory.StartTransaction())
				{
					crudActions.Delete(item);
					tran.Commit();
				}

				Assert.AreEqual(initialCommentCount, ActiveRecordMediator<Comment>.Count());
				Assert.AreEqual(initialCommentVersionCount, ActiveRecordMediator<CommentVersion>.Count());
			}
		}


		[TestMethod]
		public void Get_by_ids()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				const int numberOfItems = 3;
				CreateCommentTree();
				var dataActions = RepositoryFactory.GetRepository<Comment>().GetAction<ICommentData>();
				var testData = dataActions.GetAllForAdmin()
					.OrderBy(comment => comment.Level)
					.ToPage(0, numberOfItems);
				var data = dataActions.GetByIds(testData.Select(item => item.Id))
					.OrderBy(comment => comment.Level)
					.ToArray();
				Assert.AreEqual(numberOfItems, data.Length);
				for (int i = 0; i < numberOfItems; i++)
				{
					Assert.AreEqual(testData[i].Id, data[i].Id);
					Assert.AreEqual(testData[i].Text, data[i].Text);
					Assert.IsInstanceOfType(data[i].Content, typeof(Content));
					Assert.IsNotNull(data[i].Content);
				}
			}
		}

		[TestMethod]
		public void GetAllFlat()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				CreateCommentTree();
				var dataActions = RepositoryFactory.GetRepository<Comment>().GetAction<ICommentData>();
				var data = dataActions.GetAllByDate(_testText.Id).ToArray();

				Assert.AreEqual("first_comment", data[0].Name);
				Assert.AreEqual("second_comment", data[1].Name);
				// 2003, 1, 1
				Assert.AreEqual("item0", data[2].Name);
				Assert.AreEqual("item00", data[3].Name);
				Assert.AreEqual("item000", data[4].Name);
				// 2003, 1, 2
				Assert.AreEqual("item1", data[5].Name);
				Assert.AreEqual("item01", data[6].Name);
				Assert.AreEqual("item001", data[7].Name);
				// 2003, 1, 3
				Assert.AreEqual("item2", data[8].Name);
				Assert.AreEqual("item02", data[9].Name);
				Assert.AreEqual("item20", data[10].Name);
				Assert.AreEqual("item200", data[11].Name);
				Assert.AreEqual("item2000", data[12].Name);
				Assert.AreEqual("item20000", data[13].Name);
				// 2003, 1, 4
				Assert.AreEqual("item03", data[14].Name);
			}
		}

		[TestMethod]
		public void GetAllFlatDesc()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				CreateCommentTree();
				var dataActions = RepositoryFactory.GetRepository<Comment>().GetAction<ICommentData>();
				var data = dataActions.GetAllByDateDesc(_testText.Id).ToArray();
				// 2003, 1, 4
				Assert.AreEqual("item03", data[0].Name);
				// 2003, 1, 3
				Assert.AreEqual("item20000", data[1].Name);
				Assert.AreEqual("item2000", data[2].Name);
				Assert.AreEqual("item200", data[3].Name);
				Assert.AreEqual("item02", data[4].Name);
				Assert.AreEqual("item20", data[5].Name);
				Assert.AreEqual("item2", data[6].Name);
				// 2003, 1, 2
				Assert.AreEqual("item001", data[7].Name);
				Assert.AreEqual("item01", data[8].Name);
				Assert.AreEqual("item1", data[9].Name);
				// 2003, 1, 1
				Assert.AreEqual("item000", data[10].Name);
				Assert.AreEqual("item00", data[11].Name);
				Assert.AreEqual("item0", data[12].Name);
				
				Assert.AreEqual("first_comment", data[13].Name);
				Assert.AreEqual("second_comment", data[14].Name);
				
			}
		}

		[TestMethod]
		public void GetAllTree()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				CreateCommentTree();
				var dataActions = RepositoryFactory.GetRepository<Comment>().GetAction<ICommentData>();
				var data = dataActions.GetCommentTree(_testText.Id).ToArray();

				
				Assert.AreEqual("first_comment", data[0].Name);
				Assert.AreEqual("second_comment", data[1].Name);

				Assert.AreEqual("item0", data[2].Name);
				var level01 = data[2].ChildNodes.ToArray();
				Assert.AreEqual("item00", level01[0].Name);
				Assert.AreEqual("item01", level01[1].Name);
				Assert.AreEqual("item02", level01[2].Name);
				Assert.AreEqual("item03", level01[3].Name);
				var level02 = level01[0].ChildNodes.ToArray();
				Assert.AreEqual("item000", level02[0].Name);
				Assert.AreEqual("item001", level02[1].Name);

				Assert.AreEqual("item1", data[3].Name);
				Assert.AreEqual("item2", data[4].Name);
				
				Assert.AreEqual("item20", data[4].ChildNodes.ToArray()[0].Name);
				Assert.AreEqual("item200", data[4].ChildNodes.ToArray()[0].ChildNodes.ToArray()[0].Name);
				Assert.AreEqual("item2000", data[4].ChildNodes.ToArray()[0].ChildNodes.ToArray()[0].ChildNodes.ToArray()[0].Name);
				Assert.AreEqual("item20000", data[4].ChildNodes.ToArray()[0].ChildNodes.ToArray()[0].ChildNodes.ToArray()[0].ChildNodes.ToArray()[0].Name);
			}
		}

		[TestMethod]
		public void GetAllAdmin()
		{
			using (RepositoryFactory.StartUnitOfWork())
			{
				CreateCommentTree();
				var dataActions = RepositoryFactory.GetRepository<Comment>().GetAction<ICommentData>();
				var data = dataActions.GetAllForAdmin()
					.OrderBy(comment => comment.CreatedDate)
					.OrderBy(comment => comment.Level)
					.ToArray();

				Assert.AreEqual("first_comment", data[0].Name);
				Assert.AreEqual("second_comment", data[1].Name);
				// 2003, 1, 1
				Assert.AreEqual("item0", data[2].Name);
				Assert.AreEqual("item00", data[3].Name);
				Assert.AreEqual("item000", data[4].Name);
				// 2003, 1, 2
				Assert.AreEqual("item1", data[5].Name);
				Assert.AreEqual("item01", data[6].Name);
				Assert.AreEqual("item001", data[7].Name);
				// 2003, 1, 3
				Assert.AreEqual("item2", data[8].Name);
				Assert.AreEqual("item02", data[9].Name);
				Assert.AreEqual("item20", data[10].Name);
				Assert.AreEqual("item200", data[11].Name);
				Assert.AreEqual("item2000", data[12].Name);
				Assert.AreEqual("item20000", data[13].Name);
				// 2003, 1, 4
				Assert.AreEqual("item03", data[14].Name);

				foreach (var comment in data)
				{
					Assert.AreEqual(_testText.Id, comment.ContentId);
					Assert.AreEqual(ContentType.StaticText, comment.ContentType);
				}
			}
		}

		/// <summary>
		/// Creates the comment tree.
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		private void CreateCommentTree()
		{
			using (var tran = RepositoryFactory.StartTransaction())
			{
				var crudActions = RepositoryFactory.GetRepository<Comment>().GetAction<ICommentCrud>();
				var item0 = crudActions.Create(new CommentActionModel()
												{
													Name = "item0",
													ParentId = null,
													ChangeUser = _user,
													AnonymousName = null,
													ClientIp = "127.0.0.1",
													ContentId = _testText.Id,
													Text = "item0",
													ChangeDate = new DateTime(2003, 1, 1),
													UserAgent = "ie6"
												});
				var item00 = crudActions.Create(new CommentActionModel()
				{
					Name = "item00",
					ParentId = item0.Id,
					ChangeUser = _user,
					AnonymousName = null,
					ClientIp = "127.0.0.1",
					ContentId = _testText.Id,
					Text = "item00",
					ChangeDate = new DateTime(2003, 1, 1),
					UserAgent = "ie6"
				});
				var item000 = crudActions.Create(new CommentActionModel()
				{
					Name = "item000",
					ParentId = item00.Id,
					ChangeUser = _user,
					AnonymousName = null,
					ClientIp = "127.0.0.1",
					ContentId = _testText.Id,
					Text = "item000",
					ChangeDate = new DateTime(2003, 1, 1),
					UserAgent = "ie6"
				});
				var item001 = crudActions.Create(new CommentActionModel()
				{
					Name = "item001",
					ParentId = item00.Id,
					ChangeUser = _user,
					AnonymousName = null,
					ClientIp = "127.0.0.1",
					ContentId = _testText.Id,
					Text = "item001",
					ChangeDate = new DateTime(2003, 1, 2),
					UserAgent = "ie6"
				});
				var item01 = crudActions.Create(new CommentActionModel()
				{
					Name = "item01",
					ParentId = item0.Id,
					ChangeUser = _user,
					AnonymousName = null,
					ClientIp = "127.0.0.1",
					ContentId = _testText.Id,
					Text = "item01",
					ChangeDate = new DateTime(2003, 1, 2),
					UserAgent = "ie6"
				});
				var item02 = crudActions.Create(new CommentActionModel()
				{
					Name = "item02",
					ParentId = item0.Id,
					ChangeUser = _user,
					AnonymousName = null,
					ClientIp = "127.0.0.1",
					ContentId = _testText.Id,
					Text = "item02",
					ChangeDate = new DateTime(2003, 1, 3),
					UserAgent = "ie6"
				});
				var item03 = crudActions.Create(new CommentActionModel()
				{
					Name = "item03",
					ParentId = item0.Id,
					ChangeUser = _user,
					AnonymousName = null,
					ClientIp = "127.0.0.1",
					ContentId = _testText.Id,
					Text = "item03",
					ChangeDate = new DateTime(2003, 1, 4),
					UserAgent = "ie6"
				});
				var item1 = crudActions.Create(new CommentActionModel()
				{
					Name = "item1",
					ParentId = null,
					ChangeUser = _user,
					AnonymousName = null,
					ClientIp = "127.0.0.1",
					ContentId = _testText.Id,
					Text = "item1",
					ChangeDate = new DateTime(2003, 1, 2),
					UserAgent = "ie6"
				});
				var item2 = crudActions.Create(new CommentActionModel()
				{
					Name = "item2",
					ParentId = null,
					ChangeUser = _user,
					AnonymousName = null,
					ClientIp = "127.0.0.1",
					ContentId = _testText.Id,
					Text = "item2",
					ChangeDate = new DateTime(2003, 1, 3),
					UserAgent = "ie6"
				});
				var item20 = crudActions.Create(new CommentActionModel()
				{
					Name = "item20",
					ParentId = item2.Id,
					ChangeUser = _user,
					AnonymousName = null,
					ClientIp = "127.0.0.1",
					ContentId = _testText.Id,
					Text = "item20",
					ChangeDate = new DateTime(2003, 1, 3),
					UserAgent = "ie6"
				});
				var item200 = crudActions.Create(new CommentActionModel()
				{
					Name = "item200",
					ParentId = item20.Id,
					ChangeUser = _user,
					AnonymousName = null,
					ClientIp = "127.0.0.1",
					ContentId = _testText.Id,
					Text = "item200",
					ChangeDate = new DateTime(2003, 1, 3),
					UserAgent = "ie6"
				});
				var item2000 = crudActions.Create(new CommentActionModel()
				{
					Name = "item2000",
					ParentId = item200.Id,
					ChangeUser = _user,
					AnonymousName = null,
					ClientIp = "127.0.0.1",
					ContentId = _testText.Id,
					Text = "item2000",
					ChangeDate = new DateTime(2003, 1, 3),
					UserAgent = "ie6"
				});
				var item20000 = crudActions.Create(new CommentActionModel()
				{
					Name = "item20000",
					ParentId = item2000.Id,
					ChangeUser = _user,
					AnonymousName = null,
					ClientIp = "127.0.0.1",
					ContentId = _testText.Id,
					Text = "item20000",
					ChangeDate = new DateTime(2003, 1, 3),
					UserAgent = "ie6"
				});
				tran.Commit();
			}
		}
	}
}
