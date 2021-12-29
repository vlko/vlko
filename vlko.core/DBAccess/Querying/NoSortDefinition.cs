using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vlko.core.DBAccess.Querying
{
    public class NoSortDefinition
    {
        private NoSortDefinition() { }
        public static NoSortDefinition Use() => new NoSortDefinition();
    }
}
