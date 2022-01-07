using System;

namespace vlko.core.DBAccess.PostgreSQL
{

    [Serializable]
    public class PostgreSQLAccessException : Exception
    {
        public PostgreSQLAccessException() { }
        public PostgreSQLAccessException(string message) : base(message) { }
        public PostgreSQLAccessException(string message, Exception inner) : base(message, inner) { }
        protected PostgreSQLAccessException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}