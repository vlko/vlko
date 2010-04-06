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
            model.IoC.IoC.InitializeWith(new WindsorContainer());
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
                                                     Name = "first_comment",
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
                Assert.AreEqual(_testText.Comments[0].Content.Id, first.Content.Id);

                // get second
                var second = crudActions.FindByPk(_testText.Comments[1].Id);
                Assert.AreEqual(_testText.Comments[1].Id, second.Id);
                Assert.AreEqual(_testText.Comments[1].Name, second.Name);
                Assert.AreEqual(_testText.Comments[1].CommentVersions[1].CreatedDate, second.ChangeDate);
                Assert.AreEqual(_testText.Comments[1].CommentVersions[1].Text, second.Text);
                Assert.AreEqual(_testText.Comments[1].CommentVersions[1].CreatedBy.Id, second.ChangeUser.Id);
                Assert.AreEqual(_testText.Comments[1].Content.Id, second.Content.Id);
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
                                   Content = _testText,
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
                Assert.AreEqual(item.Content.Id, storedItem.Content.Id);

                var subItem = new CommentActionModel()
                                  {
                                      Name = "subtest",
                                      ParentId = item.Id,
                                      ChangeUser = _user,
                                      AnonymousName = null,
                                      ClientIp = "127.0.0.1",
                                      Content = _testText,
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
                Assert.AreEqual(subItem.Content.Id, storedItem.Content.Id);
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
                    Content = _testText,
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
                Assert.AreEqual(item.Content.Id, storedItem.Content.Id);

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
                Assert.AreEqual(item.Content.Id, storedItem.Content.Id);
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
                    Content = _testText,
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
                Assert.AreEqual(item.Content.Id, storedItem.Content.Id);

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
    }
}
