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
    public class StoreLoadInTransaction
    {
        [Fact]
        public async Task StoreAutoId()
        {
            var dbIdent = "test";
            Database.RegisterDB(dbIdent);
            using (var session = DB.StartSession<PostgreSQLSession>(dbIdent))
            using (var transaction = session.StartTransaction())
            {
                using (var command = session.Connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE test (\"Id\" serial PRIMARY KEY,  \"Text\" varchar(254))";
                    command.Transaction = session.NativeTransaction;
                    var result = command.ExecuteScalar();
                }
                // create first and test serial
                var item = new Test { Text = "test" };
                await session.StoreAsync(item);
                item.Id.ShouldBe(1);

                // create second and test serial
                item = new Test { Text = "test2" };
                await session.StoreAsync(item);
                item.Id.ShouldBe(2);

                // rollback changes
                transaction.Rollback();
            }
        }

        [Fact]
        public async Task ChangeValue()
        {
            var dbIdent = "test";
            Database.RegisterDB(dbIdent);
            using (var session = DB.StartSession<PostgreSQLSession>(dbIdent))
            using (var transaction = session.StartTransaction())
            {
                using (var command = session.Connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE test (\"Id\" serial PRIMARY KEY,  \"Text\" varchar(254))";
                    command.Transaction = session.NativeTransaction;
                    var result = command.ExecuteScalar();
                }
                // create new
                var item = new Test { Text = "test" };
                await session.StoreAsync(item);
                item.Id.ShouldBe(1);

                // change text value
                item.Text = "test2";
                await session.StoreAsync(item);
                item.Id.ShouldBe(1);

                // load and test
                item = await session.LoadAsync<Test>(1);
                item.Id.ShouldBe(1);
                item.Text.ShouldBe("test2");

                // rollback changes
                transaction.Rollback();
            }
        }

        [Fact]
        public async Task LoadMultiple()
        {
            var dbIdent = "test";
            Database.RegisterDB(dbIdent);
            using (var session = DB.StartSession<PostgreSQLSession>(dbIdent))
            using (var transaction = session.StartTransaction())
            {
                using (var command = session.Connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE test (\"Id\" serial PRIMARY KEY,  \"Text\" varchar(254))";
                    command.Transaction = session.NativeTransaction;
                    var result = command.ExecuteScalar();
                }
                // create first and test serial
                var item = new Test { Text = "test" };
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

                var data = await session.LoadMoreAsyncToArray<Test>(2, 1, 3);
                data = data.OrderBy(x => x.Id).ToArray();
                data.ShouldNotBeNull();
                data.ShouldNotBeEmpty();
                data.Length.ShouldBe(3);
                data[0].Id.ShouldBe(1);
                data[1].Id.ShouldBe(2);
                data[2].Id.ShouldBe(3);

                // rollback changes
                transaction.Rollback();
            }
        }

        [Fact]
        public async Task Delete()
        {
            var dbIdent = "test";
            Database.RegisterDB(dbIdent);
            using (var session = DB.StartSession<PostgreSQLSession>(dbIdent))
            using (var transaction = session.StartTransaction())
            {
                using (var command = session.Connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE test (\"Id\" serial PRIMARY KEY,  \"Text\" varchar(254))";
                    command.Transaction = session.NativeTransaction;
                    var result = command.ExecuteScalar();
                }
                // create first and test serial
                var item = new Test { Text = "test" };
                await session.StoreAsync(item);
                item.Id.ShouldBe(1);

                // load and test
                item = await session.LoadAsync<Test>(1);
                item.Id.ShouldBe(1);
                item.Text.ShouldBe("test");

                // delete
                await session.DeleteAsync(item);

                // test if deleted
                item = await session.LoadAsync<Test>(1, false);
                item.ShouldBeNull();


                // rollback changes
                transaction.Rollback();
            }
        }

        [Table("test")]
        public class Test
        {
            [Key]
            public int Id { get; set; }
            public string Text { get; set; }
        }
    }
}
