using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vlko.core.InversionOfControl;

namespace vlko.core.DBAccess
{
    public static class DB
    {
        private const string DefaultDatabaseIdentConst = "#default_db_const#";
        private static IDictionary<string, DBInfoHolder> _registeredDatabases = new Dictionary<string, DBInfoHolder>();

        public static DBInfoHolder RegisterIdent(string ident = null, IoCScope scope = null)
        {
            var storeIdent = ident ?? DefaultDatabaseIdentConst;
            var dbInfoHolder = new DBInfoHolder {
                Ident = ident,
                Databases = new Dictionary<string, IDatabase>(), 
                Scope = scope ?? IoC.Scope };
            _registeredDatabases[storeIdent] = dbInfoHolder;
            return dbInfoHolder;
        }

        /// <summary>
        /// Check if ident is already registered;
        /// </summary>
        /// <param name="ident">Database ident</param>
        /// <returns>True if database ident already registered otherwise false;</returns>
        public static bool IsRegistered(string ident) => _registeredDatabases.ContainsKey(ident ?? DefaultDatabaseIdentConst);

        /// <summary>
		/// Starts the unit of work.
		/// </summary>
		/// <returns>New unit of work.</returns>
		public static SessionType StartSession<SessionType>(string ident = null, ISessionOptions sessionOptions = null)
                                    where SessionType : ISession
        {
            var storeIdent = ident ?? DefaultDatabaseIdentConst;
            if (!_registeredDatabases.ContainsKey(storeIdent))
            {
                throw new Exception(storeIdent == DefaultDatabaseIdentConst ?
                    "Default database not registered!"
                    : $"Database with ident {ident} not registered!");
            }
            var typeIdent = typeof(SessionType).FullName;
            if (!_registeredDatabases[storeIdent].Databases.ContainsKey(typeIdent))
            {
                throw new Exception($"Session type {typeIdent} not registered for database ident [{ident}]!");
            }
            var dbInfo = _registeredDatabases[storeIdent];
            var newSession = ((IDatabase<SessionType>)dbInfo.Databases[typeIdent]).CreateSession(sessionOptions);
            newSession.Init(dbInfo.Scope);
            return newSession;
        }

        public class DBInfoHolder
        {
            public string Ident { get; set; }
            internal Type DBType { get; set; }
            internal IDictionary<string, IDatabase> Databases { get; set; }
            internal IoCScope Scope { get; set; }

            public DBInfoHolder RegisterSessionProvider<SessionType>(IDatabase<SessionType> database) where SessionType : ISession
            {
                var typeIdent = typeof(SessionType).FullName;
                Databases[typeIdent] = database;
                return this;
            }
        }

        public static IDisposable StartSession<T>(object dbIdent)
        {
            throw new NotImplementedException();
        }
    }
}
