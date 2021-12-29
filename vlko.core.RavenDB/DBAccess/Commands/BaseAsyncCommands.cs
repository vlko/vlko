using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vlko.core.DBAccess;

namespace vlko.core.RavenDB.DBAccess.Commands
{
    public class BaseAsyncCommands : ICommands<RavenAsyncSession>
    {
        public RavenAsyncSession Session { get; private set; }

        public void Init(RavenAsyncSession session)
        {
            Session = session;
        }
    }
}
