using Dapper;
using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vlko.core.DBAccess.PostgreSQL
{
    /// <summary>
    /// Custom Type Mapper that converts Object C# properties to JSON and vice versa.
    /// </summary>
    public class JsonbMapper : SqlMapper.TypeHandler<object>
    {
        private readonly Type _type;

        public JsonbMapper(Type type)
        {
            this._type = type;
        }
        public override dynamic Parse(object value)
        {
            if (value == null)
            {
                return null;
            }
            return JsonConvert.DeserializeObject(value.ToString(), _type);
        }

        public override void SetValue(System.Data.IDbDataParameter parameter, object value)
        {
            var param = (NpgsqlParameter)parameter;
            param.NpgsqlDbType = NpgsqlDbType.Jsonb;
            param.Value = value;
        }
    }
}
