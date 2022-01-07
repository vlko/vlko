using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using NLog;
using Npgsql;
using vlko.core.DBAccess;

namespace vlko.core.DBAccess.PostgreSQL
{
    public class PostgreSQLDatabase : IDatabase<PostgreSQLSession>
    {
        internal static Logger Logger = LogManager.GetLogger("PostgreSQLDatabase");
        private string _connectionString;
        public PostgreSQLDatabase(string connectionString)
        {
            _connectionString = connectionString;
            NpgsqlConnection.GlobalTypeMapper.UseJsonNet();
        }
        public void RegisterJsonType(Type type)
        {
            SqlMapper.AddTypeHandler(type, new JsonbMapper(type));
        }
        public PostgreSQLSession CreateSession(ISessionOptions sessionOptions)
        {
            var sessionIdent = Guid.NewGuid();
            Logger.Trace($"Creating session [{sessionIdent}] ...");
            return new PostgreSQLSession(new Npgsql.NpgsqlConnection(_connectionString), sessionIdent);
        }
    }
}
