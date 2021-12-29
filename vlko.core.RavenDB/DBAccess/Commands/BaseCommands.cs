using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vlko.core.DBAccess;

namespace vlko.core.RavenDB.DBAccess.Commands
{
    public class BaseCommands : ICommands<RavenSession>
    {
        public RavenSession Session { get; private set; }

        public void Init(RavenSession session)
        {
            Session = session;
        }
    }
}
