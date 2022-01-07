using System;

namespace vlko.core.DBAccess.SQLite
{

    [Serializable]
    public class SQLiteAccessException : Exception
    {
        public SQLiteAccessException() { }
        public SQLiteAccessException(string message) : base(message) { }
        public SQLiteAccessException(string message, Exception inner) : base(message, inner) { }
        protected SQLiteAccessException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}