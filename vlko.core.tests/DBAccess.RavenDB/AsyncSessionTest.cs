using finstat.BLL.tests;
using Raven.Client.Documents;
using Raven.Client.Documents.Queries.Timings;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using vlko.core.DBAccess;
using vlko.core.InversionOfControl;
using vlko.core.RavenDB.DBAccess;
using vlko.core.tests.DBAccess.RavenDB.Model;
using Xunit;

namespace vlko.core.tests.DBAccess.RavenDB
{
    public class AsyncSessionTest : LocalStaticMemoryClientTest
    {
        private static string storeIdent = "async_session";
        static AsyncSessionTest()
        {
            var scope = IoC.Scope[storeIdent];
            scope.AddCatalogAssembly(Assembly.Load("vlko.core"));
            scope.AddCatalogAssembly(Assembly.Load("vlko.core.RavenDB"));
            scope.Initialize();
        }

        public AsyncSessionTest()
        {
            var scope = IoC.Scope[storeIdent];
            SetUp(InitDB, storeIdent, scope);
        }

        private void InitDB(string storeIdent)
        {
            SyncSessionTest.FillWithData(storeIdent);
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                new IndexForIndexItem().Execute(session.Advanced.DocumentStore);
                WaitForIndexing(session);
            }
        }

        [Fact]
        public async Task StoreLoadDelete()
        {
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                await session.StoreAsync(new SimpleItem
                {
                    Id = "delete-test",
                    Text = "test"
                });
                await tran.CommitAsync();
            }
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                var item = await session.LoadAsync<SimpleItem>("delete-test");
                item.ShouldNotBeNull();
                await session.DeleteAsync(item);
                await tran.CommitAsync();
            }

            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            {
                var item = await session.LoadAsync<SimpleItem>("delete-test", false);
                item.ShouldBeNull();
            }
        }

        [Fact]
        public async Task StoreInTransaction()
        {
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                await session.StoreAsync(new SimpleItem
                {
                    Id = "store-test",
                    Text = "test"
                });
                await tran.CommitAsync();
            }
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            {
                var item = await session.LoadAsync<SimpleItem>("store-test");
                item.ShouldNotBeNull();
                item.Id.ShouldBe("store-test");
                item.Text.ShouldBe("test");
            }
        }

        [Fact]
        public async Task LoadMore()
        {
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                for (int i = 0; i < 5; i++)
                {
                    await session.StoreAsync(new SimpleItem
                    {
                        Id = "more-test" + i,
                        Text = "test" + i
                    });
                }

                await tran.CommitAsync();
            }

            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            {
                var items = await session.LoadMoreAsync<SimpleItem>("more-test0", "more-test1", "more-test2", "more-test3", "more-test4", "not-existing");
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
        public async Task LoadMoreWitTransformer()
        {
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                await session.StoreAsync(new SimpleItem
                {
                    Id = "transform-load",
                    Text = "TransformDocumentLoad"
                });
                await tran.CommitAsync();
            }
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            {
                var items = await session.LoadMoreWithTransformerAsync<IndexItem, TestLoadTransformer, TransformResult>("test-0", "test-10", "test-99", "not-existing");
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
        public async Task LoadWithTransformer()
        {
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                await session.StoreAsync(new SimpleItem
                {
                    Id = "transform-load",
                    Text = "TransformDocumentLoad"
                });
                await tran.CommitAsync();
            }
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            {
                var item = await session.LoadWithTransformerAsync<IndexItem, TestLoadTransformer, TransformResult>("test-0");
                // test first item
                item.ShouldNotBeNull();
                item.Id.ShouldBe("test-0");
                item.Composited.ShouldBe("text-0|0");
                item.Computed.ShouldBe("TransformDocumentLoad");
            }
        }

        [Fact]
        public async Task RunQuery()
        {
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            {
                var items = await session.Query<IndexForIndexItem, IndexItem>().ToArrayAsync();
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
        public async Task RunQueryWithProjection()
        {
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            {
                var items = await session.QueryWithProjection<IndexForIndexItem, ProjectionItem>()
                        .ToArrayAsync();
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
        public async Task RunDocumentQuery()
        {
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            {
                var items = await session.DocumentQuery<IndexForIndexItem, IndexItem>().ToArrayAsync();
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
        public async Task RunDocoumentQueryWithProjection()
        {
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            {
                QueryTimings timings;
                var items = await session.DocumentQueryWithProjection<IndexForIndexItem, ProjectionItem>()
                    .Timings(out timings).ToArrayAsync();
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
        public async Task RunQueryDef()
        {
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            {
                var query = session.Query<IndexForIndexItem, IndexItem>();
                var page = await session.GetQueryDef(query)
                    .LoadPartialAsync(0, 100);
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
        public async Task RunQueryDefProjection()
        {
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            {
                var query = session.Query<IndexForIndexItem, IndexItem>();
                var page = await session.GetQueryDef(query)
                    .Project<ProjectionItem>()
                    .LoadPartialAsync(0, 100);
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
        public async Task RunQueryDefSimpleTransformer()
        {
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            {
                var query = session.Query<IndexForIndexItem, IndexItem>();
                var page = await session.GetQueryDef(query)
                    .Transform<TransformResult, TestTransformer>()
                    .LoadPartialAsync(0, 100);
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
        public async Task RunQueryDefTransformerWithLoad()
        {
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                await session.StoreAsync(new SimpleItem
                {
                    Id = "transform-load",
                    Text = "TransformDocumentLoad"
                });
                await tran.CommitAsync();
            }
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            {
                var query = session.Query<IndexForIndexItem, IndexItem>();
                var page = await session.GetQueryDef(query)
                    .Transform<TransformResult, TestLoadTransformer>()
                    .LoadPartialAsync(0, 20);
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
        public async Task RunQueryDefTransformerWithParams()
        {
            var autoIdent = "transform-" + Guid.NewGuid();
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                await session.StoreAsync(new SimpleItem
                {
                    Id = autoIdent,
                    Text = "TransformDocumentParamLoad"
                });
                await tran.CommitAsync();
            }
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            {
                var query = session.Query<IndexForIndexItem, IndexItem>();
                var page = await session.GetQueryDef(query)
                    .Transform<TransformResult, TestParamTransformer>(
                        new Dictionary<string, string> { { "ident", autoIdent } })
                    .LoadPartialAsync(0, 20);
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
        public async Task RunQueryDefStream()
        {
            var autoIdent = "stream-" + Guid.NewGuid();
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                await session.StoreAsync(new SimpleItem
                {
                    Id = autoIdent,
                    Text = "TransformStream"
                });
                await tran.CommitAsync();
            }
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
            {
                var query = session.Query<IndexForIndexItem, IndexItem>();
                var stream = session.GetQueryDef(query)
                    .Transform<TransformResult, TestParamTransformer>(
                        new Dictionary<string, string> { { "ident", autoIdent } })
                    .StreamAsync(_ => _);
                var items = new List<TransformResult>();
                await foreach (var item in stream)
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
            using (var session = DB.StartSession<RavenAsyncSession>(storeIdent))
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
