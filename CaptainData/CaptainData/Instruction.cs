using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Dapper;

namespace CaptainData
{
    public class Instruction
    {
        private readonly Dictionary<string, object> _columnValues = new Dictionary<string, object>();

        public string TableName { get; set; }

        public object this[string index]
        {
            get
            {
                return _columnValues[index];
            }
            set
            {
                _columnValues[index] = value;
            }
        }

        public void Apply(SqlConnection connection)
        {
            var sql =
                $"INSERT INTO {TableName} ({string.Join(", ", _columnValues.Keys)}) VALUES ({string.Join(", ", _columnValues.Values.Select(ToSqlValue))})";
            connection.Execute(sql);
        }

        private object ToSqlValue(object o)
        {
            if (o.GetType() == typeof(string))
            {
                return "'" + o + "'";
            }

            return o.ToString();
        }
    }
}