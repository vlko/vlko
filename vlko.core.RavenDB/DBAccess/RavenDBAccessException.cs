using System;

namespace vlko.core.RavenDB.DBAccess
{

    [Serializable]
    public class RavenDBAccessException : Exception
    {
        public RavenDBAccessException() { }
        public RavenDBAccessException(string message) : base(message) { }
        public RavenDBAccessException(string message, Exception inner) : base(message, inner) { }
        protected RavenDBAccessException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}