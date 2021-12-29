using System;
using System.Collections.Generic;
using System.Text;

namespace vlko.core.web.Commands.Model
{
    public class TokenResult<T>
    {
        public T Status { get; set; }
        public string VerifyToken { get; set; }
    }
}
