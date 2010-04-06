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
    public class StaticTextActions : InMemoryTest
    {
        private StaticText[] _testData;

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
                var statText1 = new StaticText
                                    {
                                        Title = "text1",
                                        CreatedBy = _user,
                                        CreatedDate = DateTime.Now,
                                        ActualVersion = 1,
                                        PublishDate = DateTime.Now,
                                        AreCommentAllowed = true,
                                        StaticTextVersions = new List<StaticTextVersion>
                                                                 {
                                                                     new StaticTextVersion
                                                                         {
                                                                             CreatedBy = _user,
                                                                             Text = "version0",
                                                                             Version = 0,
                                                                             CreatedDate = new DateTime(2002, 1, 1)
                                                                         },
                                                                    new StaticTextVersion
                                                                         {
                                                                             CreatedBy = _user,
                                                                             Text = "version1",
                                                                             Version = 1,
                                                                             CreatedDate = new DateTime(2002, 2, 1)
                                                                         }
                                                                 }
                                    };
                var statText2 = new StaticText
                {
                    Title = "text2",
                    CreatedBy = _user,
                    CreatedDate = DateTime.Now,
                    ActualVersion = 2,
                    PublishDate = DateTime.Now,
                    AreCommentAllowed = true,
                    StaticTextVersions = new List<StaticTextVersion>
                                                                 {
                                                                     new StaticTextVersion
                                                                         {
                                                                             CreatedBy = _user,
                                                                             Text = "version0",
                                                                             Version = 0,
                                                                             CreatedDate = new DateTime(2002, 1, 1)
                                                                         },
                                                                    new StaticTextVersion
                                                                         {
                                                                             CreatedBy = _user,
                                                                             Text = "version1",
                                                                             Version = 1,
                                                                             CreatedDate = new DateTime(2002, 2, 1)
                                                                         },
                                                                    new StaticTextVersion
                                                                         {
                                                                             CreatedBy = _user,
                                                                             Text = "version2",
                                                                             Version = 2,
                                                                             CreatedDate = new DateTime(2002, 3, 1)
                                                                         }
                                                                 }
                };
                var statText3 = new StaticText
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

                ActiveRecordMediator<User>.Create(_user);
                ActiveRecordMediator<StaticText>.Create(statText1);
                ActiveRecordMediator<StaticText>.Create(statText2);
                ActiveRecordMediator<StaticText>.Create(statText3);
                _testData = new[] { statText1, statText2, statText3 };
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
                var crudActions = RepositoryFactory.GetRepository<StaticText>().GetAction<IStaticTextCrud>();

                // get first
                var first = crudActions.FindByPk(_testData[0].Id);
                Assert.AreEqual(_testData[0].Id, first.Id);
                Assert.AreEqual(_testData[0].Title, first.Title);
                Assert.AreEqual(_testData[0].StaticTextVersions[1].CreatedDate, first.ChangeDate);
                Assert.AreEqual(_testData[0].StaticTextVersions[1].Text, first.Text);

                // get second
                var second = crudActions.FindByPk(_testData[1].Id);
                Assert.AreEqual(_testData[1].Id, second.Id);
                Assert.AreEqual(_testData[1].Title, second.Title);
                Assert.AreEqual(_testData[1].StaticTextVersions[2].CreatedDate, second.ChangeDate);
                Assert.AreEqual(_testData[1].StaticTextVersions[2].Text, second.Text);

                // get third
                var third = crudActions.FindByPk(_testData[2].Id);
                Assert.AreEqual(_testData[2].Id, third.Id);
                Assert.AreEqual(_testData[2].Title, third.Title);
                Assert.AreEqual(_testData[2].StaticTextVersions[0].CreatedDate, third.ChangeDate);
                Assert.AreEqual(_testData[2].StaticTextVersions[0].Text, third.Text);
            }
        }

        [TestMethod]
        public void Test_create()
        {
            using (RepositoryFactory.StartUnitOfWork())
            {
                var item = new StaticTextActionModel()
                               {
                                   Creator = _user,
                                   ChangeDate = new DateTime(2002, 1, 1),
                                   PublishDate = new DateTime(2002, 2, 1),
                                   AllowComments = true,
                                   Title = "create new",
                                   Text = "create new content"
                               };

                var crudActions = RepositoryFactory.GetRepository<StaticText>().GetAction<IStaticTextCrud>();

                using (var tran = RepositoryFactory.StartTransaction())
                {
                    item = crudActions.Create(item);
                    tran.Commit();
                }


                var storedItem = crudActions.FindByPk(item.Id);

                Assert.AreEqual(item.Id, storedItem.Id);
                Assert.AreEqual(item.Creator.Id, storedItem.Creator.Id);
                Assert.AreEqual(item.ChangeDate, storedItem.ChangeDate);
                Assert.AreEqual(item.PublishDate, storedItem.PublishDate);
                Assert.AreEqual(item.AllowComments, storedItem.AllowComments);
                Assert.AreEqual(item.Title, storedItem.Title);
                Assert.AreEqual(item.Text, storedItem.Text);
            }
        }

        [TestMethod]
        public void Test_update()
        {
            using (RepositoryFactory.StartUnitOfWork())
            {
                var item = new StaticTextActionModel()
                {
                    Creator = _user,
                    ChangeDate = new DateTime(2002, 1, 1),
                    PublishDate = new DateTime(2002, 2, 1),
                    AllowComments = true,
                    Title = "updateable new",
                    Text = "updateable new content"
                };

                var crudActions = RepositoryFactory.GetRepository<StaticText>().GetAction<IStaticTextCrud>();

                using (var tran = RepositoryFactory.StartTransaction())
                {
                    item = crudActions.Create(item);
                    tran.Commit();
                }

                // check created item
                var storedItem = crudActions.FindByPk(item.Id);

                Assert.AreEqual(item.Id, storedItem.Id);
                Assert.AreEqual(item.Creator.Id, storedItem.Creator.Id);
                Assert.AreEqual(item.ChangeDate, storedItem.ChangeDate);
                Assert.AreEqual(item.PublishDate, storedItem.PublishDate);
                Assert.AreEqual(item.AllowComments, storedItem.AllowComments);
                Assert.AreEqual(item.Title, storedItem.Title);
                Assert.AreEqual(item.Text, storedItem.Text);

                // change item
                item.Text = "updateable new content - change 1";
                item.ChangeDate = new DateTime(2002, 2, 1);

                using (var tran = RepositoryFactory.StartTransaction())
                {
                    item = crudActions.Update(item);
                    tran.Commit();
                }

                // check changed item
                storedItem = crudActions.FindByPk(item.Id);

                Assert.AreEqual(item.Id, storedItem.Id);
                Assert.AreEqual(item.Creator.Id, storedItem.Creator.Id);
                Assert.AreEqual(item.ChangeDate, storedItem.ChangeDate);
                Assert.AreEqual(item.PublishDate, storedItem.PublishDate);
                Assert.AreEqual(item.AllowComments, storedItem.AllowComments);
                Assert.AreEqual(item.Title, storedItem.Title);
                Assert.AreEqual(item.Text, storedItem.Text);

                // change item
                item.Text = "updateable new content - change 2";
                item.ChangeDate = new DateTime(2002, 3, 1);

                using (var tran = RepositoryFactory.StartTransaction())
                {
                    item = crudActions.Update(item);
                    tran.Commit();
                }

                // check changed item
                storedItem = crudActions.FindByPk(item.Id);

                Assert.AreEqual(item.Id, storedItem.Id);
                Assert.AreEqual(item.Creator.Id, storedItem.Creator.Id);
                Assert.AreEqual(item.ChangeDate, storedItem.ChangeDate);
                Assert.AreEqual(item.PublishDate, storedItem.PublishDate);
                Assert.AreEqual(item.AllowComments, storedItem.AllowComments);
                Assert.AreEqual(item.Title, storedItem.Title);
                Assert.AreEqual(item.Text, storedItem.Text);

                // check if there are 3 items in history
                var staticText = ActiveRecordMediator<StaticText>.FindByPrimaryKey(item.Id);
                Assert.AreEqual(3, staticText.StaticTextVersions.Count);
                Assert.AreEqual(new DateTime(2002, 1, 1), staticText.StaticTextVersions[0].CreatedDate);
                Assert.AreEqual("updateable new content", staticText.StaticTextVersions[0].Text);
                Assert.AreEqual(new DateTime(2002, 2, 1), staticText.StaticTextVersions[1].CreatedDate);
                Assert.AreEqual("updateable new content - change 1", staticText.StaticTextVersions[1].Text);
                Assert.AreEqual(new DateTime(2002, 3, 1), staticText.StaticTextVersions[2].CreatedDate);
                Assert.AreEqual("updateable new content - change 2", staticText.StaticTextVersions[2].Text);
            }
        }

        [TestMethod]
        public void Test_delete()
        {
            using (RepositoryFactory.StartUnitOfWork())
            {
                var initialStaticTextCount = ActiveRecordMediator<StaticText>.Count();
                var initialStaticTextVersionCount = ActiveRecordMediator<StaticTextVersion>.Count();
                var item = new StaticTextActionModel()
                               {
                                   Creator = _user,
                                   ChangeDate = new DateTime(2002, 1, 1),
                                   PublishDate = new DateTime(2002, 2, 1),
                                   AllowComments = true,
                                   Title = "updateable new",
                                   Text = "updateable new content"
                               };

                var crudActions = RepositoryFactory.GetRepository<StaticText>().GetAction<IStaticTextCrud>();

                using (var tran = RepositoryFactory.StartTransaction())
                {
                    item = crudActions.Create(item);
                    tran.Commit();
                }

                // check created item
                var storedItem = crudActions.FindByPk(item.Id);

                Assert.AreEqual(item.Id, storedItem.Id);
                Assert.AreEqual(item.Creator.Id, storedItem.Creator.Id);
                Assert.AreEqual(item.ChangeDate, storedItem.ChangeDate);
                Assert.AreEqual(item.PublishDate, storedItem.PublishDate);
                Assert.AreEqual(item.AllowComments, storedItem.AllowComments);
                Assert.AreEqual(item.Title, storedItem.Title);
                Assert.AreEqual(item.Text, storedItem.Text);

                Assert.AreNotEqual(initialStaticTextCount, ActiveRecordMediator<StaticText>.Count());
                Assert.AreNotEqual(initialStaticTextVersionCount, ActiveRecordMediator<StaticTextVersion>.Count());

                using (var tran = RepositoryFactory.StartTransaction())
                {
                    crudActions.Delete(item);
                    tran.Commit();
                }

                Assert.AreEqual(initialStaticTextCount , ActiveRecordMediator<StaticText>.Count());
                Assert.AreEqual(initialStaticTextVersionCount, ActiveRecordMediator<StaticTextVersion>.Count());
            }
        }
    }
}
