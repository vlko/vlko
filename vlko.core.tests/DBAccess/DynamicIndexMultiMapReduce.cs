using finstat.BLL.tests;
using Microsoft.VisualBasic;
using Raven.Client.Documents;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using vlko.core.DBAccess;
using vlko.core.InversionOfControl;
using vlko.core.RavenDB.DBAccess;
using vlko.core.tests.DBAccess.DynamicModel;
using vlko.core.tests.DBAccess.Model;
using Xunit;

namespace vlko.core.tests.DBAccess
{
    public class DynamicIndexMultiMapReduce : LocalStaticMemoryClientTest
    {
        private static string storeIdent = "dynamic_index";
        static DynamicIndexMultiMapReduce()
        {
            var scope = IoC.Scope[storeIdent];
            scope.AddCatalogAssembly(Assembly.Load("vlko.core"));
            scope.AddCatalogAssembly(Assembly.Load("vlko.core.RavenDB"));
            scope.Initialize();
        }

        public DynamicIndexMultiMapReduce()
        {
            var scope = IoC.Scope[storeIdent];
            SetUp(InitDB, storeIdent, scope);
        }

        private void InitDB(string storeIdent)
        {
            FillWithData(storeIdent);
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                new DynamicModelIndex().Execute(session.Advanced.DocumentStore);
                WaitForIndexing(session);
                WaitForUserToContinueTheTest(session.Advanced.DocumentStore);
            }
        }

