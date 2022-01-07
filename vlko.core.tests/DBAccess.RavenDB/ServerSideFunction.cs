using finstat.BLL.tests;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using vlko.core.DBAccess;
using vlko.core.InversionOfControl;
using vlko.core.RavenDB.DBAccess;
using vlko.core.tests.DBAccess.RavenDB.ServerSideModel;
using Xunit;

namespace vlko.core.tests.DBAccess.RavenDB
{
    public class ServerSideFunction : LocalStaticMemoryClientTest
    {
        private static string storeIdent = "server_side_index";
        static ServerSideFunction()
        {
            var scope = IoC.Scope[storeIdent];
            scope.AddCatalogAssembly(Assembly.Load("vlko.core"));
            scope.AddCatalogAssembly(Assembly.Load("vlko.core.RavenDB"));
            scope.Initialize();
        }

        public ServerSideFunction()
        {
            var scope = IoC.Scope[storeIdent];
            SetUp(InitDB, storeIdent, scope);
        }

        private void InitDB(string storeIdent)
        {
            FillWithData(storeIdent);
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                new ServerSideFunctionIndex().Execute(session.Advanced.DocumentStore);
                WaitForIndexing(session);
                WaitForUserToContinueTheTest(session.Advanced.DocumentStore);
            }
        }


        [Fact]
        public void CheckIfAllIndexed()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var items = session.Query<ServerSideFunctionIndex, ServerSideResult>().ToArray();
                items.Length.ShouldBe(4);
                items[0].Id.ShouldBe("0001");
                items[0].Name.ShouldBe("first");
                items[0].Year.ShouldBe(2002);
                items[0].AssetActual.ShouldBe(200);
                items[0].AssetPrev.ShouldBe(100);
                items[0].AssetChange.ShouldBe(1);

                items[1].Id.ShouldBe("0002");
                items[1].Name.ShouldBe("second");
                items[1].Year.ShouldBe(2000);
                items[1].AssetActual.ShouldBe(100);
                items[1].AssetPrev.ShouldBe(200);
                items[1].AssetChange.ShouldBe(-0.5);

                items[2].Id.ShouldBe("0003");
                items[2].Name.ShouldBe("third");
                items[2].Year.ShouldBe(2003);
                items[2].AssetActual.ShouldBe(300);
                items[2].AssetPrev.ShouldBeNull();
                items[2].AssetChange.ShouldBeNull();

                items[3].Id.ShouldBe("0004");
                items[3].Name.ShouldBeNull();
                items[3].Year.ShouldBe(2000);
                items[3].AssetActual.ShouldBeNull();
                items[3].AssetPrev.ShouldBe(400);
                items[3].AssetChange.ShouldBeNull();
            }
        }

        [Fact]
        public void CheckIfProperNullConditions()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var items = session.Query<ServerSideFunctionIndex, ServerSideResult>()
                    .Where(x => x.Name == null).ToArray();
                items.Length.ShouldBe(1);
                items[0].Id.ShouldBe("0004");
                items = session.Query<ServerSideFunctionIndex, ServerSideResult>()
                    .Where(x => x.AssetChange == null).ToArray();
                items.Length.ShouldBe(2);
            }
        }

        internal static void FillWithData(string storeIdent)
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                session.Store(new RelatedModel
                {
                    Id = "0001",
                    Name = "first",
                });
                session.Store(new DataModel
                {
                    Id = "0001|2000",
                    RelId = "0001",
                    Year = 2000,
                    Data = new Dictionary<string, ValueSubModel> { { "1", new ValueSubModel(201, 101) } }
                });
                session.Store(new DataModel
                {
                    Id = "0001|2001",
                    RelId = "0001",
                    Year = 2001,
                    Data = new Dictionary<string, ValueSubModel> { { "1", new ValueSubModel(202, 102) } }
                });
                session.Store(new DataModel
                {
                    Id = "0001|2002",
                    RelId = "0001",
                    Year = 2002,
                    Data = new Dictionary<string, ValueSubModel> { { "1", new ValueSubModel(200, 100) } }
                });
                session.Store(new RelatedModel
                {
                    Id = "0002",
                    Name = "second",
                });
                session.Store(new DataModel
                {
                    Id = "0002|2000",
                    RelId = "0002",
                    Year = 2000,
                    Data = new Dictionary<string, ValueSubModel> { { "1", new ValueSubModel(100, 200) } }
                });
                session.Store(new RelatedModel
                {
                    Id = "0003",
                    Name = "third",
                });
                session.Store(new DataModel
                {
                    Id = "0003|2003",
                    RelId = "0003",
                    Year = 2003,
                    Data = new Dictionary<string, ValueSubModel> { { "1", new ValueSubModel(300, null) } }
                });
                session.Store(new DataModel
                {
                    Id = "0004|2000",
                    RelId = "0004",
                    Year = 2000,
                    Data = new Dictionary<string, ValueSubModel> { { "1", new ValueSubModel(null, 400) } }
                });
                tran.Commit();
            }
        }


    }
}
