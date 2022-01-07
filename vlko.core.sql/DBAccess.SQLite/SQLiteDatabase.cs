using Microsoft.Data.Sqlite;
using NLog;
using Npgsql;
using System;

namespace vlko.core.DBAccess.SQLite
{
    public class SQLiteDatabase : IDatabase<SQLiteSession>
    {
        internal static Logger Logger = LogManager.GetLogger("PostgreSQLDatabase");
        private string _connectionString;
        public SQLiteDatabase(string connectionString)
        {
            _connectionString = connectionString;
            NpgsqlConnection.GlobalTypeMapper.UseJsonNet();
        }

        public SQLiteSession CreateSession(ISessionOptions sessionOptions)
        {
            var sessionIdent = Guid.NewGuid();
            Logger.Trace($"Creating session [{sessionIdent}] ...");
            return new SQLiteSession(new SqliteConnection(_connectionString), sessionIdent);
        }
    }
}
