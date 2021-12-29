using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vlko.core.InversionOfControl
{
    public class IoCScopeHolder : IoCScope
    {
        public IoCScopeHolder() : base(null) 
        {
            _holder = new Dictionary<string, IoCScope>(8);
        }

        IDictionary<string, IoCScope> _holder;

        public IoCScope this[string ident]
        {
            get
            {
                if (string.IsNullOrEmpty(ident))
                {
                    return this;
                }
                if (!_holder.ContainsKey(ident))
                {
                    _holder[ident] = new IoCScope(ident);
                }
                return _holder[ident];
            }
        }
    }
}
