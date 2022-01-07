using Dapper;
using Dapper.Contrib.Extensions;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vlko.core.DBAccess;
using vlko.core.DBAccess.PostgreSQL;
using Xunit;

namespace vlko.core.tests.DBAccess.PostgreSQL
{
    public class QueryData
    {
        [Fact]
        public async Task QueryOrder()
        {
            var dbIdent = "test";
            Database.RegisterDB(dbIdent);
            using (var session = DB.StartSession<PostgreSQLSession>(dbIdent))
            using (var transaction = session.StartTransaction())
            {
                using (var command = session.Connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE tests (\"Id\" serial PRIMARY KEY,  \"Text\" varchar(254))";
                    command.Transaction = session.NativeTransaction;
                    var result = command.ExecuteScalar();
                }
                // create first and test serial
                var item = new Test { Text = "test1" };
                await session.StoreAsync(item);
                item.Id.ShouldBe(1);

                // create second and test serial
                item = new Test { Text = "test2" };
                await session.StoreAsync(item);
                item.Id.ShouldBe(2);

                // create third and test serial
                item = new Test { Text = "test3" };
                await session.StoreAsync(item);
                item.Id.ShouldBe(3);

                // run async query and test results
                var data = (await session.Connection.QueryAsync<Test>("select * from tests order by \"Text\" desc")).ToArray();
                data.ShouldNotBeNull();
                data.Length.ShouldBe(3);
                data[0].Text.ShouldBe("test3");
                data[1].Text.ShouldBe("test2");
                data[2].Text.ShouldBe("test1");
                // rollback changes
                transaction.Rollback();
            }
        }

        [Fact]
        public async Task QueryJoin()
        {
            var dbIdent = "test";
            Database.RegisterDB(dbIdent);
            using (var session = DB.StartSession<PostgreSQLSession>(dbIdent))
            using (var transaction = session.StartTransaction())
            {
                using (var command = session.Connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE mains (\"Id\" varchar(20) PRIMARY KEY,  \"Text\" varchar(254))";
                    command.Transaction = session.NativeTransaction;
                    var result = command.ExecuteScalar();
                }
                using (var command = session.Connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE refs (\"Id\" serial PRIMARY KEY, \"RefId\" varchar(20),  \"Data\" varchar(254))";
                    command.Transaction = session.NativeTransaction;
                    var result = command.ExecuteScalar();
                }
                // fill with data
                var item = new Main { Id = "001", Text = "test1" };
                await session.StoreAsync(item);
                var refItem = new Ref { RefId = "001", Data = "refTest1" };
                await session.StoreAsync(refItem);
                item = new Main { Id = "002", Text = "test2" };
                await session.StoreAsync(item);
                refItem = new Ref { RefId = "002", Data = "refTest2" };
                await session.StoreAsync(refItem);
                item = new Main { Id = "003", Text = "test3" };
                await session.StoreAsync(item);
                refItem = new Ref { RefId = "003", Data = "refTest3" };
                await session.StoreAsync(refItem);

                // run async query and test results
                var data = (await session.Connection.QueryAsync<Main, Ref, Main>("select * from mains T left join refs R on T.\"Id\" = R.\"RefId\" order by \"Text\" desc", (main, reference) => { main.Reference = reference; return main; })).ToArray();
                data.ShouldNotBeNull();
                data.Length.ShouldBe(3);
                data[0].Text.ShouldBe("test3");
                data[0].Reference.Data.ShouldBe("refTest3");
                data[1].Text.ShouldBe("test2");
                data[1].Reference.Data.ShouldBe("refTest2");
                data[2].Text.ShouldBe("test1");
                data[2].Reference.Data.ShouldBe("refTest1");
                // rollback changes
                transaction.Rollback();
            }
        }

        [Fact]
        public async Task QueryJoinWithJson()
        {
            var dbIdent = "test-query-json";
            Database.RegisterDB(dbIdent, new[] { typeof(JsonData) });
            using (var session = DB.StartSession<PostgreSQLSession>(dbIdent))
            using (var transaction = session.StartTransaction())
            {
                using (var command = session.Connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE mainjsons (\"Id\" varchar(20) PRIMARY KEY,  \"Text\" varchar(254))";
                    command.Transaction = session.NativeTransaction;
                    var result = command.ExecuteScalar();
                }
                using (var command = session.Connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE refjsons (\"Id\" serial PRIMARY KEY, \"RefId\" varchar(20),  \"Data\" jsonb)";
                    command.Transaction = session.NativeTransaction;
                    var result = command.ExecuteScalar();
                }
                // fill with data
                var item = new MainJson { Id = "001", Text = "test1" };
                await session.StoreAsync(item);
                var refItem = new RefJson { RefId = "001", Data = new JsonData { Number = 1, Decimal = 1.2 } };
                await session.StoreAsync(refItem);
                item = new MainJson { Id = "002", Text = "test2" };
                await session.StoreAsync(item);
                refItem = new RefJson { RefId = "002", Data  = new JsonData { Number = 2, Decimal = 1.8 } };
                await session.StoreAsync(refItem);
                item = new MainJson { Id = "003", Text = "test3" };
                await session.StoreAsync(item);
                refItem = new RefJson { RefId = "003", Data  = new JsonData { Number = 3, Decimal = 1.5 } };
                await session.StoreAsync(refItem);

                // run async query and test results
                var data = (await session.Connection.QueryAsync<MainJson, RefJson, MainJson>("select * from mainjsons T left join refjsons R on T.\"Id\" = R.\"RefId\" order by \"Text\" desc", (main, reference) => { main.Reference = reference; return main; })).ToArray();
                data.ShouldNotBeNull();
                data.Length.ShouldBe(3);
                data[0].Text.ShouldBe("test3");
                data[0].Reference.Data.ShouldNotBeNull();
                data[0].Reference.Data.Number.ShouldBe(3);
                data[1].Text.ShouldBe("test2");
                data[1].Reference.Data.ShouldNotBeNull();
                data[1].Reference.Data.Number.ShouldBe(2);
                data[2].Text.ShouldBe("test1");
                data[2].Reference.Data.ShouldNotBeNull();
                data[2].Reference.Data.Number.ShouldBe(1);

                // run async query and test results
                 data = (await session.Connection.QueryAsync<MainJson, RefJson, MainJson>("select T.*, R.* from mainjsons T"
                     + " left join refjsons R on T.\"Id\" = R.\"RefId\""
                     + " ,LATERAL jsonb_to_record(R.\"Data\") AS l(\"Decimal\" float)"
                     + " order by l.\"Decimal\" desc", (main, reference) => { main.Reference = reference; return main; })).ToArray();
                data.ShouldNotBeNull();
                data.Length.ShouldBe(3);
                data[0].Text.ShouldBe("test2");
                data[0].Reference.Data.ShouldNotBeNull();
                data[0].Reference.Data.Number.ShouldBe(2);
                data[1].Text.ShouldBe("test3");
                data[1].Reference.Data.ShouldNotBeNull();
                data[1].Reference.Data.Number.ShouldBe(3);
                data[2].Text.ShouldBe("test1");
                data[2].Reference.Data.ShouldNotBeNull();
                data[2].Reference.Data.Number.ShouldBe(1);
                // rollback changes
                transaction.Rollback();
            }
        }



        public class Main
        {
            [ExplicitKey]
            public string Id { get; set; }
            public string Text { get; set; }
            [Computed]
            public Ref Reference { get; set; }
        }

        public class Ref
        {
            [Key]
            public int Id { get; set; }

            public string RefId { get; set; }
            public string Data { get; set; }
        }

        public class MainJson
        {
            [ExplicitKey]
            public string Id { get; set; }
            public string Text { get; set; }
            [Computed]
            public RefJson Reference { get; set; }
        }

        public class RefJson
        {
            [Key]
            public int Id { get; set; }

            public string RefId { get; set; }
            public JsonData Data { get; set; }
        }

        public class JsonData
        {
            public int Number { get; set; }
            public double Decimal { get; set; }
        }

        public class Test
        {
            [Key]
            public int Id { get; set; }
            public string Text { get; set; }
        }
    }
}
