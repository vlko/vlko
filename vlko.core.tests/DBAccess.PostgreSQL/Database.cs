using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using vlko.core.DBAccess;
using vlko.core.DBAccess.PostgreSQL;
using vlko.core.InversionOfControl;

namespace vlko.core.tests.DBAccess.PostgreSQL
{
    public class Database
    {
        private static string _metaDBConnectionString = "Server=localhost;Database={0};Port=5432;User Id={1};Password={2};";
        public static void RegisterDB(string dbIdent, Type[] jsonTypes = null)
        {
            lock (typeof(Database))
            {
                if (!DB.IsRegistered(dbIdent))
                {
                    IoC.Scope[dbIdent].AddCatalogAssembly(Assembly.Load("vlko.core.web"));
                    IoC.Scope[dbIdent].Initialize();
                    LoadEnvironmentSettings();
                    var database = new PostgreSQLDatabase(
                        string.Format(_metaDBConnectionString,
                        Environment.GetEnvironmentVariable("PQSQL_TEST_DB"),
                        Environment.GetEnvironmentVariable("PQSQL_USER"),
                        Environment.GetEnvironmentVariable("PQSQL_PSWD")));
                    foreach (var type in jsonTypes ?? new Type[0])
                    {
                        database.RegisterJsonType(type);
                    }
                    DB.RegisterIdent(dbIdent, IoC.Scope[dbIdent])
                        .RegisterSessionProvider(database);
                }
            }
        }

        private static void LoadEnvironmentSettings()
        {
            try
            {
                var launchSettingsPath = Path.Combine("Properties", "launchSettings.json");

                using (var file = File.OpenText(launchSettingsPath))
                {
                    var reader = new JsonTextReader(file);
                    var jObject = JObject.Load(reader);

                    var variables = jObject
                        .GetValue("profiles")
                        .SelectMany(profiles => profiles.Children())
                        .SelectMany(profile => profile.Children<JProperty>())
                        .Where(prop => prop.Name == "environmentVariables")
                        .SelectMany(prop => prop.Value.Children<JProperty>())
                        .ToList();

                    foreach (var variable in variables)
                    {
                        Environment.SetEnvironmentVariable(variable.Name, variable.Value.ToString());
                    }
                }
            }
            catch { }
        }
    }
}
