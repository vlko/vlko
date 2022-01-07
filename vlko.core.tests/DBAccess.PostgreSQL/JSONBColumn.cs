using Dapper.Contrib.Extensions;
using Npgsql;
using NpgsqlTypes;
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
    public class JSONBColumn
    {
        [Fact]
        public async Task StoreData()
        {
            var dbIdent = "test_store_json";
            Database.RegisterDB(dbIdent, new[] { typeof(SubClass) });
            using (var session = DB.StartSession<PostgreSQLSession>(dbIdent))
            using (var transaction = session.StartTransaction())
            {
                using (var command = session.Connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE jsondata (\"Id\" serial PRIMARY KEY,  \"Text\" varchar(254), \"Data\" jsonb)";
                    command.Transaction = session.NativeTransaction;
                    var result = command.ExecuteScalar();
                }
                // create first and test serial
                var item = new JsonData { Text = "test", Data = new SubClass{ Val = 1, TextVal = "test_val" } };
                await session.StoreAsync(item);
                item.Id.ShouldBe(1);

                // rollback changes
                transaction.Rollback();
            }
        }

        [Fact]
        public async Task LoadData()
        {
            var dbIdent = "test_load_json";
            Database.RegisterDB(dbIdent, new[] { typeof(SubClass) });
            using (var session = DB.StartSession<PostgreSQLSession>(dbIdent))
            using (var transaction = session.StartTransaction())
            {
                using (var command = session.Connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE jsondata (\"Id\" serial PRIMARY KEY,  \"Text\" varchar(254), \"Data\" jsonb)";
                    command.Transaction = session.NativeTransaction;
                    var result = command.ExecuteScalar();
                }
                // create first and test serial
                var item = new JsonData { Text = "test", Data = new SubClass{ Val = 1, TextVal = "test_val" } };
                await session.StoreAsync(item);
                item.Id.ShouldBe(1);

                item = await session.LoadAsync<JsonData>(1);
                item.Id.ShouldBe(1);
                item.Data.ShouldNotBeNull();
                item.Data.Val.ShouldBe(1);
                item.Data.TextVal.ShouldBe("test_val");
                // rollback changes
                transaction.Rollback();
            }
        }

        [Fact]
        public async Task LoadAndStoreGenerics()
        {
            var dbIdent = "test_generics_json";
            Database.RegisterDB(dbIdent, new[] { typeof(SubClass), typeof(SubClass2) });
            using (var session = DB.StartSession<PostgreSQLSession>(dbIdent))
            using (var transaction = session.StartTransaction())
            {
                using (var command = session.Connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE jsondata (\"Id\" serial PRIMARY KEY,  \"Text\" varchar(254), \"Data\" jsonb)";
                    command.Transaction = session.NativeTransaction;
                    var result = command.ExecuteScalar();
                }
                // create first and test serial
                var item = new JsonGenerics<SubClass> { Text = "test", Data = new SubClass{ Val = 1, TextVal = "test_val" } };
                await session.StoreAsync(item);
                item.Id.ShouldBe(1);
                var item2 = new JsonGenerics<SubClass2> { Text = "test", Data = new SubClass2{ Val = "test", Fload = 2.5, OtherText = "test_other" } };
                await session.StoreAsync(item2);
                item2.Id.ShouldBe(2);

                var testItem = await session.LoadAsync<JsonGenerics<SubClass>>(1);
                testItem.Id.ShouldBe(1);
                testItem.Data.ShouldNotBeNull();
                testItem.Data.Val.ShouldBe(1);
                testItem.Data.TextVal.ShouldBe("test_val");

                var testItem2 = await session.LoadAsync<JsonGenerics<SubClass2>>(2);
                testItem2.Id.ShouldBe(2);
                testItem2.Data.ShouldNotBeNull();
                testItem2.Data.Val.ShouldBe("test");
                testItem2.Data.Fload.ShouldBe(2.5);
                testItem2.Data.OtherText.ShouldBe("test_other");

                // rollback changes
                transaction.Rollback();
            }
        }

        [Table("jsondata")]
        public class JsonData
        {
            [Key]
            public int Id { get; set; }
            public string Text { get; set; }
            public SubClass Data { get; set; }
        }
        public class SubClass
        {
            public int Val { get; set; }
            public string TextVal { get; set; }

        }

        public class SubClass2
        {
            public string Val { get; set; }
            public double Fload { get; set; }
            public string OtherText { get; set; }

        }

        [Table("jsondata")]
        public class JsonGenerics<T>
        {
            [Key]
            public int Id { get; set; }
            public string Text { get; set; }

            public T Data { get; set; }
        }
    }

}
