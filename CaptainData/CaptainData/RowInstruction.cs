using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Dapper;

namespace CaptainData
{
    public class RowInstruction
    {
        private readonly Dictionary<string, ColumnInstruction> _columnInstructions = new Dictionary<string, ColumnInstruction>();

        private string _tableName;

        public void SetTable(string tableName)
        {
            _tableName = tableName;
        }

        public object this[string index]
        {
            set
            {
                _columnInstructions[index] = new ColumnInstruction { Value = value };
            }
        }

        public void Apply(SqlConnection connection)
        {
            var columnValues = _columnInstructions.Where(x => x.Key != null).ToDictionary(x => x.Key, x => x.Value);
            var sql = $"INSERT INTO {_tableName} ({string.Join(", ", columnValues.Keys)}) VALUES ({string.Join(", ", columnValues.Values.Select(x => ToSqlValue(x.Value)))})";
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

        public bool IsDefinedFor(string columnName) => _columnInstructions.ContainsKey(columnName);
    }

    public class ColumnInstruction
    {
        public object Value { get; set; }
    }
}