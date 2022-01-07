using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using vlko.core.DBAccess;
using vlko.core.DBAccess.PostgreSQL;
using vlko.core.InversionOfControl;
using Xunit;

namespace vlko.core.tests.DBAccess.PostgreSQL
{
    public class SimpleDBOperations
    {

        [Fact]
        public void MakeSession()
        {
            var dbIdent = "test";
            Database.RegisterDB(dbIdent);
            using (var session = DB.StartSession<PostgreSQLSession>(dbIdent))
            {
                session.ShouldNotBeNull();
                session.Connection.ShouldNotBeNull();
            }
        }

        [Fact]
        public void MakeTestQuery()
        {
            var dbIdent = "test";
            Database.RegisterDB(dbIdent);
            using (var session = DB.StartSession<PostgreSQLSession>(dbIdent))
            {
                session.ShouldNotBeNull();
                session.Connection.ShouldNotBeNull();
                using (var command = session.Connection.CreateCommand())
                {
                    command.CommandText = "select 1+2";
                    var result = command.ExecuteScalar();
                    result.ShouldNotBeNull();
                    result.ShouldBe(3);
                }
            }
        }

        [Fact]
        public async Task MakeTestQueryAsync()
        {
            var dbIdent = "test";
            Database.RegisterDB(dbIdent);
            using (var session = DB.StartSession<PostgreSQLSession>(dbIdent))
            {
                session.ShouldNotBeNull();
                session.Connection.ShouldNotBeNull();
                using (var command = session.Connection.CreateCommand())
                {
                    command.CommandText = "select 1+2";
                    var result = await command.ExecuteScalarAsync();
                    result.ShouldNotBeNull();
                    result.ShouldBe(3);
                }
            }
        }
    }
}
