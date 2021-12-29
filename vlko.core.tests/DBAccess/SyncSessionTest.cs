using finstat.BLL.tests;
using Raven.Client.Documents.Queries.Timings;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using vlko.core.DBAccess;
using vlko.core.InversionOfControl;
using vlko.core.RavenDB.DBAccess;
using vlko.core.tests.DBAccess.Model;
using Xunit;

namespace vlko.core.tests.DBAccess
{
    public class SyncSessionTest : LocalStaticMemoryClientTest
    {
        private static string storeIdent = "sync_session";
        static SyncSessionTest()
        {
            var scope = IoC.Scope[storeIdent];
            scope.AddCatalogAssembly(Assembly.Load("vlko.core"));
            scope.AddCatalogAssembly(Assembly.Load("vlko.core.RavenDB"));
            scope.Initialize();
        }

        public SyncSessionTest()
        {
            var scope = IoC.Scope[storeIdent];
            SetUp(InitDB, storeIdent, scope);
        }

        private void InitDB(string storeIdent)
        {
            FillWithData(storeIdent);
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                new IndexForIndexItem().Execute(session.Advanced.DocumentStore);
                WaitForIndexing(session);
            }
        }

        internal static void FillWithData(string storeIdent)
        {
            Func<int, IndexItem> generateItem = index => new IndexItem
            {
                Id = $"test-{index}",
                Text = $"text-{index}",
                NotIndexed = $"not-indexed-text-{index}",
                Value = index * 1.1,
                Date = new DateTime(2000 + index, 1, 1),
                Integer = index,
                NullDate = index % 2 == 0 ? new DateTime(2000 + index, 2, 1) : (DateTime?)null,
                NullValue = index % 2 == 1 ? index * 2 : (double?)null,

            };
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                for (int i = 0; i < 100; i++)
                {
                    session.Store(generateItem(i));
                }
                tran.Commit();
            }
        }

        [Fact]
        public void StoreLoadDelete()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                session.Store(new SimpleItem
                {
                    Id = "delete-test",
                    Text = "test"
                });
                tran.Commit();
            }
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                var item = session.Load<SimpleItem>("delete-test");
                item.ShouldNotBeNull();
                session.Delete(item);
                tran.Commit();
            }

            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var item = session.Load<SimpleItem>("delete-test", false);
                item.ShouldBeNull();
            }
        }

        [Fact]
        public void StoreInTransaction()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                session.Store(new SimpleItem
                {
                    Id = "store-test",
                    Text = "test"
                });
                tran.Commit();
            }
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var item = session.Load<SimpleItem>("store-test");
                item.ShouldNotBeNull();
                item.Id.ShouldBe("store-test");
                item.Text.ShouldBe("test");
            }
        }

        [Fact]
        public void LoadMore()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                for (int i = 0; i < 5; i++)
                {
                    session.Store(new SimpleItem
                    {
                        Id = "more-test" + i,
                        Text = "test" + i
                    });
                }

                tran.Commit();
            }

            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var items = session.LoadMore<SimpleItem>("more-test0", "more-test1", "more-test2", "more-test3", "more-test4", "not-existing");
                // last one is null as not existing
                items.Count.ShouldBe(6);
                items["not-existing"].ShouldBeNull();
                for (int i = 0; i < 5; i++)
                {
                    items["more-test" + i].ShouldNotBeNull();
                    items["more-test" + i].Id.ShouldBe("more-test" + i);
                }
            }
        }

        [Fact]
        public void LoadMoreWithTransformer()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                session.Store(new SimpleItem
                {
                    Id = "transform-load",
                    Text = "TransformDocumentLoad"
                });
                tran.Commit();
            }
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var items = session.LoadMoreWithTransformer<IndexItem, TestLoadTransformer, TransformResult>("test-0", "test-10", "test-99", "not-existing");
                // last one is null as not existing
                items.Length.ShouldBe(3);
                // test first item
                items[0].ShouldNotBeNull();
                items[0].Id.ShouldBe("test-0");
                items[0].Composited.ShouldBe("text-0|0");
                items[0].Computed.ShouldBe("TransformDocumentLoad");
                // test second item
                items[1].ShouldNotBeNull();
                items[1].Id.ShouldBe("test-10");
                items[1].Composited.ShouldBe("text-10|10");
                items[1].Computed.ShouldBe("TransformDocumentLoad");
                // test third item
                items[2].ShouldNotBeNull();
                items[2].Id.ShouldBe("test-99");
                items[2].Composited.ShouldBe("text-99|99");
                items[2].Computed.ShouldBe("TransformDocumentLoad");
            }
        }

        [Fact]
        public void LoadWithTransformer()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                session.Store(new SimpleItem
                {
                    Id = "transform-load",
                    Text = "TransformDocumentLoad"
                });
                tran.Commit();
            }
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var item = session.LoadWithTransformer<IndexItem, TestLoadTransformer, TransformResult>("test-0");
                // test first item
                item.ShouldNotBeNull();
                item.Id.ShouldBe("test-0");
                item.Composited.ShouldBe("text-0|0");
                item.Computed.ShouldBe("TransformDocumentLoad");
            }
        }

        [Fact]
        public void TestEvict()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                session.Store(new SimpleItem
                {
                    Id = "evict-test",
                    Text = "test"
                });
                for (int i = 0; i < 7; i++)
                {
                    session.Store(new SimpleItem
                    {
                        Id = "evict-test" + i,
                        Text = "test" + i
                    });
                }

                tran.Commit();
            }
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var toStoreManually = session.Load<SimpleItem>("evict-test");
                toStoreManually.Text += "change";
                var outOfTransaction = session.Load<SimpleItem>("evict-test5");
                outOfTransaction.Text += "change";
                using (var tran = session.StartTransaction())
                {
                    var item = session.Load<SimpleItem>("evict-test0");
                    var itemToEvict = session.Load<SimpleItem>("evict-test1");
                    var evictItems = session.LoadMoreEvict<SimpleItem>(new[] { "evict-test2", "evict-test3" });
                    var evictItem = session.LoadEvict<SimpleItem>("evict-test4", true);

                    item.Text += "change";
                    itemToEvict.Text += "change";
                    evictItems["evict-test2"].Text += "change";
                    evictItems["evict-test3"].Text += "change";
                    evictItem.Text += "change";

                    session.Evict(itemToEvict);
                    session.Store(toStoreManually);

                    tran.Commit();
                }
            }
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var item = session.Load<SimpleItem>("evict-test0");
                var manuallyStored = session.Load<SimpleItem>("evict-test");
                var notChangedItems = session.LoadMore<SimpleItem>("evict-test1", "evict-test2", "evict-test3", "evict-test4", "evict-test5");
                item.Text.ShouldEndWith("change");
                manuallyStored.Text.ShouldEndWith("change");
                foreach (var notChanged in notChangedItems)
                {
                    notChanged.Value.Text.ShouldNotEndWith("change");
                }
            }
        }

        [Fact]
        public void RunQuery()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var items = session.Query<IndexForIndexItem, IndexItem>().ToArray();
                items.ShouldNotBeNull();
                items.Length.ShouldBe(100);
                // test third item
                items[2].ShouldNotBeNull();
                items[2].Id.ShouldBe("test-2");
                items[2].Text.ShouldBe("text-2");
                items[2].NotIndexed.ShouldBe("not-indexed-text-2");
                items[2].Integer.ShouldBe(2);
                items[2].Value.ShouldBe(2 * 1.1);
                items[2].Date.ShouldBe(new DateTime(2002, 1, 1));
                items[2].NullDate.ShouldBe(new DateTime(2002, 2, 1));
                items[2].NullValue.ShouldBeNull();
                // test last item
                items[99].ShouldNotBeNull();
                items[99].Id.ShouldBe("test-99");
                items[99].Text.ShouldBe("text-99");
                items[99].Integer.ShouldBe(99);
                items[99].Value.ShouldBe(99 * 1.1);
                items[99].Date.ShouldBe(new DateTime(2099, 1, 1));
                items[99].NullDate.ShouldBeNull();
                items[99].NullValue.ShouldBe(99 * 2);
            }
        }

        [Fact]
        public void RunQueryWithProjection()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var items = session.QueryWithProjection<IndexForIndexItem, ProjectionItem>()
                        .ToArray();
                items.ShouldNotBeNull();
                items.Length.ShouldBe(100);
                // test third item
                items[2].ShouldNotBeNull();
                items[2].Id.ShouldBe("test-2");
                items[2].Text.ShouldBe("text-2");
                items[2].Computed.ShouldBe("test-2|text-2");
                items[2].Integer.ShouldBe(2);
                items[2].Value.ShouldBe(2 * 1.1);
                items[2].Date.ShouldBe(new DateTime(2002, 1, 1));
                items[2].NullDate.ShouldBe(new DateTime(2002, 2, 1));
                items[2].NullValue.ShouldBeNull();
                // test last item
                items[99].ShouldNotBeNull();
                items[99].Id.ShouldBe("test-99");
                items[99].Text.ShouldBe("text-99");
                items[99].Integer.ShouldBe(99);
                items[99].Value.ShouldBe(99 * 1.1);
                items[99].Date.ShouldBe(new DateTime(2099, 1, 1));
                items[99].NullDate.ShouldBeNull();
                items[99].NullValue.ShouldBe(99 * 2);
            }
        }

        [Fact]
        public void RunDocumentQuery()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var items = session.DocumentQuery<IndexForIndexItem, IndexItem>().ToArray();
                items.ShouldNotBeNull();
                items.Length.ShouldBe(100);
                // test third item
                items[2].ShouldNotBeNull();
                items[2].Id.ShouldBe("test-2");
                items[2].Text.ShouldBe("text-2");
                items[2].NotIndexed.ShouldBe("not-indexed-text-2");
                items[2].Integer.ShouldBe(2);
                items[2].Value.ShouldBe(2 * 1.1);
                items[2].Date.ShouldBe(new DateTime(2002, 1, 1));
                items[2].NullDate.ShouldBe(new DateTime(2002, 2, 1));
                items[2].NullValue.ShouldBeNull();
                // test last item
                items[99].ShouldNotBeNull();
                items[99].Id.ShouldBe("test-99");
                items[99].Text.ShouldBe("text-99");
                items[99].Integer.ShouldBe(99);
                items[99].Value.ShouldBe(99 * 1.1);
                items[99].Date.ShouldBe(new DateTime(2099, 1, 1));
                items[99].NullDate.ShouldBeNull();
                items[99].NullValue.ShouldBe(99 * 2);
            }
        }

        [Fact]
        public void RunDocoumentQueryWithProjection()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                QueryTimings timings;
                var items = session.DocumentQueryWithProjection<IndexForIndexItem, ProjectionItem>()
                    .Timings(out timings).ToArray();
                items.ShouldNotBeNull();
                items.Length.ShouldBe(100);
                // test third item
                items[2].ShouldNotBeNull();
                items[2].Id.ShouldBe("test-2");
                items[2].Text.ShouldBe("text-2");
                items[2].Computed.ShouldBe("test-2|text-2");
                items[2].Integer.ShouldBe(2);
                items[2].Value.ShouldBe(2 * 1.1);
                items[2].Date.ShouldBe(new DateTime(2002, 1, 1));
                items[2].NullDate.ShouldBe(new DateTime(2002, 2, 1));
                items[2].NullValue.ShouldBeNull();
                // test last item
                items[99].ShouldNotBeNull();
                items[99].Id.ShouldBe("test-99");
                items[99].Text.ShouldBe("text-99");
                items[99].Integer.ShouldBe(99);
                items[99].Value.ShouldBe(99 * 1.1);
                items[99].Date.ShouldBe(new DateTime(2099, 1, 1));
                items[99].NullDate.ShouldBeNull();
                items[99].NullValue.ShouldBe(99 * 2);
            }
        }

        [Fact]
        public void RunQueryDef()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var query = session.Query<IndexForIndexItem, IndexItem>();
                var page = session.GetQueryDef(query)
                    .LoadPartial(0, 100);
                page.ShouldNotBeNull();
                page.TotalCount.ShouldBe(100);
                var items = page.PageData.ToArray();
                // test third item
                items[2].ShouldNotBeNull();
                items[2].Id.ShouldBe("test-2");
                items[2].Text.ShouldBe("text-2");
                items[2].NotIndexed.ShouldBe("not-indexed-text-2");
                items[2].Integer.ShouldBe(2);
                items[2].Value.ShouldBe(2 * 1.1);
                items[2].Date.ShouldBe(new DateTime(2002, 1, 1));
                items[2].NullDate.ShouldBe(new DateTime(2002, 2, 1));
                items[2].NullValue.ShouldBeNull();
                // test last item
                items[99].ShouldNotBeNull();
                items[99].Id.ShouldBe("test-99");
                items[99].Text.ShouldBe("text-99");
                items[99].Integer.ShouldBe(99);
                items[99].Value.ShouldBe(99 * 1.1);
                items[99].Date.ShouldBe(new DateTime(2099, 1, 1));
                items[99].NullDate.ShouldBeNull();
                items[99].NullValue.ShouldBe(99 * 2);
            }
        }

        [Fact]
        public void RunQueryDefProjection()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var query = session.Query<IndexForIndexItem, IndexItem>();
                var page = session.GetQueryDef(query)
                    .Project<ProjectionItem>()
                    .LoadPartial(0, 100);
                page.ShouldNotBeNull();
                page.TotalCount.ShouldBe(100);
                var items = page.PageData.ToArray();
                // test third item
                items[2].ShouldNotBeNull();
                items[2].Id.ShouldBe("test-2");
                items[2].Text.ShouldBe("text-2");
                items[2].Computed.ShouldBe("test-2|text-2");
                items[2].Integer.ShouldBe(2);
                items[2].Value.ShouldBe(2 * 1.1);
                items[2].Date.ShouldBe(new DateTime(2002, 1, 1));
                items[2].NullDate.ShouldBe(new DateTime(2002, 2, 1));
                items[2].NullValue.ShouldBeNull();
                // test last item
                items[99].ShouldNotBeNull();
                items[99].Id.ShouldBe("test-99");
                items[99].Text.ShouldBe("text-99");
                items[99].Integer.ShouldBe(99);
                items[99].Value.ShouldBe(99 * 1.1);
                items[99].Date.ShouldBe(new DateTime(2099, 1, 1));
                items[99].NullDate.ShouldBeNull();
                items[99].NullValue.ShouldBe(99 * 2);
            }
        }

        [Fact]
        public void RunQueryDefSimpleTransformer()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var query = session.Query<IndexForIndexItem, IndexItem>();
                var page = session.GetQueryDef(query)
                    .Transform<TransformResult, TestTransformer>()
                    .LoadPartial(0, 100);
                page.ShouldNotBeNull();
                page.TotalCount.ShouldBe(100);
                var items = page.PageData.ToArray();
                // test third item
                items[2].ShouldNotBeNull();
                items[2].Id.ShouldBe("test-2");
                items[2].Composited.ShouldBe("text-2|2");
                items[2].Computed.ShouldBe("this is generated on fly");
                // test last item
                items[99].ShouldNotBeNull();
                items[99].Id.ShouldBe("test-99");
                items[99].Composited.ShouldBe("text-99|99");
                items[99].Computed.ShouldBe("this is generated on fly");
            }
        }

        [Fact]
        public void RunQueryDefTransformerWithLoad()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                session.Store(new SimpleItem
                {
                    Id = "transform-load",
                    Text = "TransformDocumentLoad"
                });
                tran.Commit();
            }
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var query = session.Query<IndexForIndexItem, IndexItem>();
                var page = session.GetQueryDef(query)
                    .Transform<TransformResult, TestLoadTransformer>()
                    .LoadPartial(0, 20);
                page.ShouldNotBeNull();
                page.TotalCount.ShouldBe(100);
                page.PageData.Count().ShouldBe(20);
                var items = page.PageData.ToArray();
                // test third item
                items[2].ShouldNotBeNull();
                items[2].Id.ShouldBe("test-2");
                items[2].Composited.ShouldBe("text-2|2");
                items[2].Computed.ShouldBe("TransformDocumentLoad");
                // test last item
                items[19].ShouldNotBeNull();
                items[19].Id.ShouldBe("test-19");
                items[19].Composited.ShouldBe("text-19|19");
                items[19].Computed.ShouldBe("TransformDocumentLoad");
            }
        }

        [Fact]
        public void RunQueryDefTransformerWithParams()
        {
            var autoIdent = "transform-" + Guid.NewGuid();
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                session.Store(new SimpleItem
                {
                    Id = autoIdent,
                    Text = "TransformDocumentParamLoad"
                });
                tran.Commit();
            }
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var query = session.Query<IndexForIndexItem, IndexItem>();
                var page = session.GetQueryDef(query)
                    .Transform<TransformResult, TestParamTransformer>(
                        new Dictionary<string, string> { { "ident", autoIdent } })
                    .LoadPartial(0, 20);
                page.ShouldNotBeNull();
                page.TotalCount.ShouldBe(100);
                page.PageData.Count().ShouldBe(20);
                var items = page.PageData.ToArray();
                // test third item
                items[2].ShouldNotBeNull();
                items[2].Id.ShouldBe("test-2");
                items[2].Composited.ShouldBe("text-2|2");
                items[2].Computed.ShouldBe("TransformDocumentParamLoad");
                // test last item
                items[19].ShouldNotBeNull();
                items[19].Id.ShouldBe("test-19");
                items[19].Composited.ShouldBe("text-19|19");
                items[19].Computed.ShouldBe("TransformDocumentParamLoad");
            }
        }

        [Fact]
        public void RunQueryDefStream()
        {
            var autoIdent = "stream-" + Guid.NewGuid();
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                session.Store(new SimpleItem
                {
                    Id = autoIdent,
                    Text = "TransformStream"
                });
                tran.Commit();
            }
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var query = session.Query<IndexForIndexItem, IndexItem>();
                var stream = session.GetQueryDef(query)
                    .Transform<TransformResult, TestParamTransformer>(
                        new Dictionary<string, string> { { "ident", autoIdent } })
                    .Stream(_ => _);
                var items = new List<TransformResult>();
                foreach (var item in stream)
                {
                    items.Add(item);
                }
                items.Count.ShouldBe(100);
                // test third item
                items[2].ShouldNotBeNull();
                items[2].Id.ShouldBe("test-2");
                items[2].Composited.ShouldBe("text-2|2");
                items[2].Computed.ShouldBe("TransformStream");
                // test last item
                items[19].ShouldNotBeNull();
                items[19].Id.ShouldBe("test-19");
                items[19].Composited.ShouldBe("text-19|19");
                items[19].Computed.ShouldBe("TransformStream");
            }
        }

        [Fact]
        public void TypeIdentGenerating()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                session.GetTypeIdent<SimpleItem>().ShouldBe("simpleitems");
                session.GetTypeIdent<SomePerson>().ShouldBe("somepeople");
                session.GetTypeIdent<Generic<string>>().ShouldBe("generic`1");
            }
        }
        public class SomePerson { }
        public class Generic<T> { }
    }
}
