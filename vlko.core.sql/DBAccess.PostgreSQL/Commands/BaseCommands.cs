using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vlko.core.DBAccess;
using vlko.core.DBAccess.PostgreSQL;

namespace vlko.core.DBAccess.PostgreSQL.Commands
{
    public class BaseCommands : ICommands<PostgreSQLSession>
    {
        public PostgreSQLSession Session { get; private set; }

        public void Init(PostgreSQLSession session)
        {
            Session = session;
        }
    }
}
