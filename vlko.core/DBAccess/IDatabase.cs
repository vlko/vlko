using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vlko.core.DBAccess
{
    public interface IDatabase<SessionType>: IDatabase where SessionType : ISession
    {
        SessionType CreateSession(ISessionOptions sessionOptions);
    }

    public interface IDatabase
    {

    }
}