        [Fact]
        public void CheckIfAllIndexed()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var items = session.Query<DynamicModelIndex, DynamicModelResult>().ToArray();
                items.Length.ShouldBe(4);
                items[0].Id.ShouldBe("0001");
                items[0].Name.ShouldBe("first");
                items[0].Year.ShouldBe(2002);
                items[1].Id.ShouldBe("0002");
                items[1].Name.ShouldBe("second");
                items[1].Year.ShouldBe(2000);
                items[2].Id.ShouldBe("0003");
                items[2].Name.ShouldBe("third");
                items[2].Year.ShouldBe(2003);
                items[3].Id.ShouldBe("0004");
                items[3].Name.ShouldBeNull();
            }
        }
        [Fact]
        public void CheckIfProperNullConditions()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var items = session.Query<DynamicModelIndex, DynamicModelResult>()
                    .Where(x => x.Year == null).ToArray();
                items.Length.ShouldBe(1);
                items[0].Id.ShouldBe("0004");
                items = session.DocumentQuery<DynamicModelIndex, DynamicModelResult>()
                    .WhereEquals("Analysis", null).ToArray();
                items.Length.ShouldBe(2);
            }
        }

        [Fact]
        public void CheckProjectionFromDynamicFields()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var items = session.Query<DynamicModelIndex, DynamicModelProjection>()
                    .ProjectInto<DynamicModelProjection>().ToArray();
                items.Length.ShouldBe(4);
                items[0].Id.ShouldBe("0001");
                items[0].Name.ShouldBe("first");
                items[0].Year.ShouldBe(2002);
                items[0].Created.ShouldBe(new DateTime(2000, 1, 1));
                items[0].Cancelled.ShouldBeNull();
                items[0].Subjects.ShouldNotBeNull();
                items[0].Subjects.Length.ShouldBe(2);
                items[0].Subjects[0].ShouldBe("first subject");
                items[0].Subjects[1].ShouldBe("second subject");
                items[0].Assets.ShouldBe(102);
                items[0].Analysis.ShouldBe(112);
                items[0].RelatedDate.ShouldNotBeNull();
                items[0].RelatedDate.Length.ShouldBe(2);

                items[1].Id.ShouldBe("0002");
                items[1].Name.ShouldBe("second");
                items[1].Year.ShouldBe(2000);
                items[1].Created.ShouldBe(new DateTime(1999, 1, 1));
                items[1].Cancelled.ShouldBe(new DateTime(2003, 1, 1));
                items[1].Subjects.ShouldBeNull();
                items[1].Assets.ShouldBe(200);
                items[1].Analysis.ShouldBe(210);
                items[1].RelatedDate.ShouldBeNull();

                items[2].Id.ShouldBe("0003");
                items[2].Name.ShouldBe("third");
                items[2].Year.ShouldBe(2003);
                items[2].Created.ShouldBe(new DateTime(2003, 1, 1));
                items[2].Cancelled.ShouldBeNull();
                items[2].Subjects.ShouldBeNull();
                items[2].Assets.ShouldBe(300);
                items[2].Analysis.ShouldBeNull();
                items[2].RelatedDate.ShouldBeNull();

                items[3].Id.ShouldBe("0004");
                items[3].Name.ShouldBeNull();
                items[3].Created.ShouldBeNull();
                items[3].Cancelled.ShouldBeNull();
                items[3].Subjects.ShouldBeNull();
                items[3].Assets.ShouldBeNull();
                items[3].Analysis.ShouldBeNull();
                items[3].RelatedDate.ShouldNotBeNull();
                items[3].RelatedDate.Length.ShouldBe(1);
            }
        }

        [Fact]
        public void CheckLinqQueriesFromDynamicFields()
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            {
                var items = session.Query<DynamicModelIndex, DynamicModelProjection>()
                    .Where(x => x.Year == null)
                    .ProjectInto<DynamicModelProjection>().ToArray();
                items.Length.ShouldBe(1);
                items[0].Id.ShouldBe("0004");
                items = session.Query<DynamicModelIndex, DynamicModelProjection>()
                    .Where(x => x.Analysis == null)
                    .ProjectInto<DynamicModelProjection>().ToArray();
                items.Length.ShouldBe(2);
                items = session.Query<DynamicModelIndex, DynamicModelProjection>()
                    .Where(x => x.Created >= new DateTime(2000, 1, 1))
                    .ProjectInto<DynamicModelProjection>().ToArray();
                items.Length.ShouldBe(2);
            }
        }

        internal static void FillWithData(string storeIdent)
        {
            using (var session = DB.StartSession<RavenSession>(storeIdent))
            using (var tran = session.StartTransaction())
            {
                session.Store(new DataModel
                {
                    Id = "0001",
                    Name = "first",
                    Created = new DateTime(2000, 1, 1)
                });
                session.Store(new HistoryDataModel
                {
                    Id = "history-0001",
                    RelId = "0001",
                    Subjects = new[] { "first subject", "second subject" }
                });
                session.Store(new FinancialDataModel
                {
                    Id = "0001|2000",
                    RelId = "0001",
                    Year = 2000,
                    Data = new Dictionary<string, double?> { { "1", 100 }, { "A1", 110 } }
                });
                session.Store(new FinancialDataModel
                {
                    Id = "0001|2001",
                    RelId = "0001",
                    Year = 2001,
                    Data = new Dictionary<string, double?> { { "1", 101 }, { "A1", 111 } }
                });
                session.Store(new FinancialDataModel
                {
                    Id = "0001|2002",
                    RelId = "0001",
                    Year = 2002,
                    Data = new Dictionary<string, double?> { { "1", 102 }, { "A1", 112 } }
                });
                session.Store(new ORDateDataModel
                {
                    Id = "or001",
                    RelId = "0001",
                    Date = new DateTime(2000, 1, 1)
                });
                session.Store(new ORDateDataModel
                {
                    Id = "or002",
                    RelId = "0001",
                    Date = new DateTime(2001, 1, 1)
                });
                session.Store(new DataModel
                {
                    Id = "0002",
                    Name = "second",
                    Created = new DateTime(1999, 1, 1),
                    Cancelled = new DateTime(2003, 1, 1)
                });
                session.Store(new FinancialDataModel
                {
                    Id = "0002|2000",
                    RelId = "0002",
                    Year = 2000,
                    Data = new Dictionary<string, double?> { { "1", 200 }, { "A1", 210 } }
                });
                session.Store(new DataModel
                {
                    Id = "0003",
                    Name = "third",
                    Created = new DateTime(2003, 1, 1)
                });
                session.Store(new FinancialDataModel
                {
                    Id = "0003|2003",
                    RelId = "0003",
                    Year = 2003,
                    Data = new Dictionary<string, double?> { { "1", 300 } }
                });
                session.Store(new ORDateDataModel
                {
                    Id = "or004",
                    RelId = "0004",
                    Date = new DateTime(2000, 1, 1)
                });
                tran.Commit();
            }
        }


    }
}
