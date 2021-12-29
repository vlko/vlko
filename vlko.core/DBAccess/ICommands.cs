using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vlko.core.DBAccess
{
    public interface ICommands
    {

    }
    public interface ICommands<TSessionType> : ICommands where TSessionType: ISession
    {
        TSessionType Session { get; }
        void Init(TSessionType session);
    }
}
