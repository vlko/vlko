using vlko.core.DBAccess;
using vlko.core.RavenDB.DBAccess;
using vlko.core.Roots;
using vlko.core.Setting;

namespace vlko.core.RavenDB
{
    public class DatabaseSettingProvider : ISettingProvider
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>True if there is value, otherwise false;</returns>
        public bool GetValue(string name, ref string value)
        {
            using (var session = DB.StartSession<RavenSession>())
            {
                var item = session.Load<AppSetting>(name, false);
                if (item != null)
                {
                    value = item.Value;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Saves the value for specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void SaveValue(string name, string value)
        {
            using (var session = DB.StartSession<RavenSession>())
            using (var tran = session.StartTransaction())
            {
                var dbItem = session.Load<AppSetting>(name, false);
                if (dbItem == null)
                {
                    dbItem = new AppSetting
                    {
                        Id = name,
                        Value = value
                    };
                }
                else
                {
                    dbItem.Value = value;

                }
                session.Store<AppSetting>(dbItem);
                tran.Commit();
            }
        }
    }
}